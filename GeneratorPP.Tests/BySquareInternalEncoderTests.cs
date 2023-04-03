using GeneratorPP.Core.Implementation;
using Digital.Slovensko.Ekosystem.GeneratorPP.Models;
using GeneratorPP.Core.Models.BySquare;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digital.Slovensko.Ekosystem.GeneratorPP.Tests
{
    [TestClass]
    public class BySquareInternalEncoderTests
    {
        [TestMethod]
        public void Encode_ShouldReturnTheSameOutputAsExternalEncoder()
        {
            // arrange
            var configuration = new ExternalGeneratorSettings();
            var sampleDocument = this.CreateSampleBySquareXmlDocuments(configuration);

            // The externalEncoded string was obtained from original BySquare implementation:
            // configuration.Url = "https://app.bysquare.com/api/generateStringCodes";
            // configuration.Username = "***";
            // configuration.Password = "***";
            // var externalEncoder = new BySquareExternalEncoder(configuration, null);
            // var externalEncoded = externalEncoder.Encode(sampleDocument.Documents[0]);
            const string externalEncoded = 
                "0007600054C8J2HVP2SVF56AV7DU85T2V2LRVDIAISK7DQTELQ0KET9NVS0ERS41BEJS3LFKRPS29E1LTH" +
                "G8QC9FCBUD413RVV83BFMBHOSJO74L0H6D3717FPLP8AP5VGS83I2CUQSOL3H000";

            // act
            var internalEncoded = new BySquareInternalEncoder().Encode(sampleDocument.Documents[0]);

            // assert
            Assert.AreEqual(externalEncoded, internalEncoded);
        }

        /// <summary>
        /// Creates the sample by square XML document.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        private BySquareXmlDocuments CreateSampleBySquareXmlDocuments(ExternalGeneratorSettings configuration)
        {
            var account1 = new BankAccount
            {
                IBAN = "SK2911000000005902208743",
                BIC = "TATRSKBX",
            };

            var account2 = new BankAccount
            {
                IBAN = "SK2011000000002619521810",
                BIC = "TATRSKBX",
            };

            var payment = new Payment(account1, account2)
            {
                Amount = 10,
                PaymentNote = "poznamka",
                OriginatorsReferenceInformation = "reference",
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
    }
}
