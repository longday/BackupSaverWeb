using System.Threading.Tasks;

namespace WebUI.Services.Interfaces
{
    /// <summary>
    /// Interface for async saving
    /// </summary>
    public interface IAsyncSaver
    {
        Task SaveAsync(string source);
    }
}