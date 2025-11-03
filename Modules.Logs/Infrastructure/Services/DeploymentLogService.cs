using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Modules.Deployments.Domain.Entities;
using Modules.Logs.Application;
using Modules.Logs.Application.Interfaces; 
using Modules.Logs.Persistence.Context;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Modules.Logs.Infrastructure.Services
{
    public class DeploymentLogService : IDeploymentLogService
    {
        private readonly LogsDbContext _context;
        private readonly string _logDirectory;

        public DeploymentLogService(LogsDbContext context, IConfiguration configuration)
        {
            _context = context;
            _logDirectory = configuration["Logs:BasePath"] ?? "/var/log/paas";
            Directory.CreateDirectory(_logDirectory);
        }

        public async Task AppendLogAsync(Guid appInstanceId, string message, CancellationToken cancellationToken)
        {
            var fileName = appInstanceId == Guid.Empty
                ? "temp.log"
                : $"{appInstanceId}.log";

            var filePath = Path.Combine(_logDirectory, fileName);
            var logEntry = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} | {message}{Environment.NewLine}";

            await File.AppendAllTextAsync(filePath, logEntry, cancellationToken);
        }

        public async Task SaveHistoryAsync(Guid appInstanceId, string action, string performedBy, CancellationToken cancellationToken)
        {
            var history = new DeploymentHistory(appInstanceId, action, performedBy);
            await _context.DeploymentHistories.AddAsync(history, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
