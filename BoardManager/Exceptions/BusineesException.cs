using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardManager.Exceptions
{
    public class BusineesException : Exception
    {
        public BusineesException(string message) : base(message)
        {
        }
    }
}
