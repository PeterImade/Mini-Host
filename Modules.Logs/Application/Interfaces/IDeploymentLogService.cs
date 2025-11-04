using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Logs.Application.Interfaces
{
    public interface IDeploymentLogService
    {
        Task AppendLogAsync(Guid appInstanceId, string message, CancellationToken cancellationToken);
        Task SaveHistoryAsync(Guid appInstanceId, string action, string performedBy, CancellationToken cancellationToken);
        Task<string> GetLogsAsync(Guid appInstanceId, CancellationToken cancellationToken);
        Task DeleteLogAsync(Guid appInstanceId);
    }
}
