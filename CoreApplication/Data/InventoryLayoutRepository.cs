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
    public class InventoryLayoutRepository : IInventoryLayoutRepository
    {

        private readonly DbContext _context = null;
        private IOptions<Configuration.DatabaseSettings> _settings;
        public InventoryLayoutRepository(IOptions<Configuration.DatabaseSettings> settings)
        {
            _context = new DbContext(settings);
            _settings = settings;
        }

        public async Task<InventoryLayout> GetInventoryLayout()
        {
            try
            {
                var inventoryLayout = await _context.InventoryLayout
                    .Find(_ => true)
                    .FirstOrDefaultAsync();

                return inventoryLayout;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateInventoryLayout(InventoryLayout layout)
        {
            try
            {
                var update = Builders<InventoryLayout>.Update
                    .Set(o => o.Zone, layout.Zone)
                    .Set(o => o.Aisle, layout.Aisle)
                    .Set(o => o.Rack, layout.Rack)
                    .Set(o => o.Shelf, layout.Shelf)
                    .Set(o => o.Bin, layout.Bin);

                var result = await _context.InventoryLayout.UpdateOneAsync(_ => true, update);

                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
