using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace backupDBv1.Entity
{
    [BsonIgnoreExtraElements]
    public class Contract
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string domain { get; set; }
        public string sub_domain { get; set; }
        public long active_date { get; set; }
        public int preview_notification_date_number { get; set; }
        public long duration { get; set; }
        public string vocative { get; set; }
        public string customer_name { get; set; }
        public string phone_number { get; set; }
        public string email { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public DateTime date_remind { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class DataResultDto
    {
        public bool status { get; set; }

        public Contract[] output { get; set; }

    }

    public class DataRunCommand<T>
    {
        public T retval { get; set; }
        public int ok { get; set; }
    }
}
