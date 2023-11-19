using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoreApplication.Model
{
    public class CheckOut
    {
        [BsonId]
        public ObjectId InternalId { get; set; }
        public string SKU { get; set; } = String.Empty;
        public int Qty { get; set; } = 0;

        public string Title { get; set; } = String.Empty;
        public string Location { get; set; } = String.Empty;

        public string OrderId {  get; set; } = String.Empty;

        public Boolean IsCheckedOut { get; set; } = false;
    }
}
