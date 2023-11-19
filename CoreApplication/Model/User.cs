using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoreApplication.Model
{
    public class User
    {
        [BsonId]
        public ObjectId InternalId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public Boolean Verified { get; set; } = false;

        public string Token { get; set; } = null;
    }
}
