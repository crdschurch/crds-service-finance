using System;
using MongoDB.Driver;

namespace Crossroads.Service.Finance.Services.DbClients
{

    public class CosmosDbService : ICosmosDbService
    {
        public readonly MongoClient _mongoClient;

        public CosmosDbService(
            MongoClient mongoClient,
            string databaseName)
        {
            _mongoClient = mongoClient;
        }

        // TODO: test code, remove later
        public MongoClient GetMongoClient()
        {
            return _mongoClient;
        }

        //public async Task<List<string>> DisplayDatabaseNames()
        //{
        //    var dbNames = new List<string>();

        //    try
        //    {
        //        using (var cursor = await _mongoClient.ListDatabasesAsync())
        //        {
        //            await cursor.ForEachAsync(document => dbNames.Add(document.ToString()));
        //        }

        //        return dbNames;
        //    }
        //    catch(Exception ex)
        //    {
        //        //Write your own code here to handle exceptions
        //    }

        //    return null;
        //}

        // TODO: Implement these in the next story
        //public async Task AddItemAsync(Item item)
        //{
        //    await _mongoClient.CreateItemAsync<Item>(item, new PartitionKey(item.Id));
        //}

        //public async Task DeleteItemAsync(string id)
        //{
        //    await _mongoClient.DeleteItemAsync<Item>(id, new PartitionKey(id));
        //}

        //public async Task<Item> GetItemAsync(string id)
        //{
        //    try
        //    {
        //        ItemResponse<Item> response = await _mongoClient.ReadItemAsync<Item>(id, new PartitionKey(id));
        //        return response.Resource;
        //    }
        //    catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        //    {
        //        return null;
        //    }

        //}

        //public async Task<IEnumerable<Item>> GetItemsAsync(string queryString)
        //{
        //    var query = _mongoClient.GetItemQueryIterator<Item>(new QueryDefinition(queryString));
        //    List<Item> results = new List<Item>();
        //    while (query.HasMoreResults)
        //    {
        //        var response = await query.ReadNextAsync();

        //        results.AddRange(response.ToList());
        //    }

        //    return results;
        //}

        //public async Task UpdateItemAsync(string id, Item item)
        //{
        //    await _mongoClient.UpsertItemAsync<Item>(item, new PartitionKey(id));
        //}
    }
}