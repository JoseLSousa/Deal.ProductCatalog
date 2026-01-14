using System.Text;
using System.Text.Json;
using Application.DTOs.Export;
using Application.Interfaces;
using Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Infra.Services
{
    public class ExportService : IExportService
    {
        private readonly AppDbContext _context;
        private readonly IAuditLogService _auditLogService;

        public ExportService(AppDbContext context, IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        public async Task<ProductReportDto> GetReportDataAsync()
        {
            // Debug: verificar total de produtos no banco
            var totalProducts = await _context.Products.CountAsync();
            var activeCount = await _context.Products.Where(p => p.Active).CountAsync();
            var notDeletedCount = await _context.Products.Where(p => p.DeletedAt == null).CountAsync();
            
            Console.WriteLine($"[DEBUG] Total produtos: {totalProducts}, Ativos: {activeCount}, Não deletados: {notDeletedCount}");

            var activeProducts = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .Where(p => p.Active && p.DeletedAt == null)
                .OrderBy(p => p.Name)
                .ToListAsync();

            Console.WriteLine($"[DEBUG] Produtos encontrados para relatório: {activeProducts.Count}");

            var report = new ProductReportDto();

            report.Products = activeProducts.Select(p => new ProductReportItemDto
            {
                Name = p.Name,
                Description = p.Description,
                Category = p.Category?.Name ?? "Sem categoria",
                Price = p.Price,
                Tags = p.Tags != null && p.Tags.Any() 
                    ? string.Join(", ", p.Tags.Where(t => t.DeletedAt == null).Select(t => t.Name))
                    : string.Empty,
                CreatedAt = p.CreatedAt
            }).ToList();

            if (activeProducts.Any())
            {
                report.Statistics.TotalActiveProducts = activeProducts.Count;
                report.Statistics.AveragePrice = activeProducts.Average(p => p.Price);
                
                report.Statistics.ProductsByCategory = activeProducts
                    .GroupBy(p => p.Category?.Name ?? "Sem categoria")
                    .ToDictionary(g => g.Key, g => g.Count());

                report.Statistics.Top3MostExpensive = activeProducts
                    .OrderByDescending(p => p.Price)
                    .Take(3)
                    .Select(p => new TopProductDto
                    {
                        Name = p.Name,
                        Price = p.Price
                    })
                    .ToList();
            }

            return report;
        }

        public async Task<byte[]> GenerateCsvReportAsync()
        {
            var report = await GetReportDataAsync();
            var csv = new StringBuilder();

            csv.AppendLine("Nome,Descrição,Categoria,Preço,Tags,Data de Criação");

            foreach (var product in report.Products)
            {
                csv.AppendLine($"\"{product.Name}\",\"{product.Description}\",\"{product.Category}\",{product.Price},\"{product.Tags}\",{product.CreatedAt:yyyy-MM-dd HH:mm:ss}");
            }

            csv.AppendLine();
            csv.AppendLine("=== ESTATÍSTICAS ===");
            csv.AppendLine($"Total de Produtos Ativos,{report.Statistics.TotalActiveProducts}");
            csv.AppendLine($"Preço Médio,{report.Statistics.AveragePrice:F2}");
            csv.AppendLine();
            csv.AppendLine("=== PRODUTOS POR CATEGORIA ===");
            
            foreach (var category in report.Statistics.ProductsByCategory)
            {
                csv.AppendLine($"{category.Key},{category.Value}");
            }

            csv.AppendLine();
            csv.AppendLine("=== TOP 3 MAIS CAROS ===");
            csv.AppendLine("Nome,Preço");
            
            foreach (var top in report.Statistics.Top3MostExpensive)
            {
                csv.AppendLine($"\"{top.Name}\",{top.Price:F2}");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        public async Task<string> GenerateJsonReportAsync()
        {
            var report = await GetReportDataAsync();
            
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Serialize(report, options);
        }
    }
}
