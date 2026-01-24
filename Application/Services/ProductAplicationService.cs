using Application.Interfaces;
using Domain.Abstractions;

namespace Application.Services
{
    public class ProductAplicationService(IUnitOfWork unitOfWork) : IProductApplicationService
    {

        public async Task ActivateProductAsync(Guid productId)
        {
            var product = await unitOfWork.Products.GetByIdAsync(productId)
                ?? throw new KeyNotFoundException("Product not found.");

            product.Activate();

            await unitOfWork.CommitAsync();
        }
    }
}
