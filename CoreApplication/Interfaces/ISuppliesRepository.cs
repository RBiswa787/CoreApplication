using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApplication.Model;

namespace CoreApplication.Interfaces
{
    public interface ISuppliesRepository
    {
        Task<IEnumerable<Supplies>> GetAllSupplies();

        Task CreateSupplyAsync(Supplies supply);

        Task<Supplies> GetSupplyByIdAsync(string id);

        Task<bool> DeleteSupplyAsync(string id);

        Task<IEnumerable<Supplies>> GetSuppliesBySupplier(string supplier);
    }
}
