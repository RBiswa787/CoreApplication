using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApplication.Model;

namespace CoreApplication.Interfaces
{
    public interface ICheckOutRepository
    {
        Task AddCheckOut(string sku, int qty, string title, string location, string orderid, bool isCheckedOut);

        Task<List<CheckOut>> GetAllCheckOuts();

        Task<bool> UpdateIsCheckedOut(string SKU, string OrderId);
        Task DeleteCheckOutsByLocation(string location);

        Task DeleteCheckOutsByOrderId(string orderId);
    }
}
