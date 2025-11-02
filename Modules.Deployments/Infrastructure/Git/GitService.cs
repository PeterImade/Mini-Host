using LibGit2Sharp;
using Modules.Deployments.Application.Interfaces;
using Modules.Deployments.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Infrastructure.Git
{
    public class GitService: IGitService
    {
        private readonly string _baseCloneDir;

        public GitService(string baseCloneDir = "/tmp/clones")
        {
            _baseCloneDir = baseCloneDir;
            Directory.CreateDirectory(_baseCloneDir);
        }
        public async Task<string> CloneAsync(RepoUrl repoUrl, CancellationToken cancellationToken)
        {
            var repoName = Path.GetFileNameWithoutExtension(repoUrl);
            var targetPath = Path.Combine(_baseCloneDir, $"{repoName}_{Guid.NewGuid()}");

            return await Task.Run(() =>
            {
                Repository.Clone(repoUrl, targetPath);
                return targetPath;
            }, cancellationToken);
        }
    }
}
