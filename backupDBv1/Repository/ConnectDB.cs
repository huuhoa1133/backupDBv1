using MongoDB.Driver;
using System.Configuration;

namespace backupDBv1.Repository
{
    public class ConnectDB
    {
        private static string ConnectionString { get; set; }
        private static MongoUrl MongoUrl { get; set; }
        private static MongoClient MongoClient { get; set; }
        public static IMongoDatabase db { get; set; }

        public IMongoDatabase GetDB(string ConnectionString)
        {
            MongoUrl = new MongoUrl(ConnectionString);
            MongoClient = new MongoClient(MongoUrl);
            db = MongoClient.GetDatabase(MongoUrl.DatabaseName);

            return db;
        }
    }
}
