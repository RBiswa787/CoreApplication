using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApplication.Model;

namespace CoreApplication.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllOrders();
        Task<Order> GetOrderById(string orderId);
        Task<List<Order>> GetOrdersByPartnerId(string partnerId);
        Task<string> AddOrder(Order order);
        Task<bool> RemoveOrder(string orderId);
        Task<bool> UpdateOrderStatus(string orderId, string status);

        Task CheckInToInventory(string location, string sku, int qty, string title);

        Task DeleteCheckInsByLocation(string location);

    }
}
