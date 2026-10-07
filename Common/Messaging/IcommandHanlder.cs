using Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Messaging
{
    public interface IcommandHandler<TCommand, Tresponse> : IRequestHandler<TCommand, ResultT<Tresponse>>
         where TCommand : Icommand<Tresponse>
    { }

    public interface IcommandHandler<TCommand> : IRequestHandler<TCommand,Result>
        where TCommand : Icommand
    { }

    }
