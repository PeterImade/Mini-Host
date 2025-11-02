using Modules.Deployments.Application.Interfaces;
using Modules.Deployments.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Infrastructure.Nginx
{
    public class NginxManager: INginxManager
    {
        private readonly string _sitesAvailable = "/etc/nginx/sites-available";
        private readonly string _sitesEnabled = "/etc/nginx/sites-enabled";

        public async Task ConfigureReverseProxyAsync(string containerId, string domain, Port port, CancellationToken cancellationToken)
        {
            var configName = $"{domain}.conf";
            var configPath = Path.Combine(_sitesAvailable, configName);

            var configContent = $@"
            server {{
                listen 80;
                server_name {domain};

                location / {{
                    proxy_pass http://localhost:{port.Value};
                    proxy_set_header Host $host;
                    proxy_set_header X-Real-IP $remote_addr;
                    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
                    proxy_set_header X-Forwarded-Proto $scheme;
                }}
            }}
";

            await File.WriteAllTextAsync(configPath, configContent, cancellationToken);

            var enabledPath = Path.Combine(_sitesEnabled, configName);
            if (!File.Exists(enabledPath))
                File.CreateSymbolicLink(enabledPath, configPath);

            await ReloadNginxAsync(cancellationToken);
        }

        public async Task RemoveReverseProxyConfigAsync(string domain, CancellationToken cancellationToken)
        {
            var configName = $"{domain}.conf";
            var configAvailable = Path.Combine(_sitesAvailable, configName);
            var configEnabled = Path.Combine(_sitesEnabled, configName);

            if (File.Exists(configEnabled))
                File.Delete(configEnabled);

            if (File.Exists(configAvailable))
                File.Delete(configAvailable);

            await ReloadNginxAsync(cancellationToken);
        }

        private async Task ReloadNginxAsync(CancellationToken cancellationToken)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "nginx",
                Arguments = "-s reload",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            await process.WaitForExitAsync(cancellationToken);

            if (process.ExitCode != 0)
            {
                var error = await process.StandardError.ReadToEndAsync();
                throw new Exception($"Nginx reload failed: {error}");
            }
        }
    }
}