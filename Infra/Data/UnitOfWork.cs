using Application.Interfaces;
using Domain.Abstractions;
using Infra.Data.Repositories;

namespace Infra.Data
{
    public class UnitOfWork(AppDbContext context,
        IAuditLogService auditLogService
        ) : IUnitOfWork
    {
        private IProductRepository? _productRepository;

        public IProductRepository Products =>
            _productRepository ??= new ProductRepository(context);


        public async Task<int> CommitAsync()
        {
            return await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
