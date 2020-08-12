using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Business.Interfaces
{
    public interface ILockable
    {
        bool IsLocked { get; set; }
        bool OwnsTheLock { get; set; }
    }
}
