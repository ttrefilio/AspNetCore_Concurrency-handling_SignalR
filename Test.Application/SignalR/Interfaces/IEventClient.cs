using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Application.SignalR.Interfaces
{
    public interface IEventClient
    {
        Task ReceiveMessage(string user, string message);
        Task ReceiveUpdate(string message);
    }
}
