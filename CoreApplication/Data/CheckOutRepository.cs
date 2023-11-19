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
using Microsoft.AspNetCore.Mvc;

namespace CoreApplication.Data
{
    public class CheckOutRepository : ICheckOutRepository
    {
        private readonly DbContext _context = null;
        private readonly IInventoryRepository _inventoryRepository;
        private IOptions<Configuration.DatabaseSettings> _settings;


        public CheckOutRepository(IOptions<Configuration.DatabaseSettings> settings, IInventoryRepository inventoryRepository)
        {
            _context = new DbContext(settings);
            _settings = settings;
            _inventoryRepository = inventoryRepository;
        }

        public async Task AddCheckOut(string sku, int qty, string title, string location, string orderid, bool isCheckedOut)
        {
            var checkOut = new CheckOut
            {
                SKU = sku,
                Qty = qty,
                Location = location,
                Title = title,
                OrderId = orderid,
                IsCheckedOut = isCheckedOut
            };

            await _context.CheckOut.InsertOneAsync(checkOut);
        }

        public async Task DeleteCheckOutsByLocation(string location)
        {
            var filter = Builders<CheckOut>.Filter.Eq(c => c.Location, location);
            await _context.CheckOut.DeleteManyAsync(filter);
        }

        public async Task<List<CheckOut>> GetAllCheckOuts()
        {
            var filter = Builders<CheckOut>.Filter.Empty;
            var checkOuts = await _context.CheckOut.Find(filter).ToListAsync();
            return checkOuts;
        }

        public async Task<bool> UpdateIsCheckedOut(string SKU,string OrderId)
        {
            var filter = Builders<CheckOut>.Filter.And(
                Builders<CheckOut>.Filter.Eq(c => c.OrderId, OrderId),
                Builders<CheckOut>.Filter.Eq(c => c.SKU, SKU));
            var update = Builders<CheckOut>.Update.Set(c => c.IsCheckedOut, true);

            var result = await _context.CheckOut.UpdateOneAsync(filter, update);

            return (result.ModifiedCount > 0);
        }

        public async Task DeleteCheckOutsByOrderId(string orderId) { 
            var filter = Builders<CheckOut>.Filter.Eq(c => c.OrderId, orderId);
            await _context.CheckOut.DeleteManyAsync(filter); 
        }

    }

}
