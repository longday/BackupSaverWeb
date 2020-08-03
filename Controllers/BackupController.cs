using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sentry;
using WebUI.Services;

namespace WebUI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BackupController : ControllerBase
    {
        private readonly ILogger<BackupController> _logger;
        private readonly BackupSaver _backupSaver;

        public BackupController(ILogger<BackupController> logger, BackupSaver backupSaver)
        {
            _logger = logger;
            _backupSaver = backupSaver;
        }

        [HttpGet]
        public async Task MakeBackup()
        {
            string sentryConnectionString = Environment.GetEnvironmentVariable("SENTRY_CONNECTION_STRING");

            _logger.LogInformation("Start pg_dump...");

            using(SentrySdk.Init(sentryConnectionString))
            {
                try
                {
                    int backupDeletionPeriodInDays = int.Parse(Environment.GetEnvironmentVariable("BACKUP_DELETION_PERIOD_IN_DAYS") ?? throw new ArgumentNullException());
                    string message = $"Items uploaded earlier than in the last {backupDeletionPeriodInDays} days have been removed";

                    await _backupSaver.MakeBackupsAsync(backupDeletionPeriodInDays, message);
        
                }
                catch(Exception ex)
                {
                    SentrySdk.CaptureException(ex);
                    SentrySdk.CaptureMessage("BackupSaver completed work with error!");
                    _logger.LogError("BackupSaver completed work with error!");

                    await Task.FromException<Exception>(ex);
                }
            }

            _logger.LogInformation("Successfully");
        }
    }
}

