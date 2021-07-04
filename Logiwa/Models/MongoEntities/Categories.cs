using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using static Logiwa.Factory.Database.MongoConnectionBase;

namespace Logiwa.Models.MongoEntities
{

    public class Categories : MongoEntity
    {
        [BsonIgnore]
        public const string COLLECTIONNAME = "Categories";
        public ObjectId CategoryId { get; set; }
        public string Name { get; set; }
    }

}
