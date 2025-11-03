using Microsoft.EntityFrameworkCore.Storage;
using Modules.Deployments.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Application.Interfaces
{
    public interface IDeploymentRepository
    {
        Task<AppInstance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<AppInstance>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(AppInstance app, CancellationToken cancellationToken = default); 
        Task UpdateAsync(AppInstance app, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
