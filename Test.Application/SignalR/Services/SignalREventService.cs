using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.Application.DTOs;
using Test.Application.SignalR.Hubs;

namespace Test.Application.SignalR.Services
{    
    public class SignalREventService
    {
        private readonly IHubContext<EventHub> _hubContext;

        public SignalREventService(IHubContext<EventHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void InformLockReleased(string groupName, int id, string user)
        {
            _hubContext.Clients.All.SendAsync("updateListLockReleased", new { Id = id });
            
            _hubContext.Clients.Group(groupName)
                       .SendAsync("informGroupLockReleased", new { Message = $"The user {user} has finished and realeased the lock. Please, refresh the page." });
        }

        public void InformLockAcquired(int id)
        {
            _hubContext.Clients.All.SendAsync("updateListLockAcquired", new { Id = id });
        }
        
    }
}
