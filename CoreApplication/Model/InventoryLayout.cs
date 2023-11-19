using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace CoreApplication.Model
{
    public class InventoryLayout
    {
        [BsonId]
        public ObjectId InternalId { get; set; }

        public int Zone { get; set; } = 4;

        public int Aisle { get; set; } = 2;

        public int Rack { get; set; } = 2;

        public int Shelf { get; set; } = 5;

        public int Bin {  get; set; } = 5;
    }
}
