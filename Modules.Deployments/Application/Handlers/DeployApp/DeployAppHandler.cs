using Microsoft.Extensions.Configuration;
using Modules.Deployments.Application.Interfaces;
using Modules.Deployments.Domain.Entities;
using Modules.Deployments.Domain.Exceptions;
using Modules.Deployments.Domain.ValueObjects;
using Modules.Logs.Application;
using Modules.Logs.Application.Interfaces;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Modules.Deployments.Application.Commands.DeployApp
{
    public class DeployAppHandler : IDeployAppHandler
    {
        private readonly IDeploymentRepository _deploymentRepository;
        private readonly IDockerManager _dockerManager;
        private readonly IGitService _gitService;
        private readonly INginxManager _nginxManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRuntimeDetector _runtimeDetector;
        private readonly IDeploymentLogService _logService;
        private readonly string _baseDomain;

        public DeployAppHandler(
            IDeploymentRepository deploymentRepository,
            IDockerManager dockerManager,
            IGitService gitService,
            INginxManager nginxManager,
            IUnitOfWork unitOfWork,
            IRuntimeDetector runtimeDetector,
            IDeploymentLogService logService,
            IConfiguration configuration)
        {
            _deploymentRepository = deploymentRepository;
            _dockerManager = dockerManager;
            _gitService = gitService;
            _nginxManager = nginxManager;
            _unitOfWork = unitOfWork;
            _runtimeDetector = runtimeDetector;
            _logService = logService;
            _baseDomain = configuration["Deployment:BaseDomain"] ?? "yourpaas.com";
        }

        public async Task<AppInstance> HandleAsync(string repoUrl, int port, CancellationToken cancellationToken)
        {
            string? containerId = null;
            string? localPath = null;
            string? domain = null;

            try
            {
                await _logService.AppendLogAsync(Guid.Empty, "🚀 Starting deployment...", cancellationToken);

                // 1️⃣ Clone repo
                await _logService.AppendLogAsync(Guid.Empty, "Cloning repository...", cancellationToken);
                localPath = await _gitService.CloneAsync(new RepoUrl(repoUrl), cancellationToken);

                // 2️⃣ Ensure Dockerfile exists
                await _logService.AppendLogAsync(Guid.Empty, "Checking for Dockerfile...", cancellationToken);
                _runtimeDetector.EnsureDockerfileExists(localPath);

                // 3️⃣ Build + run container
                await _logService.AppendLogAsync(Guid.Empty, "Building Docker image and starting container...", cancellationToken);
                containerId = await _dockerManager.BuildAndRunContainerAsync(localPath, new Port(port), cancellationToken);

                // 4️⃣ Generate domain
                var slug = Guid.NewGuid().ToString("N")[..6];
                domain = $"app-{slug}.{_baseDomain}";
                await _logService.AppendLogAsync(Guid.Empty, $"Generated domain: {domain}", cancellationToken);

                // 5️⃣ Configure Nginx
                await _logService.AppendLogAsync(Guid.Empty, "Configuring Nginx reverse proxy...", cancellationToken);
                await _nginxManager.ConfigureReverseProxyAsync(containerId, domain, new Port(port), cancellationToken);

                // 6️⃣ Save deployment
                var appInstance = new AppInstance(new RepoUrl(repoUrl), new Port(port));
                appInstance.AssignDomain(domain);
                appInstance.MarkAsRunning(containerId);
                await _deploymentRepository.AddAsync(appInstance, cancellationToken);

                // 7️⃣ Write final logs to file + DB history
                await _logService.AppendLogAsync(appInstance.Id, $"✅ Deployment successful! App running at http://{domain}", cancellationToken);
                await _logService.SaveHistoryAsync(appInstance.Id, "Deployed", "system", cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                Console.WriteLine($"✅ Deployment successful → {domain}");
                return appInstance;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Deployment failed: {ex.Message}");
                await _logService.AppendLogAsync(Guid.Empty, $"❌ Deployment failed: {ex.Message}", cancellationToken);

                // Stop container
                if (!string.IsNullOrEmpty(containerId))
                {
                    try
                    {
                        await _dockerManager.StopAndRemoveContainerAsync(containerId, cancellationToken);
                        await _logService.AppendLogAsync(Guid.Empty, "🧹 Cleaned up container.", cancellationToken);
                    }
                    catch { }
                }

                // Delete repo folder
                if (!string.IsNullOrEmpty(localPath) && Directory.Exists(localPath))
                {
                    try
                    {
                        Directory.Delete(localPath, true);
                        await _logService.AppendLogAsync(Guid.Empty, "🧹 Deleted cloned repo.", cancellationToken);
                    }
                    catch { }
                }

                // Remove nginx config
                if (!string.IsNullOrEmpty(domain))
                {
                    try
                    {
                        await _nginxManager.RemoveReverseProxyConfigAsync(domain, cancellationToken);
                        await _logService.AppendLogAsync(Guid.Empty, "🧹 Removed Nginx config.", cancellationToken);
                    }
                    catch { }
                }

                throw new DomainException($"Deployment failed and was rolled back: {ex.Message}");
            }
        }
    }
}
