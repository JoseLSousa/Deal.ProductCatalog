using Domain.Abstractions;
using Infra.Data.Repositories;

namespace Infra.Data
{
    public class UnitOfWork(AppDbContext context
        ) : IUnitOfWork
    {
        private IProductRepository? _productRepository;
        private ITagRepository? _tagRepository;
        private ICategoryRepository? _categoryRepository;

        public IProductRepository Products =>
            _productRepository ??= new ProductRepository(context);

        public ITagRepository Tags =>
            _tagRepository ??= new TagRepository(context);

        public ICategoryRepository Categories =>
            _categoryRepository ??= new CategoryRepository(context);

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
