using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Exceptions
{
    public class AppException:Exception
    {
        public int Statuscode { get;}
        public AppException(string message ,int statuscode = 500) : base(message)
        {
         Statuscode = statuscode;
        }
    }
}
