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
    public class CheckInRepository : ICheckInRepository
    {
        private readonly DbContext _context = null;
        private readonly IInventoryRepository _inventoryRepository;
        private IOptions<Configuration.DatabaseSettings> _settings;


        public CheckInRepository(IOptions<Configuration.DatabaseSettings> settings, IInventoryRepository inventoryRepository)
        {
            _context = new DbContext(settings);
            _settings = settings;
            _inventoryRepository = inventoryRepository;
        }

        public async Task AddCheckIn(string sku, int qty, string title, string location)
        {
            var checkIn = new CheckIn
            {
                SKU = sku,
                Qty = qty,
                Location = location,
                Title = title
            };

            await _context.CheckIn.InsertOneAsync(checkIn);
        }

        public async Task DeleteCheckInsByLocation(string location)
        {
            var filter = Builders<CheckIn>.Filter.Eq(c => c.Location, location);
            await _context.CheckIn.DeleteManyAsync(filter);
        }

        public async Task<List<CheckIn>> GetAllCheckIns()
        {
            var filter = Builders<CheckIn>.Filter.Empty;
            var checkIns = await _context.CheckIn.Find(filter).ToListAsync();
            return checkIns;
        }

        
    }

}
