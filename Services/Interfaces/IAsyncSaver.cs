using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebUI.Services.Interfaces
{
    /// <summary>
    /// Interface for async saving
    /// </summary>
    public interface IAsyncSaver
    {   
        List<string> Logs{ get; }

        Task SaveAsync(string source);
    }
}
