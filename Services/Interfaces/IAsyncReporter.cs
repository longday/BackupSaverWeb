using System.Threading.Tasks;

namespace WebUI.Services.Interfaces
{   
    /// <summary>
    /// Interface for async reporting
    /// </summary>
    public interface IAsyncReporter
    {
        Task ReportAsync(string message);
    }
}