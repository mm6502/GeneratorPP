using System;
using System.Linq;
using System.Xml;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;

namespace GeneratorPP.Core.Implementation
{
    /// <summary>
    /// Helper functions for manipulating excel files.
    /// </summary>
    public static class ExcelExtensions
    {
        /// <summary>
        /// Gets the row.
        /// </summary>
        /// <param name="sheetData">The sheet data.</param>
        /// <param name="cellReference">The cell reference.</param>
        /// <param name="create">if set to <c>true</c> [create].</param>
        /// <returns></returns>
        public static Row? GetRow(this SheetData sheetData, string cellReference, bool create = false)
        {
            var row = sheetData
                .Elements<Row>()
                .Where(r => r.RowIndex is not null)
                .FirstOrDefault(r => cellReference.EndsWith(r.RowIndex!.Value.ToString()));

            if (row != null) 
                return row;

            if (!create)
                return null;

            row = new Row();
            sheetData.AppendChild(row);

            return row;
        }

        /// <summary>
        /// Gets the row.
        /// </summary>
        /// <param name="sheetData">The sheet data.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="create">if set to <c>true</c> [create].</param>
        /// <returns></returns>
        public static Row? GetRow(this SheetData sheetData, int rowIndex, bool create = false)
        {
            var row = sheetData
                .Elements<Row>()
                .Where(r => r.RowIndex is not null)
                .FirstOrDefault(r => r.RowIndex!.Value == rowIndex);

            if (row != null) 
                return row;

            if (!create)
                return null;

            row = new Row();
            sheetData.AppendChild(row);

            return row;
        }

        /// <summary>
        /// Gets the cell.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="create">if set to <c>true</c> [create].</param>
        /// <returns></returns>
        public static Cell? GetCell(this Row row, string columnName, bool create = false)
        {
            var cell = row
                .Elements<Cell>()
                .FirstOrDefault(c => c.CellReference?.Value?.StartsWith(columnName) ?? false);

            if (cell != null) 
                return cell;

            if (!create)
                return null;

            cell = new Cell();
            row.AppendChild(cell);

            return cell;
        }

        /// <summary>
        /// Gets the cell.
        /// </summary>
        /// <param name="sheetData">The sheet data.</param>
        /// <param name="cellReference">The cell reference.</param>
        /// <param name="create">if set to <c>true</c> [create].</param>
        /// <returns></returns>
        public static Cell? GetCell(this SheetData sheetData, string cellReference, bool create = false)
        {
            var row = sheetData.GetRow(cellReference, create);
            if (row == null)
                return null;

            var cell = row
                .Elements<Cell>()
                .FirstOrDefault(c => c.CellReference == cellReference);

            if (cell != null) 
                return cell;

            if (!create)
                return null;

            cell = new Cell();
            row.AppendChild(cell);

            return cell;
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="sharedStringTable">The shared string table.</param>
        /// <returns></returns>
        public static string? GetString(this Cell? cell, SharedStringTable? sharedStringTable)
        {
            var value = cell?.CellValue?.Text;

            if (value == null)
                return null;

            if ((sharedStringTable != null) && (cell?.DataType?.Value == CellValues.SharedString))
                value = sharedStringTable
                    .OfType<SharedStringItem>()
                    .Skip(Convert.ToInt32(value))
                    .FirstOrDefault()?
                    .InnerText;

            return value;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sheetData">The sheet data.</param>
        /// <param name="cellReference">The cell reference.</param>
        /// <param name="value">The value.</param>
        public static void SetValue<T>(this SheetData sheetData, string cellReference, T value)
        {
            sheetData.GetCell(cellReference, true)?.SetValue(value);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="value">The value.</param>
        public static void SetValue<T>(this Row row, string columnName, T value)
        {
            row.GetCell(columnName, true).SetValue(value);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cell">The cell.</param>
        /// <param name="value">The value.</param>
        public static void SetValue<T>(this Cell? cell, T value)
        {
            if (cell == null)
                return;

            switch (value)
            {
                case string s:
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(s);
                    return;
                case DateTime d:
                    cell.DataType = CellValues.Number;
                    cell.CellValue = new CellValue(XmlConvert.ToString(d.ToOADate()));
                    return;
                case bool b:
                    cell.DataType = CellValues.Boolean;
                    cell.CellValue = new CellValue(XmlConvert.ToString(b));
                    return;
                case sbyte n:
                    cell.DataType = CellValues.Number;
                    cell.CellValue = new CellValue(XmlConvert.ToString(n));
                    return;
                case byte n:
                    cell.DataType = CellValues.Number;
                    cell.CellValue = new CellValue(XmlConvert.ToString(n));
                    return;
                case short n:
                    cell.DataType = CellValues.Number;
                    cell.CellValue = new CellValue(XmlConvert.ToString(n));
                    return;
                case ushort n:
                    cell.DataType = CellValues.Number;
                    cell.CellValue = new CellValue(XmlConvert.ToString(n));
                    return;
                case int n:
                    cell.DataType = CellValues.Number;
                    cell.CellValue = new CellValue(XmlConvert.ToString(n));
                    return;
                case uint n:
                    cell.DataType = CellValues.Number;
                    cell.CellValue = new CellValue(XmlConvert.ToString(n));
                    return;
                case long n:
                    cell.DataType = CellValues.Number;
                    cell.CellValue = new CellValue(XmlConvert.ToString(n));
                    return;
                case ulong n:
                    cell.DataType = CellValues.Number;
                    cell.CellValue = new CellValue(XmlConvert.ToString(n));
                    return;
                case float n:
                    cell.DataType = CellValues.Number;
                    cell.CellValue = new CellValue(XmlConvert.ToString(n));
                    return;
                case double n:
                    cell.DataType = CellValues.Number;
                    cell.CellValue = new CellValue(XmlConvert.ToString(n));
                    return;
                case decimal n:
                    cell.DataType = CellValues.Number;
                    cell.CellValue = new CellValue(XmlConvert.ToString(n));
                    return;
            }

            cell.DataType = CellValues.String;
            cell.CellValue = new CellValue(Convert.ToString(value) ?? string.Empty);
        }

        /// <summary>
        /// Updates the cell references.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="rowShift">The row shift.</param>
        public static void UpdateCellReferences(this Row row, int rowShift)
        {
            row.RowIndex!.Value = (uint) (row.RowIndex.Value + rowShift);
            foreach (var cell in row.Elements<Cell>())
            {
                cell.UpdateCellReference(rowShift);
            }
        }

        /// <summary>
        /// Updates the cell reference.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="rowShift">The row shift.</param>
        public static void UpdateCellReference(this Cell cell, int rowShift)
        {
            cell.CellReference.UpdateCellReference(rowShift);
        }

        /// <summary>
        /// Updates the cell reference.
        /// </summary>
        /// <param name="cellReference">The cell reference.</param>
        /// <param name="rowShift">The row shift.</param>
        public static void UpdateCellReference(this StringValue? cellReference, int rowShift)
        {
            if (cellReference is not { HasValue: true })
                return;
            var references = cellReference.Value!.Split(":");
            var updatedReferences = references.Select(r => UpdateCellReferenceString(r, rowShift));
            cellReference.Value = string.Join(":", updatedReferences);
        }

        /// <summary>
        /// Updates the cell reference string.
        /// </summary>
        /// <param name="cellReference">The cell reference.</param>
        /// <param name="rowShift">The row shift.</param>
        /// <returns></returns>
        private static string UpdateCellReferenceString(string cellReference, int rowShift)
        {
            if (string.IsNullOrEmpty(cellReference))
                return cellReference;
            var index = cellReference.IndexOfAny("0123456789".ToCharArray());
            var column = cellReference[..index];
            var row = cellReference[index..];
            var rowIndex = Convert.ToInt32(row) + rowShift;
            return column + rowIndex;
        }
    }
}