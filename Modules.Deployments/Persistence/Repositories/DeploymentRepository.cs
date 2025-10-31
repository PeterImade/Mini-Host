using Microsoft.EntityFrameworkCore;
using Modules.Deployments.Application.Interfaces;
using Modules.Deployments.Domain.Entities;
using Modules.Deployments.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Persistence.Repositories
{
    public class DeploymentRepository: IDeploymentRepository
    {
        private readonly DeploymentsDbContext _dbContext;

        public DeploymentRepository(DeploymentsDbContext dbContext, CancellationToken cancellationToken = default) => _dbContext = dbContext;
        public async Task AddAsync(AppInstance app, CancellationToken cancellationToken = default) =>
            await _dbContext.AppInstances.AddAsync(app, cancellationToken);

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var appInstance = await _dbContext.AppInstances.FindAsync(id, cancellationToken);

            if (appInstance is not null)
                _dbContext.AppInstances.Remove(appInstance);
        }

        public async Task<IEnumerable<AppInstance>> GetAllAsync(CancellationToken cancellationToken = default) =>
            await _dbContext.AppInstances.AsNoTracking().ToListAsync(cancellationToken);

        public async Task<AppInstance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            await _dbContext.AppInstances.FindAsync(id, cancellationToken);

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default) => await _dbContext.SaveChangesAsync(cancellationToken);

        public async Task UpdateAsync(AppInstance app, CancellationToken cancellationToken = default) =>
           _dbContext.AppInstances.Update(app);
    }
}
