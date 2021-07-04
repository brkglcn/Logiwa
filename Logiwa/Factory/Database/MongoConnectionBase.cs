using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Logiwa.Factory.Database
{
    public abstract class MongoConnectionBase : IDisposable
    {
        private IMongoClient _client { get; set; }
        public IMongoDatabase Database { get; private set; }

        public delegate void LogError(Exception exception, string message, params object[] args);

        public MongoConnectionBase(string serverName, int port, string collectionName)
        {
            _client = new MongoClient(new MongoClientSettings { Server = new MongoServerAddress(serverName, port) });
            Database = _client.GetDatabase(collectionName);
        }

        public interface IMongoEntity
        {
            ObjectId Id { get; set; }
        }

        const string _categoryName = "MongoConnectionBase";
        public IMongoCollection<T> GetCollection<T>(string collectionName, LogError logger) where T : IMongoEntity
        {
            try
            {
                return Database.GetCollection<T>(collectionName);
            }
            catch (Exception ex)
            {
                logger?.Invoke(ex, $"category:{_categoryName} GetCollection execution failed", $"{collectionName}" + typeof(T).Name);
                return null;
            }
        }

        public IMongoCollection<T> GetSimpleCollection<T>(string collectionName, LogError logger)
        {
            try
            {
                return Database.GetCollection<T>(collectionName);
            }
            catch (Exception ex)
            {
                logger?.Invoke(ex, $"category:{_categoryName} GetSimpleCollection execution failed", ex, $"{collectionName}" + typeof(T).Name);
                return null;
            }
        }

        public bool Insert<T>(string collectionName, LogError logger, params T[] documents) where T : IMongoEntity
        {
            if (documents?.Length == 0)
                return false;
            try
            {
                var collection = GetCollection<T>(collectionName, logger);
                if (collection == null)
                    return false;
                if (documents.Length == 1)
                    collection.InsertOne(documents[0]);
                else
                    collection.InsertMany(documents);
                return true;
            }
            catch (Exception ex)
            {
                logger?.Invoke(ex, $"category:{_categoryName} Insert execution failed", ex, $"collection : {collectionName} document count : {documents.Length}");
                return false;
            }
        }

        public T GetCollectionByRecordId<T>(ObjectId id, string collectionName, LogError logger) where T : IMongoEntity
        {
            try
            {
                var collection = GetCollection<T>(collectionName, logger);
                if (collection == null)
                    return default(T);
                return collection.Find<T>(p => p.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger?.Invoke(ex, $"category:{_categoryName} GetCollectionByRecordId execution failed", ex, $"collection : {collectionName} + id : {id}");
                return default(T);
            }
        }

        public async Task<T> GetCollectionByRecordIdAsync<T>(ObjectId id, string collectionName, LogError logger) where T : IMongoEntity
        {
            try
            {
                var collection = GetCollection<T>(collectionName, logger);
                if (collection == null)
                    return default(T);
                return await collection.Find<T>(p => p.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger?.Invoke(ex, $"category:{_categoryName} GetCollectionByRecordIdAsync execution failed", ex, $"collection : {collectionName} + id : {id}");
                return default(T);
            }
        }

        public bool UpdateByRecordId<T>(string collectionName, LogError logger, ObjectId id, UpdateDefinition<T> updateDefinition) where T : IMongoEntity
        {
            try
            {
                var collection = GetCollection<T>(collectionName, logger);
                if (collection == null)
                    return false;
                var updateResult = collection.UpdateOne(Builders<T>.Filter.Eq<ObjectId>(nameof(IMongoEntity.Id), id), updateDefinition);
                return updateResult.ModifiedCount == 1;
            }
            catch (Exception ex)
            {
                logger?.Invoke(ex, $"category:{_categoryName} UpdateByRecordId execution failed", ex, $"collection : {collectionName} + id : {id}");
                return false;
            }
        }

        public async Task<bool> UpdateByRecordIdAsync<T>(string collectionName, LogError logger, ObjectId id, UpdateDefinition<T> updateDefinition) where T : IMongoEntity
        {
            try
            {
                var collection = GetCollection<T>(collectionName, logger);
                if (collection == null)
                    return false;
                var updateResult = await collection.UpdateOneAsync(Builders<T>.Filter.Eq<ObjectId>(nameof(IMongoEntity.Id), id), updateDefinition);
                return updateResult.ModifiedCount == 1;
            }
            catch (Exception ex)
            {
                logger?.Invoke(ex, $"category:{_categoryName} UpdateByRecordId execution failed", ex, $"collection : {collectionName} + id : {id}");
                return false;
            }
        }

        public bool Update<T>(string collectionName, LogError logger, FilterDefinition<T> filters, UpdateDefinition<T> updateDefinition) where T : IMongoEntity
        {
            try
            {
                var collection = GetCollection<T>(collectionName, logger);
                if (collection == null)
                    return false;
                var updateResult = collection.UpdateOne(filters, updateDefinition);
                return updateResult.ModifiedCount == 1;
            }
            catch (Exception ex)
            {
                logger?.Invoke(ex, $"category:{_categoryName} Update execution failed", ex, $"collection : {collectionName} + filters : {filters}");
                return false;
            }
        }

        public bool Replace<T>(string collectionName, LogError logger, T replacement) where T : IMongoEntity
        {
            try
            {
                var collection = GetCollection<T>(collectionName, logger);
                if (collection != null)
                {
                    collection.ReplaceOne<T>(p => p.Id == replacement.Id, replacement);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                logger?.Invoke(ex, $"category:{_categoryName} Replace execution failed", ex, $"collection : {collectionName} id : {replacement.Id}");
                return false;
            }
        }

        void IDisposable.Dispose()
        {

        }
    }
}
