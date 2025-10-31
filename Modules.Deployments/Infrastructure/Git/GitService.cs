using Modules.Deployments.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Infrastructure.Git
{
    public class GitService : IGitService
    {
        public Task<string> CloneAsync(string repoUrl, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
