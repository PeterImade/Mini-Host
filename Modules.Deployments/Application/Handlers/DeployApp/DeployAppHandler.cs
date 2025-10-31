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
    public class DeployAppCommandHandler
    {
        private readonly IDeploymentRepository _deploymentRepository;
        private readonly IDockerManager _dockerManager;
        private readonly IGitService _gitService;
        private readonly INginxManager _nginxManager;
        private readonly IUnitOfWork _unitOfWork;

        public DeployAppCommandHandler(IDeploymentRepository deploymentRepository, IDockerManager dockerManager, IGitService gitService, INginxManager nginxManager, IUnitOfWork unitOfWork)
        {
            _deploymentRepository = deploymentRepository;
            _dockerManager = dockerManager;
            _gitService = gitService;
            _nginxManager = nginxManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<AppInstance> HandleAsync(string repoUrl, int port, CancellationToken cancellationToken)
        {
            var repo = new RepoUrl(repoUrl);
            var validPort = new Port(port);

            string? containerId = null;
            string? localPath = null;

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                localPath = await _gitService.CloneAsync(repo, cancellationToken);
                containerId = await _dockerManager.BuildAndRunContainerAsync(localPath, validPort, cancellationToken);
                await _nginxManager.ConfigureReverseProxyAsync(containerId, validPort, cancellationToken);


                var appInstance = new AppInstance(repo, validPort);
                appInstance.MarkAsRunning(containerId);

                await _deploymentRepository.AddAsync(appInstance, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);   

                return appInstance;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);

                if(!string.IsNullOrEmpty(containerId))
                    await _dockerManager.StopAndRemoveContainerAsync(containerId, cancellationToken);

                if (!string.IsNullOrEmpty(localPath) && Directory.Exists(localPath))
                    Directory.Delete(localPath, true);

                // optional: placeholder for Nginx cleanup
                // await _nginxManager.RemoveReverseProxyConfigAsync(validPort, cancellationToken);

                throw new DomainException($"Deployment failed and rolled back: {ex.Message}");

            } 
        }
    }
}
