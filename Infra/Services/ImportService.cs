using System.Text.Json;
using Application.DTOs.Import;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Infra.Services
{
    public class ImportService : IImportService
    {
        private readonly AppDbContext _context;
        private readonly IAuditLogService _auditLogService;
        private readonly HttpClient _httpClient;
        private const string ExternalApiUrl = "https://fakestoreapi.com/products";

        public ImportService(
            AppDbContext context, 
            IAuditLogService auditLogService,
            HttpClient httpClient)
        {
            _context = context;
            _auditLogService = auditLogService;
            _httpClient = httpClient;
        }

        public async Task<ImportResultDto> ImportFromExternalApiAsync(string userId)
        {
            var result = new ImportResultDto();

            try
            {
                var response = await _httpClient.GetStringAsync(ExternalApiUrl);
                var externalProducts = JsonSerializer.Deserialize<List<ExternalProductDto>>(
                    response, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (externalProducts == null || !externalProducts.Any())
                {
                    result.Messages.Add("Nenhum produto encontrado na API externa.");
                    return result;
                }

                result.TotalFetched = externalProducts.Count;

                foreach (var externalProduct in externalProducts)
                {
                    try
                    {
                        await ProcessExternalProductAsync(externalProduct, result);
                    }
                    catch (Exception ex)
                    {
                        result.Messages.Add($"Erro ao processar produto '{externalProduct.Title}': {ex.Message}");
                    }
                }

                await _auditLogService.LogAsync(new Application.DTOs.LogDto(
                    LogAction.IMPORT_EXECUTED,
                    userId,
                    new
                    {
                        Source = ExternalApiUrl,
                        TotalFetched = result.TotalFetched,
                        Imported = result.Imported,
                        Skipped = result.Skipped
                    }
                ));

                return result;
            }
            catch (Exception ex)
            {
                result.Messages.Add($"Erro ao consumir API externa: {ex.Message}");
                return result;
            }
        }

        private async Task ProcessExternalProductAsync(
            ExternalProductDto externalProduct, 
            ImportResultDto result)
        {
            var categoryName = NormalizeCategoryName(externalProduct.Category);
            
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == categoryName.ToLower());

            if (category == null)
            {
                category = new Category(categoryName);
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
            }

            var isDuplicate = await _context.Products
                .AnyAsync(p => 
                    p.Name.ToLower() == externalProduct.Title.ToLower() && 
                    p.CategoryId == category.CategoryId &&
                    !p.IsDeleted);

            if (isDuplicate)
            {
                result.Skipped++;
                result.Messages.Add($"Produto '{externalProduct.Title}' já existe (categoria: {categoryName}).");
                return;
            }

            var product = new Product(
                externalProduct.Title,
                externalProduct.Description,
                externalProduct.Price,
                true,
                category.CategoryId
            );

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            result.Imported++;
            result.Messages.Add($"Produto '{externalProduct.Title}' importado com sucesso.");
        }

        private static string NormalizeCategoryName(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return "Geral";

            return char.ToUpper(category[0]) + category.Substring(1).ToLower();
        }
    }
}
