using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebUI.Services.Interfaces
{   
    /// <summary>
    /// Interface for async reporting
    /// </summary>
    public interface IAsyncReporter
    {
        string ConnectionString { get; set; }

        List<string> Logs{ get; }
        
        Task ReportAsync(string message);
    }
}