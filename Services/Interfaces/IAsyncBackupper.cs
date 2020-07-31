using System.Threading.Tasks;

namespace WebUI.Services.Interfaces
{
    /// <summary>
    /// Interface for making backups
    /// </summary>
    public interface IAsyncBackupper
    {
        Task<string> MakeBackupAsync();
    }
}