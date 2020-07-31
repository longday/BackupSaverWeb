using System;

namespace WebUI.Services
{
    public class FileIsEmptyException : Exception
    {
        public FileIsEmptyException()
        {
                
        }

        public FileIsEmptyException(string message) : base(message)
        {
            
        }
    }
}