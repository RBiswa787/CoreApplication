using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApplication.Model;

namespace CoreApplication.Interfaces
{
    public interface ICheckInRepository
    {
        Task AddCheckIn(string sku, int qty,string title, string location);

        Task<List<CheckIn>> GetAllCheckIns();

        Task DeleteCheckInsByLocation(string location);
    }
}
