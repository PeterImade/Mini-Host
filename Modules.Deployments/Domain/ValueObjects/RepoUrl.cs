using Modules.Deployments.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Domain.ValueObjects
{
    public sealed class RepoUrl
    {
        public string Value { get; }

        public RepoUrl(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !Uri.IsWellFormedUriString(value, UriKind.Absolute))
                throw new InvalidRepoUrlException("Invalid repository URL.");
            Value = value.Trim();
        }

        public override string ToString() => Value;
        public static implicit operator string(RepoUrl repoUrl) => repoUrl.Value;
    }
}
