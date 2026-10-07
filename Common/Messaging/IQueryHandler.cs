using Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Messaging
{
    internal interface IQueryHandler<TQuery,Tresponse>:IRequestHandler<TQuery,ResultT<Tresponse>>
        where TQuery : IQuery<Tresponse>
    {
    }
}
