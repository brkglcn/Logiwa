using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using static Logiwa.Factory.Database.MongoConnectionBase;

namespace Logiwa.Models.MongoEntities
{

    public class Products : MongoEntity
    {
        [BsonIgnore]
        public const string COLLECTIONNAME = "Products";
        public string Title { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public ObjectId? CategoryId { get; set; }
    }
}

public abstract class MongoEntity : IMongoEntity
{
    [BsonId]
    public ObjectId Id { get; set; }
}