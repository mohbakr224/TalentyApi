using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
    internal class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message, int statuscode = 500) : base(message, 401)
        {
        }
    }
}
