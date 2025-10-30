using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Domain.Exceptions
{
    public class InvalidPortException: Exception
    {
        public InvalidPortException(int port): base($"Invalid port: {port}") { }
    }
}
