using System;
using System.Collections.Generic;
using System.Text;
using Test.Business.Interfaces;

namespace Test.Business.Entities
{
    public abstract class EntityBase : IEntityBase
    {
        public int Id { get; set; }
    }
}
