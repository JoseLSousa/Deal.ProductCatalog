using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infra.NoSql;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Text.Json;

namespace Infra.Repositories
{
    public class AuditLogService : IAuditLogService
    {
        private readonly AuditDbContext _context;
        private readonly ILogger<AuditLogService> _logger;
        private readonly string _fallbackLogPath;
        private bool _isMongoDbAvailable = true;

        public AuditLogService(
            AuditDbContext context,
            ILogger<AuditLogService> logger)
        {
            _context = context;
            _logger = logger;
            _fallbackLogPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Logs",
                "AuditLogs"
            );

            // Garantir que o diretório existe
            Directory.CreateDirectory(_fallbackLogPath);
        }

        public async Task LogAsync(LogDto logDto)
        {
            var log = new AuditLog(
                logDto.Action,
                logDto.UserId,
                logDto.Payload
            );

            try
            {
                // Tentar salvar no MongoDB
                if (_isMongoDbAvailable)
                {
                    await _context.AuditLogs.InsertOneAsync(log);
                    _logger.LogDebug("Log de auditoria salvo no MongoDB: {Action}", logDto.Action);
                }
                else
                {
                    // Se MongoDB estiver indisponível, usar fallback
                    await SaveToFileAsync(log);
                }
            }
            catch (MongoException ex)
            {
                // MongoDB está indisponível
                _isMongoDbAvailable = false;
                _logger.LogWarning(
                    ex,
                    "MongoDB indisponível. Usando fallback de arquivo. Action: {Action}",
                    logDto.Action
                );

                // Salvar em arquivo como fallback
                await SaveToFileAsync(log);

                // Tentar reconectar no próximo log
                _ = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromMinutes(1));
                    _isMongoDbAvailable = true;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro crítico ao salvar log de auditoria. Action: {Action}",
                    logDto.Action
                );

                // Tentar salvar em arquivo como último recurso
                try
                {
                    await SaveToFileAsync(log);
                }
                catch (Exception fileEx)
                {
                    _logger.LogCritical(
                        fileEx,
                        "Falha ao salvar log de auditoria em arquivo. Action: {Action}",
                        logDto.Action
                    );
                }
            }
        }

        private async Task SaveToFileAsync(AuditLog log)
        {
            var fileName = $"audit-{DateTime.UtcNow:yyyy-MM-dd}.json";
            var filePath = Path.Combine(_fallbackLogPath, fileName);

            var logEntry = new
            {
                log.LogId,
                Action = log.Action.ToString(),
                log.UserId,
                log.TimeStamp,
                Payload = log.Payload?.ToString() ?? "{}"
            };

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(logEntry, jsonOptions);

            // Adicionar ao arquivo (append)
            await File.AppendAllTextAsync(filePath, json + ",\n");

            _logger.LogInformation(
                "Log de auditoria salvo em arquivo: {FileName}. Action: {Action}",
                fileName,
                log.Action
            );
        }
    }
}
