using FluentValidation;
using Modules.Deployments.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.API.Validators
{
    public class DeployAppValidator: AbstractValidator<DeployAppRequest>
    {
        public DeployAppValidator() 
        {
            RuleFor(x => x.RepoUrl).NotEmpty().WithMessage("Repository URL is required")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("Repository URL must be a valid URL.");

            RuleFor(x => x.Port)
                .InclusiveBetween(1024, 65535)
                .WithMessage("Port must be between 1024 and 65535.");
        }
    }
}
