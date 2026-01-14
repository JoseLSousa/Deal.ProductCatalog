using Application.DTOs.Export;

namespace Application.Interfaces
{
    public interface IExportService
    {
        Task<byte[]> GenerateCsvReportAsync();
        Task<string> GenerateJsonReportAsync();
        Task<ProductReportDto> GetReportDataAsync();
    }
}