using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Modules.Deployments.Domain.Entities;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Modules.Logs.Application.Interfaces;
using Modules.Logs.Persistence.Context;

namespace Modules.Deployments.Infrastructure.Services
{
    public class DeploymentLogService : IDeploymentLogService
    {
        private readonly LogsDbContext _context;
        private readonly string _logDirectory;
        private readonly ILogger<DeploymentLogService> _logger;
        private static readonly SemaphoreSlim _fileLock = new(1, 1);
        private const long MaxLogFileSizeBytes = 10 * 1024 * 1024; // 10MB max per log file

        public DeploymentLogService(LogsDbContext context, IConfiguration configuration, ILogger<DeploymentLogService> logger)
        {
            _context = context;
            _logger = logger;
            _logDirectory = configuration["Logs:BasePath"] ?? "/var/log/paas";
            Directory.CreateDirectory(_logDirectory);
        }

        /// <summary>
        /// Appends a log line to the deployment log file (thread-safe and async-safe)
        /// </summary>
        public async Task AppendLogAsync(Guid appInstanceId, string message, CancellationToken cancellationToken)
        {
            var fileName = appInstanceId == Guid.Empty ? "temp.log" : $"{appInstanceId}.log";
            var filePath = Path.Combine(_logDirectory, fileName);
            var logEntry = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} | {message}{Environment.NewLine}";

            await _fileLock.WaitAsync(cancellationToken);
            try
            {
                // Rotate if file too large
                if (File.Exists(filePath))
                {
                    var info = new FileInfo(filePath);
                    if (info.Length > MaxLogFileSizeBytes)
                    {
                        var archiveName = $"{Path.GetFileNameWithoutExtension(filePath)}_{DateTime.UtcNow:yyyyMMddHHmmss}.bak";
                        var archivePath = Path.Combine(_logDirectory, archiveName);
                        File.Move(filePath, archivePath, true);
                        _logger.LogInformation("Rotated log file: {FilePath} → {ArchivePath}", filePath, archivePath);
                    }
                }

                await File.AppendAllTextAsync(filePath, logEntry, cancellationToken);
            }
            catch (IOException ioEx)
            {
                _logger.LogWarning(ioEx, "File lock collision while writing deployment log for {AppInstanceId}", appInstanceId);
                await Task.Delay(50, cancellationToken); // retry after small delay
                await File.AppendAllTextAsync(filePath, logEntry, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing deployment log for {AppInstanceId}", appInstanceId);
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <summary>
        /// Saves deployment metadata (history) to the database.
        /// </summary>
        public async Task SaveHistoryAsync(Guid appInstanceId, string action, string performedBy, CancellationToken cancellationToken)
        {
            var history = new DeploymentHistory(appInstanceId, action, performedBy);
            await _context.DeploymentHistories.AddAsync(history, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<string> GetLogsAsync(Guid appInstanceId, CancellationToken cancellationToken)
        {
            var fileName = appInstanceId == Guid.Empty ? "temp.log" : $"{appInstanceId}.log";
            var filePath = Path.Combine(_logDirectory, fileName);

            if (!File.Exists(filePath))
                return "No logs found.";

            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync(cancellationToken);
        }

        public Task DeleteLogAsync(Guid appInstanceId)
        {
            var fileName = $"{appInstanceId}.log";
            var filePath = Path.Combine(_logDirectory, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("Deleted deployment log for {AppInstanceId}", appInstanceId);
            }

            return Task.CompletedTask;
        }
    }
}
