using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Test.Application.Areas.Identity.Data;
using Test.Business.Entities;
using Test.Data.Concurrency;
using Test.Data.Repository;

namespace Test.Application.SignalR.Hubs
{

    public class EventHub : Hub
    {

        private readonly UserManager<User> _userManager;
        private readonly LockRepository _lockManager;        

        public EventHub(UserManager<User> userManager, LockRepository lockManager)
        {
            _userManager = userManager;
            _lockManager = lockManager;
        }


        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("logInfo", $"The user {_userManager.GetUserName(Context.User)} SignalR ID: {Context.ConnectionId} is now connected. (OnConnectedAsync)");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.SendAsync("logInfo", $"The user {_userManager.GetUserName(Context.User)} SignalR ID: {Context.ConnectionId} is now disconnected. (OnDisconnectedAsync)");
            await base.OnDisconnectedAsync(exception);
        }

        public void AddToGroup(string id, string entityName, string groupName)
        {
            //The lock is created before the compiler reaches this point.
            //That's why the necessity of checking the absence of a lock for the user that is joining to the waiting group.
            if (!_lockManager.HasLock(int.Parse(id), entityName, _userManager.GetUserName(Context.User)))
            {
                Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }

        }

        public void RemoveLockPageEvent(string id, string entityName, string groupName)
        {
            string _user = _userManager.GetUserName(Context.User);

            try
            {                
                _lockManager.ReleaseLock(int.Parse(id), entityName, _user);
                Clients.All.SendAsync("updateListLockReleased", new { Id = id });

                Clients.Group(groupName)
                           .SendAsync("informGroupLockReleased", new { Message = $"The user {_user} has finished and realeased the lock. Please, refresh the page." });
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            

        }

    }

}


