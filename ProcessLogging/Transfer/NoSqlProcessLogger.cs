using Crossroads.Service.Finance.Services.DbClients;
using MongoDB.Bson;
using ProcessLogging.Models;

namespace ProcessLogging.Transfer
{
    public class NoSqlProcessLogger : IProcessLogger
    {
        private readonly INoSqlDbService _cosmosDbService;

        public NoSqlProcessLogger(INoSqlDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public void SaveProcessLogMessage(ProcessLogMessage processLogMessage)
        {
            var processLogMessageBson = processLogMessage.ToBsonDocument();
            _cosmosDbService.AddItemAsync(processLogMessageBson);
        }
    }
}
