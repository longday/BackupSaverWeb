using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebUI.Services.Interfaces
{
    /// <summary>
    /// Interface for making backups
    /// </summary>
    public interface IAsyncBackupper
    {
        List<string> Logs{ get; }

        string DbList { get; set; }
        
        Task<string> MakeBackupAsync();
    }
}
