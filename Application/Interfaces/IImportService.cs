using Application.DTOs.Import;

namespace Application.Interfaces
{
    public interface IImportService
    {
        Task<ImportResultDto> ImportFromExternalApiAsync(string userId);
    }
}