using System;
using System.Net.Http;
using System.Text;
using Digital.Slovensko.Ekosystem.GeneratorPP.Models;
using GeneratorPP.Core.Implementation;
using GeneratorPP.Core.Models.BySquare;
using Microsoft.Extensions.Logging;

namespace Digital.Slovensko.Ekosystem.GeneratorPP.Implementation
{
    /// <summary>
    /// QR string encoder which utilizes the bysquare.com web API.
    /// </summary>
    /// <seealso cref="IBySquareEncoder" />
    public class BySquareExternalEncoder : IBySquareEncoder
    {
        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        private ExternalGeneratorSettings Configuration { get; }

        /// <summary>
        /// Gets or sets the serializer.
        /// </summary>
        /// <value>
        /// The serializer.
        /// </value>
        private IBySquareXmlSerializer Serializer { get; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        private ILogger Logger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BySquareExternalEncoder"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public BySquareExternalEncoder(ExternalGeneratorSettings configuration, ILogger logger)
        {
            this.Configuration = configuration;
            this.Logger = logger;
            this.Serializer = new BySquareXmlSerializer();
        }

        /// <summary>
        /// Serializes given document as a QR string.
        /// </summary>
        /// <param name="document">A BySquareDocument instance.</param>
        /// <returns>
        /// QR string.
        /// </returns>
        /// <remarks>
        /// Sample response:
        /// <StringSetOfCodes>
        /// <PayBySquare>00048000BEH98QN092PSOB1EKLG0V9OQ0L0QAK2M303J09BRUEOSHJHRN8UH0JUEAVQLBQQ9L88MHK264TTJLOHE056AM3MCD3PB600</PayBySquare>
        /// <ItemsBySquare />
        /// </StringSetOfCodes>
        /// </remarks>
        public string Encode(BySquareDocument document)
        {
            // wrap in BySquareXmlDocuments object
            var url = this.Configuration.Url;
            var documents = new BySquareXmlDocuments(document)
            {
                Username = this.Configuration.Username,
                Password = this.Configuration.Password,
                ServiceId = this.Configuration.ServiceId,
                ServiceUserId = this.Configuration.ServiceUserId,
            };

            // send to bysquare.com web api generator
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(this.Serializer.SerializeAsXml(documents), Encoding.UTF8, "text/xml")
            };

            try
            {
                using var client = new HttpClient();
                var response = client.SendAsync(request, HttpCompletionOption.ResponseContentRead).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;
                var setOfCodes = this.Serializer.DeserializeSetOfCodes(responseString);
                return setOfCodes.PayBySquare;
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "bysquare.com Web API error");
                return null;
            }
        }
    }
}
