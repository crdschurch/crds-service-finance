using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using ProcessLogging.Transfer;

namespace Crossroads.Service.Finance.Services.DbClients
{
    public interface ICosmosDbService
    {
        MongoClient GetMongoClient();

        //Task<IEnumerable<Item>> GetItemsAsync(string query);
        //Task<Item> GetItemAsync(string id);
        //Task AddItemAsync(Item item);
        //Task UpdateItemAsync(string id, Item item);
        //Task DeleteItemAsync(string id);
    }
}
