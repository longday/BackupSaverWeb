using System.Threading.Tasks;
using System.Collections.Generic;
using WebUI.Services;

namespace WebUI.Services.Interfaces
{
    /// <summary>
    /// Interface for async saving
    /// </summary>
    public interface IAsyncSaver
    {   
        List<Log> Logs{ get; }

        Task SaveAsync(string source);
    }
}
