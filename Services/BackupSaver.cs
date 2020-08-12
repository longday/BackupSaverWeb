using System.Threading.Tasks;
using WebUI.Services.Interfaces;
using System.Collections.Generic;
using System;
using Sentry;

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
        public List<Log> Logs{ get; }

        public BackupSaver(IAsyncBackupper backupper, IAsyncRemover remover, 
                       IAsyncSaver saver, IAsyncReporter reporter)
        {
            _backupper = backupper ?? throw new ArgumentNullException(nameof(backupper));
            _remover = remover ?? throw new ArgumentNullException(nameof(remover));
            _saver = saver ?? throw new ArgumentNullException(nameof(saver));
            _reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
            Logs = new List<Log>();
        }

        public async Task MakeBackupsAsync()
        {
            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Start pg_dump..."));

            string sentryConnectionString = Environment.GetEnvironmentVariable("SENTRY_CONNECTION_STRING");

            using(SentrySdk.Init(sentryConnectionString))
            {
                try
                {
                    string backupArchivePath = await _backupper.MakeBackupAsync();

                    int backupDeletionPeriodInDays = int.Parse(Environment.GetEnvironmentVariable("FILE_DELETION_PERIOD_IN_DAYS") ?? throw new ArgumentNullException());
                    string message = $"Items uploaded earlier than in the last {backupDeletionPeriodInDays} days have been removed." +
                                            "Backups were archived and saved in AmazonS3...";

                    Task removeTask = _remover.RemoveAsync(backupDeletionPeriodInDays);
                    Task saveTask = _saver.SaveAsync(backupArchivePath);
                    Task reportTask = _reporter.ReportAsync(message);

                    await Task.WhenAll(removeTask, saveTask, reportTask);

                    AddServicesLogs();

                }
                catch (Exception ex)
                {
                    AddServicesLogs();
                    Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: BackupSaver completed work with error: {ex.Message}..."));
                    SentrySdk.CaptureException(ex);
                }
            }
        }

        private void AddServicesLogs()
        {
            Logs.AddRange(_backupper.Logs);
            Logs.AddRange(_saver.Logs);
            Logs.AddRange(_reporter.Logs);
        }
    }
}
