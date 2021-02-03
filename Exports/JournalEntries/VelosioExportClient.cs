using Crossroads.Web.Common.Configuration;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using VelosioJournalExport;

namespace Exports.JournalEntries
{
    // see https://medium.com/grensesnittet/integrating-with-soap-web-services-in-net-core-adebfad173fb for soap connection info
    public class VelosioExportClient : IJournalEntryExport
    {
        private readonly IConfigurationWrapper _configurationWrapper;
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public VelosioExportClient(IConfigurationWrapper configurationWrapper)
        {
            _configurationWrapper = configurationWrapper;
        }

        public async Task<string> HelloWorld()
        {
            var hw = new VelosioJournalExport.HelloWorldRequest();
            var config = new SendGLBatchSoapClient.EndpointConfiguration();
            var client = new SendGLBatchSoapClient(config, _configurationWrapper);
            var result = await client.HelloWorldAsync();
            return result.Body.HelloWorldResult;
        }

        public async Task<string> ExportJournalEntryStage(string batchNumber, decimal totalDebits, decimal totalCredits, int transactionCount, XElement batchData)
        {
            var config = new SendGLBatchSoapClient.EndpointConfiguration();
            var client = new SendGLBatchSoapClient(config, _configurationWrapper);

            var token = Environment.GetEnvironmentVariable("EXPORT_SERVICE_KEY");

            LoadBatchResponse result;

            try
            {
                result = await client.LoadBatchAsync(token,
                                                     batchNumber,
                                                     totalDebits,
                                                     totalCredits,
                                                     transactionCount,
                                                     batchData.ToString());

                _logger.Debug("Velosio export result: " + result.Body.LoadBatchResult);
                _logger.Debug("token: " + token);
                _logger.Debug("batchNumber: " + batchNumber);
                _logger.Debug("totalDebits: " + totalDebits);
                _logger.Debug("totalCredits: " + totalCredits);
                _logger.Debug("transactionCount: " + transactionCount);
                _logger.Debug("batchData.ToString(): " + batchData.ToString());
                _logger.Debug("CLIENT ENDPOINT: " + client.Endpoint.Address);
                _logger.Debug("CONFIG AS STRING: " + config.ToString());

                _logger.Info($"The result of the velosio export call was: {result.Body.LoadBatchResult}");
            }
            catch (Exception ex) {
                _logger.Error(ex, $"An exception occurred trying to send batch data to Velosio: {ex.Message}");
                result = null;
            }

            return result.Body.LoadBatchResult;
        }
    }
}