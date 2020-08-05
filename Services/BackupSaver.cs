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
            _backupper = backupper ?? throw new ArgumentNullException(nameof(backupper));
            _remover = remover ?? throw new ArgumentNullException(nameof(remover));
            _saver = saver ?? throw new ArgumentNullException(nameof(saver));
            _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
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
