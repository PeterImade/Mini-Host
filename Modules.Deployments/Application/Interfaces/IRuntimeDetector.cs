using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Application.Interfaces
{
    public interface IRuntimeDetector
    {
        string? DetectRuntime(string repoPath);
        void EnsureDockerfileExists(string repoPath);
    }
}
