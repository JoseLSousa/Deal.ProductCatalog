namespace Application.DTOs.Export
{
    public class ReportStatisticsDto
    {
        public int TotalActiveProducts { get; set; }
        public decimal AveragePrice { get; set; }
        public Dictionary<string, int> ProductsByCategory { get; set; } = new();
        public List<TopProductDto> Top3MostExpensive { get; set; } = new();
    }
}