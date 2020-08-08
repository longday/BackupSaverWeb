using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Sentry;
using WebUI.Services;
using System.Linq;

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
        public async Task<Log[]> MakeBackup()
        {
            string sentryConnectionString = Environment.GetEnvironmentVariable("SENTRY_CONNECTION_STRING");

            _logger.LogInformation($"{DateTime.Now}: Start pg_dump...");

            using(SentrySdk.Init(sentryConnectionString))
            {
                try
                {
                    int backupDeletionPeriodInDays= int.Parse(Environment.GetEnvironmentVariable("FILE_DELETION_PERIOD_IN_DAYS") ?? throw new ArgumentNullException());
                    string message = $"Items uploaded earlier than in the last {backupDeletionPeriodInDays} days have been removed." +
                                        "Backups were archived and saved in AmazonS3...";

                    await _backupSaver.MakeBackupsAsync(backupDeletionPeriodInDays, message);
                }
                catch(Exception ex)
                {
                    SentrySdk.CaptureException(ex);
                    _logger.LogError($"{DateTime.Now}: BackupSaver completed work with error! {ex.Message}");

                     return _backupSaver.Logs.OrderBy(log => log.Date).Reverse().Take(500).ToArray();
                }
            }

            _logger.LogInformation($"{DateTime.Now}: BackupSaver successfully completed work...");

            return _backupSaver.Logs.OrderBy(log => log.Date).Reverse().Take(500).ToArray();
        }
    }
}

