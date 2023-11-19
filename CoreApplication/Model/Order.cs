using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoreApplication.Model
{
    public class Order
    {
        [BsonId]
        public ObjectId InternalId { get; set; }
        [Key]
        public string OrderId { get; set; }
        public string Partner { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string OrderStatus { get; set; }
        public string PartnerType { get; set; }

        public ItemDTO[] Items { get; set; }
    }

    public class ItemDTO
    {
       
        public string SKU { get; set; } = String.Empty;
        public string Title { get; set; } = String.Empty;

        public int Qty { get; set; } = 0;
    }
}
