using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoreApplication.Model
{
    public class Supplies
    {
        [BsonId]
        public ObjectId InternalId { get; set; }

        public string Supplier { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
    }
}
