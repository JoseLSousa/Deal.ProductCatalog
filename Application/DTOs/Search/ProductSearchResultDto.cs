namespace Application.DTOs.Search
{
    public class ProductSearchResultDto
    {
        public List<ResponseProductDto> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public decimal AveragePrice { get; set; }
        public Dictionary<string, int> ItemsByCategory { get; set; } = new();
    }
}