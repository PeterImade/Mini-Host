using Modules.Deployments.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Infrastructure.Docker
{
    public class RuntimeDetector: IRuntimeDetector
    {
        public string? DetectRuntime(string repoPath)
        {
            if (File.Exists(Path.Combine(repoPath, "package.json")))
                return "node";

            if (File.Exists(Path.Combine(repoPath, "requirements.txt")) ||
                File.Exists(Path.Combine(repoPath, "pyproject.toml")))
                return "python";

            if (Directory.GetFiles(repoPath, "*.csproj", SearchOption.AllDirectories).Any())
                return "dotnet";

            if (File.Exists(Path.Combine(repoPath, "main.go")))
                return "go";

            return null;
        }

        public void EnsureDockerfileExists(string repoPath)
        {
            var dockerfilePath = Path.Combine(repoPath, "Dockerfile");
            if (File.Exists(dockerfilePath))
                return; // already has one

            var runtime = DetectRuntime(repoPath);
            if (runtime == null)
                throw new Exception("Could not detect runtime. Please include a Dockerfile manually.");

            var content = DockerfileTemplates.GetTemplateFor(runtime);
            File.WriteAllText(dockerfilePath, content);
        }
    }
}
