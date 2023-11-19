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
    public class OrderRepository : IOrderRepository
    {

        private readonly DbContext _context = null;
        private IOptions<Configuration.DatabaseSettings> _settings;
        public OrderRepository(IOptions<Configuration.DatabaseSettings> settings)
        {
            _context = new DbContext(settings);
            _settings = settings;
 
        }

        public async Task<List<Order>> GetAllOrders()
        {
            try
            {
                return await _context.Order.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Order> GetOrderById(string orderId)
        {
            try
            {
                var filter = Builders<Order>.Filter.Eq(order => order.OrderId, orderId);
                return await _context.Order.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Order>> GetOrdersByPartnerId(string partnerId)
        {
            try
            {
                var filter = Builders<Order>.Filter.Eq(order => order.Partner, partnerId);
                return await _context.Order.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> AddOrder(Order order)
        {
            try
            {
                await _context.Order.InsertOneAsync(order);

                return order.OrderId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateOrderStatus(string orderId, string status)
        {
            try
            {
                
                var filter = Builders<Order>.Filter.Eq(o => o.OrderId, orderId);

                var update = Builders<Order>.Update.Set(o => o.OrderStatus, status);

                

                var result = await _context.Order.UpdateOneAsync(filter, update);

                var order = await GetOrderById(orderId);

                if (status == "received" && order.PartnerType == "supplier")
                {
                    foreach (var item in order.Items)
                    {
                        var SKU = item.SKU;
                        var Qty = item.Qty;
                        var Title = item.Title;
                        var location = await AllocateLocation(SKU, Qty, Title);

                        await AddCheckIn(SKU, Qty, Title, location);
                        await BlockInventory(location);

                    }
                }


                if(status == "confirmed" && order.PartnerType == "vendor")
                {
                    foreach (var item in order.Items)
                    {
                        var inventory_filter = Builders<Inventory>.Filter.Eq(bin => bin.SKU, item.SKU);
                        var found = await _context.Inventory.FindAsync(inventory_filter);
                        var location = found.First().Location;
                        var SKU = item.SKU;
                        var Qty = item.Qty;
                        var Title = item.Title;

                        await AddCheckOut(SKU, Qty, Title, location, order.OrderId);

                    }
                }


                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RemoveOrder(string orderId)
        {
            try
            {
                var filter = Builders<Order>.Filter.Eq(o => o.OrderId, orderId);

                var update = Builders<Order>.Update.Set(o => o.OrderStatus, "cancelled");

                var result = await _context.Order.UpdateOneAsync(filter, update);

                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Inventory>> GetCurrentInventory()
        {
            return await _context.Inventory.Find(i => true).ToListAsync();
        }
        public async Task BlockInventory(string location)
        {


            var filter = Builders<Inventory>.Filter.Eq(i => i.Location, location);


            var update = Builders<Inventory>.Update
                .Set(i => i.IsOccupied, true);


            await _context.Inventory.UpdateOneAsync(filter, update);
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

        public async Task AddCheckIn(string sku, int qty, string title, string location)
        {
            var checkIn = new CheckIn
            {
                SKU = sku,
                Qty = qty,
                Title = title,
                Location = location
            };

            await _context.CheckIn.InsertOneAsync(checkIn);
        }


        public async Task AddCheckOut(string sku, int qty, string title, string location, string orderid)
        {
            var checkOut = new CheckOut
            {
                SKU = sku,
                Qty = qty,
                Title = title,
                Location = location,
                OrderId = orderid
            };

            await _context.CheckOut.InsertOneAsync(checkOut);
        }




        public async Task CheckInToInventory(string location,string sku, int qty, string title)
        {


            var filter = Builders<Inventory>.Filter.Eq(i => i.Location, location);


            var update = Builders<Inventory>.Update
                .Set(i => i.SKU, sku)
                .Set(i => i.Qty, qty)
                .Set(i => i.Title, title)
                .Set(i => i.IsOccupied, true);


            await _context.Inventory.UpdateOneAsync(filter, update);
            await DeleteCheckInsByLocation(location);
        }

        public async Task DeleteCheckInsByLocation(string location)
        {
            var filter = Builders<CheckIn>.Filter.Eq(c => c.Location, location);
            await _context.CheckIn.DeleteManyAsync(filter);
        }


    }
}
