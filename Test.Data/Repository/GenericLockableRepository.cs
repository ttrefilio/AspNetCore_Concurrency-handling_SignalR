using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Test.Business.Entities;
using Test.Business.Interfaces;
using Test.Data.Concurrency;
using Test.Data.Contexts;

namespace Test.Data.Repository
{
    public class GenericLockableRepository<TEntity> : IGenericLockableRepository<TEntity>
        where TEntity : EntityBase
    {
        protected readonly AppDbContext _context;            
        protected readonly LockRepository _lockManager;   

        public GenericLockableRepository(AppDbContext context, IHttpContextAccessor contextAccessor, LockRepository lockManager)
        {
            _context = context;            
            _lockManager = lockManager;

        }        

        public TEntity GetByIdForEdit(int id, string eventName, string currentUser)
        {
            using (var transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    _lockManager.AcquireLock(id, eventName, currentUser);

                    TEntity obj = _context.Set<TEntity>().Where(t => t.Id == id)
                        .SingleOrDefault();

                    if (obj != null)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        throw new ConcurrencyException("Entity not found. Entity may have been deleted by another user.");
                    }

                    return obj;
                }
                catch (ConcurrencyException ex)
                {
                    transaction.Rollback();
                    string newMessage = ex.Message.Replace("Entity", "Event " + id.ToString("D5"));
                    throw new ConcurrencyException(newMessage);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }        
    }
}
