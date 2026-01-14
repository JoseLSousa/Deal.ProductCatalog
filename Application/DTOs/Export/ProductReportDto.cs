namespace Application.DTOs.Export
{
    public class ProductReportDto
    {
        public List<ProductReportItemDto> Products { get; set; } = new();
        public ReportStatisticsDto Statistics { get; set; } = new();
    }
}