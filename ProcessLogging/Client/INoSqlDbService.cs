using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using ProcessLogging.Transfer;

namespace Crossroads.Service.Finance.Services.DbClients
{
    public interface INoSqlDbService
    {
        Task AddItemAsync(BsonDocument bsonDocument);
    }
}
