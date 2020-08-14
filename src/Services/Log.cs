using System.Collections.Generic;
using System;

namespace WebUI.Services
{
    public class Log
    {
        public DateTime Date{ get; }

        public string Message{ get; }

        public Log(DateTime date, string message)
        {
            Date = date;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}
