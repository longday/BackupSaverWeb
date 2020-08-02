using System.Threading.Tasks;
using WebUI.Services.Interfaces;

namespace WebUI.Services
{
    ///<summary>
    ///Facade over WebUI.Services API
    ///</summary>
    public sealed class BackupSaver
    {
        private readonly IAsyncBackupper _backupper;
        private readonly IAsyncRemover _remover;
        private readonly IAsyncSaver _saver;
        private readonly IAsyncReporter _reporter;

        public BackupSaver(IAsyncBackupper backupper, IAsyncRemover remover, 
                       IAsyncSaver saver, IAsyncReporter reporter)
        {
            _backupper = backupper;
            _remover = remover;
            _saver = saver;
            _reporter = reporter;
        }

        public async Task MakeBackupsAsync(int quantity, string message)
        {
             string backupArchivePath = await _backupper.MakeBackupAsync();

             Task removeTask = _remover.RemoveAsync(quantity);
             Task saveTask = _saver.SaveAsync(backupArchivePath);
             Task reportTask = _reporter.ReportAsync(message);

             await Task.WhenAll(removeTask, saveTask, reportTask);
        }
    }
}