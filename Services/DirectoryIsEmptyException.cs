using System;

namespace WebUI.Services 
{
    public class DirectoryIsEmptyException : Exception
    {
        public DirectoryIsEmptyException()
        {
            
        }

        public DirectoryIsEmptyException(string message) : base(message)
        {
            
        }
    }
}