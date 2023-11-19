using System;
using MongoDB.Bson;
using MongoDB.Driver;
using CoreApplication.Model;
using System.Threading.Tasks;
using CoreApplication.Interfaces;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using CoreApplication.Configuration;

using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver.Linq;
using Microsoft.AspNetCore.Routing;
using CoreApplication.Helpers;
using System.Security.Claims;
using System.Text;

namespace CoreApplication.Data
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly DbContext _context = null;
        private readonly IInventoryLayoutRepository _inventoryLayoutRepository;
        private IOptions<Configuration.DatabaseSettings> _settings;


        public InventoryRepository(IOptions<Configuration.DatabaseSettings> settings, IInventoryLayoutRepository inventoryLayoutRepository)
        {
            _context = new DbContext(settings);
            _settings = settings;
            _inventoryLayoutRepository = inventoryLayoutRepository;
        }

        public async Task Reset()
        {
            // Get the Inventory Layout
            var inventoryLayout = await _inventoryLayoutRepository.GetInventoryLayout();

            // Delete all existing documents from the Inventory collection
            var deleteResult = await _context.Inventory.DeleteManyAsync(Builders<Inventory>.Filter.Empty);

            // Generate new documents for each Zone-Aisle-Rack-Shelf-Bin location
            var inventoryList = new List<Inventory>();
            for (char zone = 'A'; zone < Convert.ToChar(65 + inventoryLayout.Zone); zone++)
            {
                for (int aisle = 1; aisle <= inventoryLayout.Aisle; aisle++)
                {
                    for (int rack = 1; rack <= inventoryLayout.Rack; rack++)
                    {
                        for (int shelf = 1; shelf <= inventoryLayout.Shelf; shelf++)
                        {
                            for (int bin = 1; bin <= inventoryLayout.Bin; bin++)
                            {
                                var location = $"LOC-{zone}-{aisle}-{rack}-{shelf}-{bin}";
                                var inventory = new Inventory { Location = location };
                                inventoryList.Add(inventory);
                            }
                        }
                    }
                }
            }
            if (inventoryList.Count > 0)
            {
                await _context.Inventory.InsertManyAsync(inventoryList);
            }
        }

        public async Task<List<Inventory>> GetCurrentInventory()
        {
            return await _context.Inventory.Find(i => true).ToListAsync();
        }

        public async Task<bool> UpdateInventory(string location, string sku, int qty, string title)
        {


            var filter = Builders<Inventory>.Filter.Eq(i => i.Location, location);
            var currentInventory = await _context.Inventory.Find(filter).FirstOrDefaultAsync();

            if (currentInventory != null)
            {
                qty += currentInventory.Qty;
            }

            Console.WriteLine(location);
            var update = Builders<Inventory>.Update
                .Set(i => i.SKU, sku)
                .Set(i => i.Qty, qty)
                .Set(i => i.Title, title)
                .Set(i => i.IsOccupied, true);

            var result = await _context.Inventory.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
        public async Task<string> AllocateLocation(string sku, int qty, string title)
        {
            var currentInventory = await GetCurrentInventory();
            var availableInventory = await GetAvailableInventory();
            var inventoryForSku = currentInventory.FirstOrDefault(i => i.SKU == sku);
            if (inventoryForSku != null)
            {
                return inventoryForSku.Location; // Location already allocated for the SKU
            }

            var location = GetAvailableLocation(availableInventory);
            if (string.IsNullOrEmpty(location))
            {
                return null; // No available locations
            }

            //UpdateInventory(location,sku,qty);


            return location;
        }

        

        public async Task<List<Inventory>> GetAvailableInventory()
        {
            var availableInventory = await GetCurrentInventory();
            return availableInventory
                .Where(i => !i.IsOccupied)
                .ToList();
        }

        private string GetAvailableLocation(List<Inventory> availableInventory)
        {
            var locations = new Dictionary<string, List<string>>();
            Console.WriteLine(availableInventory);
            // Group inventory by zone, aisle, rack, shelf, and bin
            foreach (var inventory in availableInventory)
            {
                var parts = inventory.Location.Split('-');
                var zone = parts[1];
                var aisle = parts[2];
                var rack = parts[3];
                var shelf = parts[4];
                var bin = parts[5];

                return $"LOC-{zone}-{aisle}-{rack}-{shelf}-{bin}"; 
            }

            return null;
                
        }

        public async Task<bool> DeductQty(string location, int qty)
        {
            var filter = Builders<Inventory>.Filter.Eq(c => c.Location, location); 
            var update = Builders<Inventory>.Update.Inc(c => c.Qty, -qty);

            var result = await _context.Inventory.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<Inventory>
            {
                ReturnDocument = ReturnDocument.After
            });

            if (result.Qty == 0)
            {
                var updateInventory = Builders<Inventory>.Update
                    .Set(c => c.SKU, "")
                    .Set(c => c.Title, "")
                    .Set(c => c.IsOccupied, false);

                var updateResult = await _context.Inventory.UpdateOneAsync(filter, updateInventory);

                return (updateResult.ModifiedCount > 0);
            }
            else
            {
                return (result.Qty > 0);
            }
        }

    }
    public class SkuWithQty
    {
        public string SKU { get; set; }
        public int Qty { get; set; }

        public string Title {  get; set; }
    }
}
