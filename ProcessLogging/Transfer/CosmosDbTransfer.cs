using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossroads.Service.Finance.Services.DbClients;
using Microsoft.Azure.Cosmos;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace ProcessLogging.Transfer
{
    public class CosmosDbTransfer : ITransferData
    {
        private readonly ICosmosDbService _cosmosDbService;

        public CosmosDbTransfer(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        // TODO: Test method, remove at later date (normally, we don't directly use the client but use a wrapper)
        public async Task<List<string>> DisplayDatabaseNames()
        {
            var dbNames = new List<string>();

            try
            {
                using (var cursor = await _cosmosDbService.GetMongoClient().ListDatabasesAsync())
                {
                    await cursor.ForEachAsync(document => dbNames.Add(document.ToString()));
                }

                return dbNames;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }
    }
}
