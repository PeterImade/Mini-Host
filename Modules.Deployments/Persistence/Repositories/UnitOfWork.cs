using Microsoft.EntityFrameworkCore.Storage;
using Modules.Deployments.Application.Interfaces;
using Modules.Deployments.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Persistence.Repositories
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly DeploymentsDbContext _deploymentsDbContext;

        public UnitOfWork(DeploymentsDbContext deploymentsDbContext)
        {
            _deploymentsDbContext = deploymentsDbContext;
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return await _deploymentsDbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _deploymentsDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
