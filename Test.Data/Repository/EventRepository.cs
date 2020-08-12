using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Test.Business.Entities;
using Test.Data.Concurrency;
using Test.Data.Contexts;

namespace Test.Data.Repository
{
    public class EventRepository : GenericLockableRepository<Event>
    {              

        public EventRepository(AppDbContext context, IHttpContextAccessor contextAccessor, LockRepository lockManager) : base(context, contextAccessor, lockManager)
        {
            
        }

        public List<Event> GetAll()
        {
            return _context.Events.ToList();
        }

        public Event GetEventById(int id)
        {
            
            return _context.Events.FirstOrDefault(e => e.Id == id);
        }        

        public void Add(Event obj)
        {
            _context.Events.Add(obj);
            _context.SaveChanges();
        }

        public void Update(Event obj, string currentUser)
        {
            using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    if (_lockManager.HasLock(obj.Id, "Event", currentUser))
                    {
                        _context.Events.Update(obj);
                        _context.SaveChanges();

                        _lockManager.ReleaseLock(obj.Id, "Event", currentUser);
                        transaction.Commit();
                    }
                    else
                    {
                        throw new ConcurrencyException("ConnectionId does not have a lock on Entity. "
                            + "This may be due to a timeout. "
                            + "Please reload record and restart editing to prevent overwriting another user's changes.");
                    }
                }
                catch (ConcurrencyException ex)
                {

                    transaction.Rollback();
                    string newMessage = ex.Message.Replace("Entity", "Event " + obj.Id.ToString());
                    throw new ConcurrencyException(newMessage);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }         

        }

        public bool Delete(int id, string currentUser)
        {
            using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    if (_lockManager.HasLock(id, "Event", currentUser))
                    {
                        var obj = _context.Events
                            .Where(e => e.Id == id)
                            .FirstOrDefault();

                        _context.Events.Remove(obj);                        

                        _lockManager.ReleaseLock(obj.Id, "Event", currentUser);

                        _context.SaveChanges();

                        transaction.Commit();

                        return true;
                    }
                    else
                    {
                        throw new ConcurrencyException("ConnectionId does not have a lock on Entity. "
                            + "This may be due to a timeout. "
                            + "Please reload record and restart editing to prevent overwriting another user's changes.");
                    }
                }
                catch (ConcurrencyException ex)
                {

                    transaction.Rollback();
                    string newMessage = ex.Message.Replace("Entity", "Event " + id.ToString());
                    throw new ConcurrencyException(newMessage);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

        }

        public bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
        
    }
}
