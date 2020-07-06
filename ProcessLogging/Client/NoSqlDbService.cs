using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Crossroads.Service.Finance.Services.DbClients
{
    public class NoSqlDbService : INoSqlDbService
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly MongoClient _mongoClient;

        // note - this will have to be refactored if we at some point push data to more than
        // one collection or db via this service...in the meantime, YAGNI...
        private string _dbName;
        private string _collectionName;
        private IMongoDatabase _mongoDatabase;
        private IMongoCollection<BsonDocument> _mongoCollection;

        public NoSqlDbService(
            MongoClient mongoClient)
        {

            _dbName = Environment.GetEnvironmentVariable("NSQL_DBNAME");
            _collectionName = Environment.GetEnvironmentVariable("NSQL_COLLECTION");
            _mongoClient = mongoClient;
            InitializeDatabaseConnection();
        }

        private void InitializeDatabaseConnection()
        {
            try
            {
                _mongoDatabase = _mongoClient.GetDatabase(_dbName);
                _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>(_collectionName);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception connecting to mongo db: {ex.Message}");
            }
        }

        public async Task AddItemAsync(BsonDocument bsonDocument)
        {
            try
            {
                await _mongoCollection.InsertOneAsync(bsonDocument);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error sending data to mongo db: {ex.Message}");
            }
        }
    }
}