using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Data.Concurrency
{
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException() 
        { 
        }

        public ConcurrencyException(string message) : base(message)
        {
        }
    }
}
