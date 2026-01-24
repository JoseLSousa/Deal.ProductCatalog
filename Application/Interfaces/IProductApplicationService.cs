namespace Application.Interfaces
{
    public interface IProductApplicationService
    {
        Task ActivateProductAsync(Guid productId);
    }
}
