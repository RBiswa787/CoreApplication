using CoreApplication.Data;
using CoreApplication.Model;

namespace CoreApplication.Interfaces
{
    public interface IInventoryRepository
    {

        Task Reset();
        Task<List<Inventory>> GetCurrentInventory();

        Task<string> AllocateLocation(string sku,int qty, string title);

        Task<bool> UpdateInventory(string location, string sku, int qty, string title);

        Task<bool> DeductQty(string location, int qty);
    }

    public class SkuWithQty
    {
        public string SKU { get; set; }
        public int Qty { get; set; }

        public string Title { get; set; }
    }
}
