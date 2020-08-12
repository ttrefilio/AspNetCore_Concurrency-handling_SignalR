using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.Business.Interfaces
{
    public interface IGenericLockableRepository<TEntity>
        where TEntity : IEntityBase
    {        
        TEntity GetByIdForEdit(int id, string eventName, string currentUser);
    }
}
