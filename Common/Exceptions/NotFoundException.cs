using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
    internal class NotFoundException : AppException
    {
        public NotFoundException(string message, int statuscode = 500) : base(message, 404)
        {
        }
    }
}
