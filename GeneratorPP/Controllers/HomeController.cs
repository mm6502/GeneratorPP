using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Digital.Slovensko.Ekosystem.GeneratorPP.Implementation;
using Digital.Slovensko.Ekosystem.GeneratorPP.Models;
using Digital.Slovensko.Ekosystem.GeneratorPP.Models.BySquare;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Digital.Slovensko.Ekosystem.GeneratorPP.Controllers
{
    /// <summary>
    /// The default controller.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    public class HomeController : Controller
    {
        #region Properties

        /// <summary>
        /// Gets the environment.
        /// </summary>
        private IHostingEnvironment Environment { get; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        private ILogger<HomeController> Logger { get; }

        /// <summary>
        /// Gets the application settings.
        /// </summary>
        private AppSettings AppSettings { get; }

        /// <summary>
        /// Gets the encoder.
        /// </summary>
        private IBySquareEncoder Encoder { get; }

        #endregion Properties

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="settings">The settings.</param>
        public HomeController(
            IHostingEnvironment environment,
            ILogger<HomeController> logger,
            IOptions<AppSettings> settings
        )
        {
            this.Environment = environment;
            this.Logger = logger;
            this.AppSettings = settings.Value;

            if (string.Equals("external", this.AppSettings?.UseGenerator, StringComparison.OrdinalIgnoreCase))
            {
                this.Encoder = new BySquareExternalEncoder(this.AppSettings?.ExternalGenerator, this.Logger);
            }
            else
            {
                this.Encoder = new BySquareInternalEncoder();
            }
        }

        #endregion Initialization

        #region Actions

        /// <summary>
        /// The Index action.
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// The Error action.
        /// </summary>
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// The Finish action.
        /// </summary>
        public IActionResult Finish()
        {
            return View();
        }

        /// <summary>
        /// Stores the uploaded file under a random name in temp folder.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UploadFile(IFormFile inputFile)
        {
            var data = new ProcessingModel
            {
                RequestId = Guid.NewGuid().ToString("D")
            };

            // copy the input file to temp
            var filePath = HomeController.GetTempInputFilePath(data.RequestId);
            using (var file = System.IO.File.OpenWrite(filePath))
            {
                inputFile.OpenReadStream().CopyTo(file);
            }

            // stores the progress
            this.StoreGenerationProgress(data);

            // redirects to file processing
            return RedirectToAction(nameof(ProcessFile), new { id = data.RequestId });
        }

        /// <summary>
        /// Displays the progress. The generation is run by a jquery.fileDownload in an iframe.
        /// </summary>
        /// <param name="id">The request id.</param>
        [HttpGet]
        public IActionResult ProcessFile(string id)
        {
            var data = this.RetrieveGenerationProgress(id);
            ViewData["BaseUrl"] = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            ViewData["RequestID"] = data.RequestId;
            return View("InProgress");
        }

        /// <summary>
        /// Generates a download for specified identifier.
        /// </summary>
        /// <param name="id">The request identifier (name of the uploaded file in temp folder).</param>
        [HttpGet]
        public async Task<IActionResult> Generate(string id)
        {
            var data = this.RetrieveGenerationProgress(id);

            // create the tasks
            var generatorTask = Task.Factory.StartNew(() => this.GenerateOutputFile(data), TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
            var clearingTask = Task.Factory.StartNew(this.CleanTemp);

            // wait for completion
            // (asp.net core does not have a synchronization context, no need for ConfigureAwait)
            // ReSharper disable once ConsiderUsingConfigureAwait
            await Task.WhenAll(generatorTask, clearingTask);

            // get the resulting file name
            var outputFilePath = generatorTask.Result;

            var downFileName = $"platobne predpisy {DateTime.Now.ToString("s").Replace(":", "").Replace("T", " ")}.xlsx";
            var downStream = System.IO.File.OpenRead(outputFilePath);

            // signal the download is ready (consumed by jquery-fileDownload)
            this.Response.Cookies.Append("fileDownload", "true", new CookieOptions {Path = "/", HttpOnly = false});
            
            this.HttpContext.Session.Remove(id);
            // ReSharper disable once ConsiderUsingConfigureAwait
            await this.HttpContext.Session.CommitAsync();

            // send the file
            this.Response.Headers.Add("Content-Disposition", new[] {$"attachment; filename=\"{downFileName}\""});
            return new FileStreamResult(downStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        /// <summary>
        /// Gets the progress.
        /// </summary>
        /// <param name="id">The identifier.</param>
        [HttpGet]
        public JsonResult GetProgress(string id)
        {
            var data = this.RetrieveGenerationProgress(id);
            return new JsonResult(data.Percent);
        }

        #endregion Actions

        #region Helpers

        /// <summary>
        /// Retrieves the download progress.
        /// </summary>
        /// <param name="id">The request identifier.</param>
        public ProcessingModel RetrieveGenerationProgress(string id)
        {
            try
            {
                var serialized = this.HttpContext.Session.GetString(id);
                return JsonConvert.DeserializeObject<ProcessingModel>(serialized);
            }
            catch
            {
                // nothing to do
            }

            return new ProcessingModel { RequestId = id };
        }

        /// <summary>
        /// Stores the download progress.
        /// </summary>
        /// <param name="data">The data.</param>
        public void StoreGenerationProgress(ProcessingModel data)
        {
            this.HttpContext?.Session?.SetString(data.RequestId, JsonConvert.SerializeObject(data));
            this.HttpContext?.Session?.CommitAsync().Wait();
        }

        /// <summary>
        /// Gets the temporary file path.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="flavor">The desired file flavor. Used to differentiate between input and output files.</param>
        public static string GetTempFilePath(string id, string flavor)
        {
            return Path.Combine(Path.GetTempPath(), $"{id}_{flavor}.xlsx");
        }

        /// <summary>
        /// Gets the temporary input file path.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public static string GetTempInputFilePath(string id)
        {
            return HomeController.GetTempFilePath(id, "in");
        }

        /// <summary>
        /// Gets the temporary output file path.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public static string GetTempOutputFilePath(string id)
        {
            return HomeController.GetTempFilePath(id, "out");
        }

        /// <summary>
        /// Generates the output file.
        /// </summary>
        /// <param name="data">The processing data.</param>
        /// <returns></returns>
        private string GenerateOutputFile(ProcessingModel data)
        {
            var progressReporter = new Progress<int>();
            progressReporter.ProgressChanged += (sender, percent) =>
            {
                data.Percent = percent;
                this.StoreGenerationProgress(data);
            };

            var serializer = this.Encoder;
            var imageManipulator = new ImageManipulator(this.Environment);
            var excelManipulator = new ExcelManipulator(serializer, imageManipulator, progressReporter);
            var sourceFilePath = HomeController.GetTempInputFilePath(data.RequestId);
            var templateFilePath = Path.Combine(this.Environment.WebRootPath, "platobne_predpisy.xltx");
            var outputFilePath = HomeController.GetTempOutputFilePath(data.RequestId);

            // another request tries to download the same file?
            if (System.IO.File.Exists(outputFilePath))
            {
                // wait for the other thread to complete
                while (true)
                {
                    try
                    {
                        using (System.IO.File.OpenWrite(outputFilePath))
                        {
                            break;
                        }
                    }
                    catch
                    {
                        // wait a while
                        Thread.Sleep(5000);
                    }
                }
                return outputFilePath;
            }
            
            // read source data
            (string paymentPurpose, var bySquareDocuments) = excelManipulator.ReadPaymentList(sourceFilePath);

            // create the output
            return excelManipulator.CreateOutputFile(templateFilePath, outputFilePath, paymentPurpose, bySquareDocuments);
        }

        /// <summary>
        /// Cleans the temporary folder.
        /// </summary>
        private void CleanTemp()
        {
            var tempPath = Path.GetTempPath();
            var counter = 100;

            foreach (var file in Directory.EnumerateFiles(tempPath, "*.xlsx"))
            {
                try
                {
                    var fileInfo = new FileInfo(file);
                    var tooOld = DateTime.UtcNow.AddHours(-1);

                    if ((fileInfo.LastWriteTimeUtc > tooOld) && (fileInfo.CreationTimeUtc > tooOld))
                        continue;

                    fileInfo.Delete();

                    if (--counter <= 0)
                        return;
                }
                catch
                {
                    // nothing to do
                }
            }
        }

        #endregion Helpers

        #region Development Stuff

#if DEBUG

        /// <summary>
        /// Generates a sample <see cref="Pay"/> document and displays it.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Sample()
        {
            // create object
            var bySquareXmlDocuments = this.CreateSampleBySquareXmlDocument(this.AppSettings.ExternalGenerator);

            // serialize object
            ViewData["XML"] = new BySquareXmlSerializer().SerializeAsXml(bySquareXmlDocuments);

            // generate QR code
            var qrstring = this.Encoder.Encode(bySquareXmlDocuments.Documents[0]);

            // create final image
            var image = new ImageManipulator(this.Environment).CreateQrCodeWithLogo(qrstring);
            ViewData["IMAGE"] = new ImageManipulator(this.Environment).EncodeImageAsBase64String(image);

            // render object
            return View();
        }

        /// <summary>
        /// Reads the sample file, displays the first <see cref="Pay"/> document.
        /// </summary>
        [HttpPost]
        public IActionResult SampleFile()
        {
            var serializer = new BySquareXmlSerializer();
            var encoder = new BySquareInternalEncoder();
            var imageManipulator = new ImageManipulator(this.Environment);
            var excelManipulator = new ExcelManipulator(encoder, imageManipulator, null);

            var filePath = Path.Combine(this.Environment.ContentRootPath, "Samples", "platobny_harok.xlsx");
            (var dummy, var bySquareDocuments) = excelManipulator.ReadPaymentList(filePath);
            ViewData["XML"] = serializer.SerializeAsXml(bySquareDocuments[0]);
            
            // generate QR code
            var qrstring = this.Encoder.Encode(bySquareDocuments[0]);
            var image = imageManipulator.CreateQrCodeWithLogo(qrstring);
            ViewData["IMAGE"] = imageManipulator.EncodeImageAsBase64String(image);

            return View("Sample");
        }

        /// <summary>
        /// Creates the sample by square XML document.
        /// </summary>
        /// <param name="configuration">The configuration for external generator (<see cref="BySquareExternalEncoder"/>).</param>
        private BySquareXmlDocuments CreateSampleBySquareXmlDocument(ExternalGeneratorSettings configuration)
        {
            var account1 = new BankAccount
            {
                IBAN = "SK2911000000005902208743",
                BIC = "TATRSKBX"
            };

            var account2 = new BankAccount
            {
                IBAN = "SK2011000000002619521810",
                BIC = "TATRSKBX"
            };

            var payment = new Payment(account1, account2)
            {
                Amount = 10,
                PaymentNote = "poznamka",
                OriginatorsReferenceInformation = "reference"
            };
           
            var pay = new Pay(payment);

            var bySquareXmlDocuments = new BySquareXmlDocuments(pay)
            {
                Username = configuration.Username,
                Password = configuration.Password,
                ServiceId = configuration.ServiceId,
                ServiceUserId = configuration.ServiceUserId,
            };

            return bySquareXmlDocuments;
        }
#endif

        #endregion Development Stuff
    }
}

