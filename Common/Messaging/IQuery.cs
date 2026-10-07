using Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Messaging
{
    public interface IQuery<Tresponse>:IRequest<ResultT<Tresponse>>
    {
    }
}
