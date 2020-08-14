using System;

namespace WebUI.Services
{
    public class DbNotFoundException : Exception
    {
        public DbNotFoundException()
        {
            
        }

        public DbNotFoundException(string message) : base(message)
        {
            
        }
    }
}