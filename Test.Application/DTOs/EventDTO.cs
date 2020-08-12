using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.Business.Interfaces;

namespace Test.Application.DTOs
{
    public class EventDTO :ILockable
    {
        public int Id { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }

        public bool IsLocked { get; set; }
        public bool OwnsTheLock { get; set; }
    }
}
