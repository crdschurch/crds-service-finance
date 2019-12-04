using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Crossroads.Web.Common.Configuration;
using Exports.Models;
using log4net;
using VelosioJournalExport;

namespace Exports.JournalEntries
{
    // see https://medium.com/grensesnittet/integrating-with-soap-web-services-in-net-core-adebfad173fb for soap connection info
    public class VelosioExportClient : IJournalEntryExport
    {
        private readonly IConfigurationWrapper _configurationWrapper;
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public VelosioExportClient(IConfigurationWrapper configurationWrapper)
        {
            _configurationWrapper = configurationWrapper;
        }

        public async Task<string> HelloWorld()
        {
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
                Console.WriteLine($"Velosio export result: {result.Body.LoadBatchResult}" );
                _logger.Info($"Velosio export result: {result.Body.LoadBatchResult}");
            }
            catch (Exception exc) {
                Console.WriteLine("Velosio export error result: " + exc);
                _logger.Error("An exception occurred trying to send batch data", exc);
                result = null;
            }

            return result.Body.LoadBatchResult;
        }
    }
}
