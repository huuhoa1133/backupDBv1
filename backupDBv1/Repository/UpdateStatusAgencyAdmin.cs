using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backupDBv1
{
    public class UpdateStatusAgencyAdmin
    {
        private IMongoDatabase _db;
        public UpdateStatusAgencyAdmin(IMongoDatabase db)
        {
            _db = db;
        }

        public  UpdateResult UpdateStatus()
        {
            try
            {
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var filter = Builders<BsonDocument>.Filter.Lte("date_end", timestamp);
                filter &= Builders<BsonDocument>.Filter.Eq("status", true);//true = khong khoa
                var update = Builders<BsonDocument>.Update
                    .Set("status", false);
                var a = _db.GetCollection<BsonDocument>("contract").UpdateMany(filter, update);
                return a;
            }catch(Exception ex)
            {
                throw;
            }
        }
    }
}
