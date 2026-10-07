using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
    internal class ValidationException : AppException
    {
        public ValidationException(string message, int statuscode = 500) : base(message, 400)
        {
        }
    }
}
