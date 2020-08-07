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
        private readonly List<Log> _logs;

        public BackupController(ILogger<BackupController> logger, BackupSaver backupSaver)
        {
            _logger = logger;
            _backupSaver = backupSaver;
            _logs = new List<Log>();
        }

        [HttpGet]
        public async Task<Log[]> MakeBackup()
        {
            string sentryConnectionString = Environment.GetEnvironmentVariable("SENTRY_CONNECTION_STRING");

            _logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Start pg_dump..."));
            _logger.LogInformation($"{DateTime.Now}: Start pg_dump...");

            using(SentrySdk.Init(sentryConnectionString))
            {
                try
                {
                    int backupDeletionPeriodInDays= int.Parse(Environment.GetEnvironmentVariable("FILE_DELETION_PERIOD_IN_DAYS") ?? throw new ArgumentNullException());
                    string message = $"Items uploaded earlier than in the last {backupDeletionPeriodInDays} days have been removed." +
                                        "Backups were archived and saved in AmazonS3...";

                    await _backupSaver.MakeBackupsAsync(backupDeletionPeriodInDays, message);

                    _logs.AddRange(_backupSaver.Logs);
        
                }
                catch(Exception ex)
                {
                    SentrySdk.CaptureException(ex);
                    _logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: BackupSaver completed work with error! {ex.Message}"));
                    _logger.LogError($"{DateTime.Now}: BackupSaver completed work with error! {ex.Message}");

                     return _logs.OrderBy(log => log.Date).Take(500).ToArray();
                }
            }

            _logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: BackupSaver successfully completed work..."));
            _logger.LogInformation($"{DateTime.Now}: BackupSaver successfully completed work...");

            return _logs.OrderBy(log => log.Date).Take(500).ToArray();

        }
    }
}

