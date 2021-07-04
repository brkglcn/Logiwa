using Logiwa.Factory.Database;
using Microsoft.Extensions.Configuration;

namespace Logiwa.Factory.Database
{
    public class MongoConnection : MongoConnectionBase
    {
        public MongoConnection(string dbName)
            : base("localhost",27017,dbName)
        {
        }
    }
}
