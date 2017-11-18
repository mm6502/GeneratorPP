using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using Digital.Slovensko.Ekosystem.GeneratorPP.Models.BySquare;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Digital.Slovensko.Ekosystem.GeneratorPP.Implementation
{
    /// <summary>
    /// Class for reading payment list documents and writing payment orders documents.
    /// </summary>
    public class ExcelManipulator
    {
        /// <summary>
        /// Gets or sets the bySquareEncoder.
        /// </summary>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        private IBySquareEncoder BySquareEncoder { get; set; }

        /// <summary>
        /// Gets or sets the image manipulator.
        /// </summary>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        private IImageManipulator ImageManipulator { get; set; }

        /// <summary>
        /// Gets the progress reporter.
        /// </summary>
        private IProgress<int> ProgressReporter { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelManipulator"/> class.
        /// </summary>
        public ExcelManipulator(IProgress<int> reporter)
        {
            this.ProgressReporter = reporter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelManipulator"/> class.
        /// </summary>
        /// <param name="bySquareEncoder">The BySquare encoder.</param>
        /// <param name="imageManipulator">The image manipulator.</param>
        /// <param name="progressReporter">The progress reporter.</param>
        public ExcelManipulator(IBySquareEncoder bySquareEncoder, IImageManipulator imageManipulator, IProgress<int> progressReporter) : this(progressReporter)
        {
            this.BySquareEncoder = bySquareEncoder;
            this.ImageManipulator = imageManipulator;
        }

        /// <summary>
        /// Reads the payment list document.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>Payment purpose and list of BySquareDocuments.</returns>
        public (string, List<Pay>) ReadPaymentList(string filePath)
        {
            var document = DocumentFormat.OpenXml.Packaging.SpreadsheetDocument.Open(filePath, false);
            var workbook = document.WorkbookPart.Workbook;
            var sharedStrings = workbook.WorkbookPart.SharedStringTablePart.SharedStringTable;

            var sheet = workbook.Sheets.OfType<Sheet>().First();
            var worksheetPart = (DocumentFormat.OpenXml.Packaging.WorksheetPart)document.WorkbookPart.GetPartById(sheet.Id);
            var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

            // bank account
            var account = new BankAccount
            {
                IBAN = sheetData.GetCell("E6")?.GetString(sharedStrings)?.Trim(),
                BIC = sheetData.GetCell("B6")?.GetString(sharedStrings)?.Trim(),
            };

            // beneficiary
            var beneficiaryName = sheetData.GetCell("B3").GetString(sharedStrings)?.Trim();
            var beneficiaryAddressLine1 = sheetData.GetCell("B4").GetString(sharedStrings)?.Trim();
            var paymentPurpose = sheetData.GetCell("B5").GetString(sharedStrings)?.Trim();

            // due date
            var dueDateString = sheetData.GetCell("B7")?.GetString(sharedStrings)?.Trim();
            var dueDate = DateTime.Today;
            if (!string.IsNullOrEmpty(dueDateString))
            {
                try
                {
                    // ReSharper disable once InconsistentNaming
                    var dueOADate = XmlConvert.ToDouble(dueDateString);
                    dueDate = DateTime.FromOADate(dueOADate);
                }
                catch
                {
                    // nothing to do
                }
            }

            // return
            var paymentPurposeCapture = paymentPurpose;
            return (paymentPurpose, sheetData.OfType<Row>()
                .Where(r => r.RowIndex > 10)
                .OrderBy(r => r.RowIndex.Value)
                .Select(r => this.ReadSinglePaymentRow(r, sharedStrings, beneficiaryName, beneficiaryAddressLine1, account, paymentPurposeCapture, dueDate))
                // filter out invalid rows
                .Where(p => p != null)
                .ToList()
            );
        }

        /// <summary>
        /// Reads the single payment row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="sharedStrings">The shared strings.</param>
        /// <param name="beneficiaryName">Name of the beneficiary.</param>
        /// <param name="beneficiaryAddressLine1">The beneficiary address line1.</param>
        /// <param name="account">The account.</param>
        /// <param name="paymentPurpose">The payment purpose.</param>
        /// <param name="dueDate">The due date.</param>
        /// <returns></returns>
        private Pay ReadSinglePaymentRow(
            Row row, SharedStringTable sharedStrings, string beneficiaryName, string beneficiaryAddressLine1,
            BankAccount account, string paymentPurpose, DateTime dueDate)
        {
            // common values for all payments
            var payment = new Payment(account)
            {
                BeneficiaryName = beneficiaryName,
                BeneficiaryAddressLine1 = beneficiaryAddressLine1,
                PaymentDueDate = dueDate,
            };

            // payment note
            this.BuildPaymentNote(row, sharedStrings, paymentPurpose, payment);

            // symbols
            payment.SpecificSymbol = row.GetCell("A")?.GetString(sharedStrings)?.Trim();
            payment.ConstantSymbol = row.GetCell("B")?.GetString(sharedStrings)?.Trim();
            payment.VariableSymbol = row.GetCell("C")?.GetString(sharedStrings)?.Trim();

            // amount
            var amountString = row.GetCell("D")?.GetString(sharedStrings)?.Trim();
            if (!string.IsNullOrWhiteSpace(amountString))
            {
                if (Decimal.TryParse(amountString, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount))
                {
                    payment.Amount = amount;
                }
            }

            // check for valid input
            if (!payment.Amount.HasValue)
            {
                return null;
            }

            return new Pay(payment);
        }

        /// <summary>
        /// Biuilds the payment note.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="sharedStrings">The shared strings.</param>
        /// <param name="paymentPurpose">The payment purpose.</param>
        /// <param name="payment">The payment.</param>
        private void BuildPaymentNote(Row row, SharedStringTable sharedStrings, string paymentPurpose, Payment payment)
        {
            var paymentNote = row.GetCell("E")?.GetString(sharedStrings)?.Trim();

            if (!string.IsNullOrWhiteSpace(payment.BeneficiaryName))
                payment.PaymentNote = payment.BeneficiaryName;

            if (!string.IsNullOrWhiteSpace(paymentPurpose))
            {
                if (!string.IsNullOrEmpty(payment.PaymentNote))
                    payment.PaymentNote += ": ";
                payment.PaymentNote += paymentPurpose;
            }

            if (!string.IsNullOrWhiteSpace(paymentNote))
            {
                if (!string.IsNullOrEmpty(payment.PaymentNote))
                    payment.PaymentNote += " - ";
                payment.PaymentNote += paymentNote;
            }
        }

        /// <summary>
        /// Creates the output file.
        /// </summary>
        /// <param name="templateFilePath">The template file path.</param>
        /// <param name="outputFilePath">The file path </param>
        /// <param name="paymentPurpose">The payment purpose.</param>
        /// <param name="pays">The pays.</param>
        /// <returns></returns>
        public string CreateOutputFile(string templateFilePath, string outputFilePath, string paymentPurpose, IList<Pay> pays)
        {
            // output filename
            string tempFilename = outputFilePath;

            // copy template
            System.IO.File.Copy(templateFilePath, tempFilename, true);

            // open the temp file as document
            using (var document = DocumentFormat.OpenXml.Packaging.SpreadsheetDocument.Open(tempFilename, true))
            {
                // change from template type to workbook type
                document.ChangeDocumentType(SpreadsheetDocumentType.Workbook);

                // get global parts
                var workbookPart = document.WorkbookPart;
                var workbook = workbookPart.Workbook;

                // get hidden "template" sheet parts
                var tempSheet = workbook.Sheets.Elements<Sheet>().Skip(1).First();
                var tempWorksheetPart = (DocumentFormat.OpenXml.Packaging.WorksheetPart)workbookPart.GetPartById(tempSheet.Id);
                var tempSheetData = tempWorksheetPart.Worksheet.GetFirstChild<SheetData>();
                var tempMergeCells = tempWorksheetPart.Worksheet.Elements<MergeCells>().FirstOrDefault();
                var tempRows = tempSheetData.Elements<Row>().ToList();
                var tempMergedCells = tempMergeCells?.Elements<MergeCell>().ToList();

                // get the output "data" sheet parts
                var dataSheet = workbook.Sheets.Elements<Sheet>().First();
                var dataWorksheetPart = (DocumentFormat.OpenXml.Packaging.WorksheetPart)workbookPart.GetPartById(dataSheet.Id);
                var dataSheetData = dataWorksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();
                if (dataSheetData == null)
                {
                    dataSheetData = new SheetData();
                    dataWorksheetPart.Worksheet.InsertAfter(dataSheetData, dataWorksheetPart.Worksheet.Elements<Columns>().First());
                }

                var shouldAddMergeCells = false;
                var dataMergeCells = dataWorksheetPart.Worksheet.Elements<MergeCells>().FirstOrDefault();
                if (dataMergeCells == null)
                {
                    dataMergeCells = new MergeCells();
                    shouldAddMergeCells = true;
                }

                // process all pays
                var rowIndex = 0;
                for (var i = 0; i < pays.Count; i++)
                {
                    rowIndex += this.AddPay(pays[i], paymentPurpose, dataSheetData, dataMergeCells, rowIndex, tempRows, tempMergedCells);
                    this.ProgressReporter?.Report((int)(100.0M * i / pays.Count));
                }

                // empty MergeCells make the worksheet invalid 
                if (shouldAddMergeCells && (dataMergeCells.ChildElements.Count > 0))
                {
                    dataWorksheetPart.Worksheet.InsertAfter(dataMergeCells, dataSheetData);
                }

                // remove hidden sheet
                tempSheet.Remove();
                workbookPart.DeletePart(tempWorksheetPart);

                // save all changes
                dataWorksheetPart.Worksheet.Save();
                workbook.Save();
                document.Save();
            }

            // return the output filename
            return tempFilename;
        }

        /// <summary>
        /// Adds the pay.
        /// </summary>
        /// <param name="pay">The pay.</param>
        /// <param name="paymentPurpose">The payment purpose.</param>
        /// <param name="dataSheetData">The data sheet data.</param>
        /// <param name="mergeCells">The merge cells.</param>
        /// <param name="baseRowIndex">Index of the base row.</param>
        /// <param name="templateRows">The template rows.</param>
        /// <param name="templateMergedCells">The template merged cells.</param>
        /// <returns>Count of rows added.</returns>
        private int AddPay(
            Pay pay, string paymentPurpose, SheetData dataSheetData, MergeCells mergeCells, int baseRowIndex, 
            List<Row> templateRows, List<MergeCell> templateMergedCells
        )
        {
            // make a deep copy of template rows
            var newRows = templateRows.Select(r => (Row) r.CloneNode(true)).ToList();

            // make deep copy of merged cells
            var newMergedCells = templateMergedCells.Select(mc => (MergeCell) mc.CloneNode(true)).ToList();
            
            // fill in data
            this.FillInPayData(pay, paymentPurpose, newRows);

            // update cell references
            {
                // explicit lambda capture
                var index = baseRowIndex;

                // update cells
                newRows.ForEach(r => r.UpdateCellReferences(index));

                // update merged cells
                newMergedCells.ForEach(mc => mc.Reference.UpdateCellReference(index));
            }

            // append rows
            dataSheetData.Append(newRows);

            // append merged cells
            mergeCells.Append(newMergedCells);

            // insert QR code
            this.InsertQrCode(pay, dataSheetData, baseRowIndex);

            // return count of rows the baseRowIndex should be increased by
            return newRows.Count + 1;
        }

        /// <summary>
        /// Fills the in pay data.
        /// </summary>
        /// <param name="pay">The pay.</param>
        /// <param name="paymentPurpose">The payment purpose.</param>
        /// <param name="newRows">The new rows.</param>
        private void FillInPayData(Pay pay, string paymentPurpose, List<Row> newRows)
        {
            // payment purpose
            newRows[1].SetValue("C", paymentPurpose);

            // beneficiary & address
            newRows[2].SetValue("C", string.Join(
                $", {Environment.NewLine}",
                pay.Payments[0].BeneficiaryName?.Trim(),
                pay.Payments[0].BeneficiaryAddressLine1?.Trim()
            ));

            // bank account
            newRows[4].SetValue("E", pay.Payments[0].BankAccounts[0].IBAN);
            newRows[5].SetValue("E", pay.Payments[0].BankAccounts[0].BIC);

            // due date
            newRows[6].SetValue("E", pay.Payments[0].PaymentDueDate);

            // symbols
            newRows[4].SetValue("I", pay.Payments[0].ConstantSymbol);
            newRows[5].SetValue("I", pay.Payments[0].SpecificSymbol);
            newRows[6].SetValue("I", pay.Payments[0].VariableSymbol);

            // amount
            if (pay.Payments[0].Amount.HasValue)
            {
                var amount = pay.Payments[0].Amount.Value
                    .ToString("0.00", CultureInfo.InvariantCulture)
                    .Replace('.', ',')
                    + " " + pay.Payments[0].CurrencyCode;
                newRows[7].SetValue("I", amount);
            }

            // payment note
            newRows[8].SetValue("E", pay.Payments[0].PaymentNote);
        }

        /// <summary>
        /// Inserts the qr code.
        /// </summary>
        /// <param name="pay">The pay.</param>
        /// <param name="dataSheetData">The data sheet data.</param>
        /// <param name="baseRowIndex">Index of the base row.</param>
        private void InsertQrCode(Pay pay, SheetData dataSheetData, int baseRowIndex)
        {
            // real image size in cm
            const double realWidth = 5.98D;
            const double realHeight = 5.17D;

            // get the worksheet
            var worksheet = ((Worksheet) dataSheetData.Parent);

            // get the parts
            var worksheetPart = worksheet.WorksheetPart;
            var drawingsPart = worksheetPart.DrawingsPart ?? worksheetPart.AddNewPart<DocumentFormat.OpenXml.Packaging.DrawingsPart>();
            var imagePart = drawingsPart.AddImagePart(DocumentFormat.OpenXml.Packaging.ImagePartType.Png);

            // get the DrawingsPart/WorksheetDrawing
            var worksheetDrawing = drawingsPart.WorksheetDrawing;
            if (worksheetDrawing == null)
            {
                worksheetDrawing = drawingsPart.WorksheetDrawing = new DocumentFormat.OpenXml.Drawing.Spreadsheet.WorksheetDrawing();
            }

            // get the WorkSheet/Drawing
            var drawing = worksheet.Elements<Drawing>().FirstOrDefault();
            if (drawing == null)
            {
                drawing = new Drawing
                {
                    Id = worksheetPart.GetIdOfPart(drawingsPart),
                };
                worksheet.AppendChild(drawing);
            }

            // image size in EMUs
            int imageWidth;
            int imageHeight;

            // get the qr image and put it into image part
            var qrString = this.BySquareEncoder.Encode(pay);
            using (var qrBitmap = this.ImageManipulator.CreateQrCodeWithLogo(qrString))
            {
                var qrImageData = this.ImageManipulator.GetImageData(qrBitmap);
                using (var imageStream = new System.IO.MemoryStream(qrImageData))
                {
                    imagePart.FeedData(imageStream);
                }

                // determine image size in EMUs
                //http://en.wikipedia.org/wiki/English_Metric_Unit#DrawingML
                // var imageWidth  = (long) bm.Width  * (long) ((float) 914400 / bm.HorizontalResolution);
                // var imageHeight = (long) bm.Height * (long) ((float) 914400 / bm.VerticalResolution);
                imageWidth  = (int) (realWidth  * 360000);
                imageHeight = (int) (realHeight * 360000);
            }

            // create the Drawing.Spreadsheet.NonVisualDrawingProperties
            var nvpId = 1 + (worksheetDrawing
                .Descendants<DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualDrawingProperties>()
                .Select(p => p.Id.Value)
                .DefaultIfEmpty()
                .Max()
            );

            var nonVisualPictureProperties = new DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualPictureProperties
            {
                NonVisualDrawingProperties = new DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualDrawingProperties
                {
                    Id = nvpId,
                    Name = $"Picture {nvpId}",
                    Description = pay.Payments[0].PaymentNote,
                },
                NonVisualPictureDrawingProperties = new DocumentFormat.OpenXml.Drawing.Spreadsheet.NonVisualPictureDrawingProperties
                {
                    PictureLocks = new DocumentFormat.OpenXml.Drawing.PictureLocks
                    {
                        NoChangeAspect = true,
                        NoChangeArrowheads = true,
                    }
                },
            };

            // create the BlipFill
            var blipFill = new DocumentFormat.OpenXml.Drawing.Spreadsheet.BlipFill
            {
                Blip = new DocumentFormat.OpenXml.Drawing.Blip
                {
                    Embed = drawingsPart.GetIdOfPart(imagePart),
                    CompressionState = DocumentFormat.OpenXml.Drawing.BlipCompressionValues.Print,
                },
                SourceRectangle = new DocumentFormat.OpenXml.Drawing.SourceRectangle()
            };

            var stretch = new DocumentFormat.OpenXml.Drawing.Stretch
            {
                FillRectangle = new DocumentFormat.OpenXml.Drawing.FillRectangle()
            };

            blipFill.AppendChild(stretch);

            // create the ShapeProperties
            var shapeProperties = new DocumentFormat.OpenXml.Drawing.Spreadsheet.ShapeProperties
            {
                BlackWhiteMode = DocumentFormat.OpenXml.Drawing.BlackWhiteModeValues.Auto,
                Transform2D = new DocumentFormat.OpenXml.Drawing.Transform2D
                {
                    Offset = new DocumentFormat.OpenXml.Drawing.Offset
                    {
                        X = 0,
                        Y = 0,
                    },
                    Extents = new DocumentFormat.OpenXml.Drawing.Extents
                    {
                        Cx = imageWidth,
                        Cy = imageHeight,
                    },
                },
            };
            var prstGeom = new DocumentFormat.OpenXml.Drawing.PresetGeometry
            {
                Preset = DocumentFormat.OpenXml.Drawing.ShapeTypeValues.Rectangle,
                AdjustValueList = new DocumentFormat.OpenXml.Drawing.AdjustValueList(),
            };

            shapeProperties.AppendChild(prstGeom);
            shapeProperties.AppendChild(new DocumentFormat.OpenXml.Drawing.NoFill());

            // create the Picture
            var picture = new DocumentFormat.OpenXml.Drawing.Spreadsheet.Picture
            {
                NonVisualPictureProperties = nonVisualPictureProperties,
                BlipFill = blipFill,
                ShapeProperties = shapeProperties,
            };

            // create the OneCellAnchor
            var oneCellAnchor = new DocumentFormat.OpenXml.Drawing.Spreadsheet.OneCellAnchor
            {
                FromMarker = new DocumentFormat.OpenXml.Drawing.Spreadsheet.FromMarker
                {
                    // anchor it at 10th column
                    ColumnId = new DocumentFormat.OpenXml.Drawing.Spreadsheet.ColumnId(('K' - 'A').ToString()),
                    // anchor it at baseRowIndex row
                    RowId = new DocumentFormat.OpenXml.Drawing.Spreadsheet.RowId((baseRowIndex + 1).ToString()),
                    ColumnOffset = new DocumentFormat.OpenXml.Drawing.Spreadsheet.ColumnOffset("0"),
                    RowOffset = new DocumentFormat.OpenXml.Drawing.Spreadsheet.RowOffset("0"),
                },
                Extent = new DocumentFormat.OpenXml.Drawing.Spreadsheet.Extent
                {
                    Cx = imageWidth,
                    Cy = imageHeight,
                },
            };

            oneCellAnchor.AppendChild(picture);
            oneCellAnchor.AppendChild(new DocumentFormat.OpenXml.Drawing.Spreadsheet.ClientData());

            // add the anchored image to WorksheetDrawing
            worksheetDrawing.AppendChild(oneCellAnchor);
        }
    }
}
