
using System;
using System.Collections.Generic;
using System.Text;
using Common.Results;
using MediatR;

namespace Common.Messaging
{
    public class Icommand:IRequest<Result>
    {
    }
    public class Icommand<Tresponse>:IRequest<ResultT<Tresponse>> { }
}
