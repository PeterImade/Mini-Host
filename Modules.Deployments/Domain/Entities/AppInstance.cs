using Modules.Deployments.Domain.Enums;
using Modules.Deployments.Domain.Exceptions;
using Modules.Deployments.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Domain.Entities
{
    public class AppInstance
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public RepoUrl RepoUrl { get; private set; }
        public Port Port { get; private set; }
        public string ContainerId { get; private set; }
        public DeploymentStatus Status { get; private set; }
        public DateTime DeployedAt { get; private set; }


        private AppInstance() { }

        public AppInstance(RepoUrl repoUrl, Port port)
        {
            RepoUrl = repoUrl;
            Port = port;
            Status = DeploymentStatus.Pending;
            DeployedAt = DateTime.UtcNow;
        }

        public void MarkAsRunning(string containerId)
        {
            if (string.IsNullOrWhiteSpace(containerId))
                throw new DomainException("Container ID cannot be empty.");
            ContainerId = containerId;
            Status = DeploymentStatus.Running;
        }

        public void MarkAsFailed() => Status = DeploymentStatus.Failed;
        public void MarkAsStopped() => Status = DeploymentStatus.Stopped;
        public void MarkAsCompleted() => Status = DeploymentStatus.Completed;
    }
}
