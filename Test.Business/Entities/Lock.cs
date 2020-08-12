using System;


namespace Test.Business.Entities
{
    public class Lock
    {
        public int EntityId { get; set; }
        public string ObjectName { get; set; }
        public string OwnerId { get; set; }
        public DateTime AcquiredDateTime { get; set; }

    }
}
