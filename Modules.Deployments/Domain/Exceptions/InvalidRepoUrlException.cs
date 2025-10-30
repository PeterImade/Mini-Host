using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Domain.Exceptions
{
    public class InvalidRepoUrlException: Exception
    {
        public InvalidRepoUrlException(string message): base(message)
        {
            
        }
    }
}
