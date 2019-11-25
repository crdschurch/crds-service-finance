using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Crossroads.Web.Common.Configuration;
using Exports.Models;
using VelosioJournalExport;

namespace Exports.JournalEntries
{
    // see https://medium.com/grensesnittet/integrating-with-soap-web-services-in-net-core-adebfad173fb for soap connection info
    public class VelosioExportClient : IJournalEntryExport
    {
        private readonly IConfigurationWrapper _configurationWrapper;

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

            var result = await client.LoadBatchAsync(
                token,
                batchNumber,
                totalDebits,
                totalCredits,
                transactionCount,
                batchData.ToString());

            return result.Body.LoadBatchResult;
        }
    }
}
