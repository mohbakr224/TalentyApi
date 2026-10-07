using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
    public class ConflictException : AppException
    {
        public ConflictException(string message, int statuscode = 500) : base(message, 409)
        {
        }
    }
}
