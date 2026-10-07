using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
    internal class ForbiddenException : AppException
    {
        public ForbiddenException(string message, int statuscode = 500) : base(message, 403)
        {
        }
    }
}
