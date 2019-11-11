using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
            var hw = new VelosioJournalExport.HelloWorldRequest();
            //var result = VelosioJournalExport.SendGLBatchSoapClient.;
            var config = new SendGLBatchSoapClient.EndpointConfiguration();
            var client = new VelosioJournalExport.SendGLBatchSoapClient(config, _configurationWrapper);
            var result = await client.HelloWorldAsync();
            return result.Body.HelloWorldResult;
        }

        public async Task<string> ExportJournalEntryStage(VelosioJournalEntryBatch velosioJournalEntryStage)
        {
            //var body = new LoadBatchRequestBody();
            //var req = new VelosioJournalExport.LoadBatchRequest();
            var config = new SendGLBatchSoapClient.EndpointConfiguration();
            var client = new VelosioJournalExport.SendGLBatchSoapClient(config, _configurationWrapper);
            //client.ClientCredentials.UserName = "token";
            var result = await client.LoadBatchAsync("token", velosioJournalEntryStage.BatchNumber, 
                velosioJournalEntryStage.TotalDebits, velosioJournalEntryStage.TotalCredits, velosioJournalEntryStage.TransactionCount, 
                velosioJournalEntryStage.BatchData.ToString());
            return result.Body.LoadBatchResult;
        }
    }
}
