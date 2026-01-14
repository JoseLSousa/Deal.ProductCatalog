namespace Application.DTOs.Export
{
    public class ProductReportItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Tags { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}