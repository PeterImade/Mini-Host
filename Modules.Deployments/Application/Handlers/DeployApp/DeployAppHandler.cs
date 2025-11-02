using Microsoft.Extensions.Configuration;
using Modules.Deployments.Application.Interfaces;
using Modules.Deployments.Domain.Entities;
using Modules.Deployments.Domain.Exceptions;
using Modules.Deployments.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Application.Commands.DeployApp
{
    public class DeployAppHandler: IDeployAppHandler
    {
        private readonly IDeploymentRepository _deploymentRepository;
        private readonly IDockerManager _dockerManager;
        private readonly IGitService _gitService;
        private readonly INginxManager _nginxManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRuntimeDetector _runtimeDetector;
        private readonly string _baseDomain;

        public DeployAppHandler(IDeploymentRepository deploymentRepository, IDockerManager dockerManager, IGitService gitService, INginxManager nginxManager, IUnitOfWork unitOfWork, IRuntimeDetector runtimeDetector, IConfiguration configuration)
        {
            _deploymentRepository = deploymentRepository;
            _dockerManager = dockerManager;
            _gitService = gitService;
            _nginxManager = nginxManager;
            _unitOfWork = unitOfWork;
            _runtimeDetector = runtimeDetector;
            _baseDomain = configuration["Deployment:BaseDomain"]!;
        }

        public async Task<AppInstance> HandleAsync(string repoUrl, int port, CancellationToken cancellationToken)
        {
            string? containerId = null;
            string? localPath = null;
            string? domain = null;

            try
            {
                // 1️⃣ Clone repo
                localPath = await _gitService.CloneAsync(new RepoUrl(repoUrl), cancellationToken);

                // 2️⃣ Ensure Dockerfile exists
                _runtimeDetector.EnsureDockerfileExists(localPath);

                // 3️⃣ Build + run container
                containerId = await _dockerManager.BuildAndRunContainerAsync(localPath, new Port(port), cancellationToken);

                // 4️⃣ Generate domain (e.g. app-12ab34.yourpaas.com)
                var slug = Guid.NewGuid().ToString("N")[..6];
                domain = $"app-{slug}.{_baseDomain}";

                // 5️⃣ Configure Nginx
                await _nginxManager.ConfigureReverseProxyAsync(containerId, domain, new Port(port), cancellationToken);

                // 6️⃣ Save deployment
                var instance = new AppInstance(new RepoUrl(repoUrl), new Port(port));
                instance.AssignDomain(domain);
                instance.MarkAsRunning(containerId);
                await _deploymentRepository.AddAsync(instance, cancellationToken);

                Console.WriteLine($"✅ Deployed {repoUrl} → {domain} (port {port})");

                return instance;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Deployment failed: {ex.Message}");

                // Stop container
                if (!string.IsNullOrEmpty(containerId))
                {
                    try { await _dockerManager.StopAndRemoveContainerAsync(containerId, cancellationToken); } catch { }
                }

                // Delete repo folder
                if (!string.IsNullOrEmpty(localPath) && Directory.Exists(localPath))
                {
                    try { Directory.Delete(localPath, true); } catch { }
                }

                // Remove nginx config
                if (!string.IsNullOrEmpty(domain))
                {
                    try { await _nginxManager.RemoveReverseProxyConfigAsync(domain, cancellationToken); } catch { }
                }

                throw new DomainException($"Deployment failed and was rolled back: {ex.Message}");
            }
        }
    }
}
