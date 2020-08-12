using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Test.Business.Entities;
using Test.Data.Contexts;

namespace Test.Data.Concurrency
{
    public class LockRepository
    {
        private readonly int _lockExpirySeconds = 15;

        private readonly AppDbContext _dbContext;

        public LockRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Lock> GetAllLocks()
        {
            return _dbContext.Locks.ToList();
        }
        
        public void AcquireLock(int id, string entityName, string owner)
        {            
            RemoveExpiredLock(id, entityName);            

            if(!HasLock(id, entityName, owner))
            {
                try
                {
                    Lock _lock = new Lock
                    {
                        EntityId = id,
                        ObjectName = entityName,
                        OwnerId = owner,
                        AcquiredDateTime = DateTime.Now
                    };

                    _dbContext.Locks.Add(_lock);
                    _dbContext.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    throw new ConcurrencyException("Entity is locked by another user and cannot be edited at this time.");
                }
            }
        }

        public void ReleaseLock(int id, string entityName, string owner)
        {
            if(HasLock(id, entityName, owner))
            {
                Lock _lock = _dbContext.Locks
                    .FirstOrDefault(l => l.EntityId == id && l.ObjectName == entityName && l.OwnerId == owner);

                if(_lock != null)
                {
                    try
                    {
                        _dbContext.Locks.Remove(_lock);
                        _dbContext.SaveChanges();
                        
                    }
                    catch (Exception)
                    {
                        throw new ConcurrencyException("Unexpected error releasing lock on Entity.");                        
                    }
                }
            }
        }

        public bool HasLock(int id, string entityName, string owner)
        {
            Lock _lock = _dbContext.Locks
                .AsNoTracking()
                .FirstOrDefault(l => l.EntityId == id && l.ObjectName == entityName && l.OwnerId == owner);
            return _lock != null ? true : false;

        }

        public void ReleaseAllLocks(string owner)
        {
            Lock[] _locks = _dbContext.Locks
                .Where(l => l.OwnerId == owner).ToArray();

            if(_locks != null)
            {
                foreach(var _lock in _locks)
                {
                    _dbContext.Locks.Remove(_lock);
                }
                _dbContext.SaveChanges();
            }
        }

        private void RemoveExpiredLock(int id, string entityName)
        {
            Lock _lock = _dbContext.Locks
                .Where(l => l.EntityId == id && l.ObjectName == entityName)
                .AsNoTracking()
                .FirstOrDefault();

            if(_lock != null && _lock.AcquiredDateTime <= DateTime.Now.AddSeconds(-_lockExpirySeconds))
            {
                _dbContext.Locks.Remove(_lock);
                _dbContext.SaveChanges();
            }

        }

        public void RenewLock(int id, string entityName, string currentUser)
        {
            Lock _lock = _dbContext.Locks
                .Where(l => l.EntityId == id && l.ObjectName == entityName)
                .AsNoTracking()
                .FirstOrDefault();

            if(_lock != null && _lock.OwnerId == currentUser)
            {
                if(_lock.AcquiredDateTime <= DateTime.Now.AddSeconds(-_lockExpirySeconds))
                {
                    throw new ConcurrencyException("Lock on Entity has expired. Please restart Business Transaction.");
                }

                _lock.AcquiredDateTime = DateTime.Now;
                _dbContext.SaveChanges();
            }
            else
            {
                throw new ConcurrencyException("Lock not found for user and Entity");
            }
        }
    }
}
