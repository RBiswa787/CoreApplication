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
    public class SuppliesRepository : ISuppliesRepository
    {
        private readonly DbContext _context = null;
        private IOptions<Configuration.DatabaseSettings> _settings;
        public SuppliesRepository(IOptions<Configuration.DatabaseSettings> settings)
        {
            _context = new DbContext(settings);
            _settings = settings;
        }

        public async Task<IEnumerable<Supplies>> GetAllSupplies()
        {
            var cursor = await _context.Supplies.FindAsync(supply => true);
            return await cursor.ToListAsync();
        }

        public async Task<Supplies> GetSupplyByIdAsync(string sku)
        {
            var filter = Builders<Supplies>.Filter.Eq(supply => supply.SKU, sku);
            return await _context.Supplies.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateSupplyAsync(Supplies supply)
        {
            await _context.Supplies.InsertOneAsync(supply);
        }


        public async Task<bool> DeleteSupplyAsync(string id)
        {
            var filter = Builders<Supplies>.Filter.Eq(supply => supply.SKU, id);
            var result = await _context.Supplies.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task<IEnumerable<Supplies>> GetSuppliesBySupplier(string supplier)
        {
            try
            {
                var filter = Builders<Supplies>.Filter.Eq(u => u.Supplier, supplier);
                var supplies = await _context.Supplies.Find(filter).ToListAsync();
                return supplies;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
