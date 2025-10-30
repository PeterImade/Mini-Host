using Modules.Deployments.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Domain.ValueObjects
{ 
        public sealed class Port
        {
            public int Value { get; }

            public Port(int value)
            {
                if (value < 1024 || value > 65535)
                    throw new InvalidPortException(value);
                Value = value;
            }

            public static implicit operator int(Port port) => port.Value;
    }
}
