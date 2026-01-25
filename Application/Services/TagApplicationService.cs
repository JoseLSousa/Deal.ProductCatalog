using Application.Interfaces;
using Domain.Abstractions;
using Domain.Entities;

namespace Application.Services
{
    public class TagApplicationService(IUnitOfWork unitOfWork) : ITagApplicationService
    {
        public async Task<Guid> CreateTagAsync(string name)
        {
            var tag = new Tag(name);

            await unitOfWork.Tags.AddAsync(tag);
            await unitOfWork.CommitAsync();

            return tag.TagId;
        }

        public async Task UpdateTagNameAsync(Guid tagId, string newName)
        {
            var tag = await unitOfWork.Tags.GetByIdAsync(tagId)
                ?? throw new KeyNotFoundException("Tag not found.");

            tag.UpdateName(newName);

            await unitOfWork.CommitAsync();
        }

        public async Task AssignTagToProductAsync(Guid tagId, Guid productId)
        {
            var tag = await unitOfWork.Tags.GetByIdAsync(tagId)
                ?? throw new KeyNotFoundException("Tag not found.");

            tag.AssignToProduct(productId);

            await unitOfWork.CommitAsync();
        }

        public async Task DeleteTagAsync(Guid tagId)
        {
            var tag = await unitOfWork.Tags.GetByIdAsync(tagId)
                ?? throw new KeyNotFoundException("Tag not found.");

            tag.Delete();

            await unitOfWork.CommitAsync();
        }

        public async Task RestoreTagAsync(Guid tagId)
        {
            var tag = await unitOfWork.Tags.GetByIdAsync(tagId)
                ?? throw new KeyNotFoundException("Tag not found.");

            tag.Restore();

            await unitOfWork.CommitAsync();
        }

        public async Task<Tag?> GetTagByIdAsync(Guid tagId)
        {
            return await unitOfWork.Tags.GetByIdAsync(tagId);
        }

        public async Task<Tag?> GetTagByNameAsync(string name)
        {
            return await unitOfWork.Tags.GetByNameAsync(name);
        }

        public async Task<IReadOnlyList<Tag>> GetTagsByProductIdAsync(Guid productId)
        {
            return await unitOfWork.Tags.GetByProductIdAsync(productId);
        }

        public async Task<IReadOnlyList<Tag>> GetActiveTagsAsync()
        {
            return await unitOfWork.Tags.GetActiveAsync();
        }
    }
}
