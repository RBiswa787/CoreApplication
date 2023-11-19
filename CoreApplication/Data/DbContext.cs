using Microsoft.Extensions.Options;
using MongoDB.Driver;
using CoreApplication.Model;


namespace CoreApplication.Data
{
    public class DbContext
    {
        private readonly IMongoDatabase _database = null;

        public DbContext(IOptions<Configuration.DatabaseSettings> settings)
        {
            Console.WriteLine(settings.Value.ConnectionString);
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<User> Users
        {
            get
            {
                return _database.GetCollection<User>("User");
            }
        }

        public IMongoCollection<Supplies> Supplies
        {
            get
            {
                return _database.GetCollection<Supplies>("Supplies");
            }
        }

        public IMongoCollection<Order> Order
        {
            get
            {
                return _database.GetCollection<Order>("Order");
            }
        }

        public IMongoCollection<InventoryLayout> InventoryLayout
        {
            get
            {
                return _database.GetCollection<InventoryLayout>("InventoryLayout");
            }
        }

        public IMongoCollection<Inventory> Inventory
        {
            get
            {
                return _database.GetCollection<Inventory>("Inventory");
            }
        }

        public IMongoCollection<CheckIn> CheckIn
        {
            get
            {
                return _database.GetCollection<CheckIn>("CheckIn");
            }
        }
        public IMongoCollection<CheckOut> CheckOut
        {
            get
            {
                return _database.GetCollection<CheckOut>("CheckOut");
            }
        }


    }
}

