using System.Threading.Tasks;

namespace WebUI.Services.Interfaces
{
    /// <summary>
    /// Interface for removing
    /// </summary>
    public interface IAsyncRemover
    { 
        Task RemoveAsync(int quantity);
    }
}
