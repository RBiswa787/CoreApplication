using CoreApplication.Model;

namespace CoreApplication.Interfaces
{
    public interface IInventoryLayoutRepository
    {
        Task<InventoryLayout> GetInventoryLayout();
        Task<bool> UpdateInventoryLayout(InventoryLayout layout);
    }
}
