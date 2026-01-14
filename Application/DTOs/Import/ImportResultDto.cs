namespace Application.DTOs.Import
{
    public class ImportResultDto
    {
        public int TotalFetched { get; set; }
        public int Imported { get; set; }
        public int Skipped { get; set; }
        public List<string> Messages { get; set; } = new();
    }
}