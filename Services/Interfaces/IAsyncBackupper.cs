using System.Threading.Tasks;
using System.Collections.Generic;
using WebUI.Services;

namespace WebUI.Services.Interfaces
{
    /// <summary>
    /// Interface for making backups
    /// </summary>
    public interface IAsyncBackupper
    {
        List<Log> Logs{ get; }

        string DbList { get; set; }
        
        Task<string> MakeBackupAsync();
    }
}
