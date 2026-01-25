namespace Domain.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ITagRepository Tags { get; }
        ICategoryRepository Categories { get; }

        Task<int> CommitAsync();
    }
}
