using Application.DTOs;

namespace Application.Interfaces
{
    public interface IItemRepository
    {
        Task<List<ItemDto>> ListAllItems();
        Task<ItemDto?> GetItemById(Guid id);
        Task CreateItem(ItemDto itemDto);
        Task DeleteItem(Guid id);
        Task UpdateItem(Guid id, ItemDto itemDto);
    }
}
