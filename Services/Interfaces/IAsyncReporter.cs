using System.Threading.Tasks;
using System.Collections.Generic;
using WebUI.Services;

namespace WebUI.Services.Interfaces
{   
    /// <summary>
    /// Interface for async reporting
    /// </summary>
    public interface IAsyncReporter
    {
        string ConnectionString { get; set; }

        List<Log> Logs{ get; }
        
        Task ReportAsync(string message);
    }
}