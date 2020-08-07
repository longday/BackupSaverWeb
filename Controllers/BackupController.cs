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
        private readonly List<string> _logs;

        public BackupController(ILogger<BackupController> logger, BackupSaver backupSaver)
        {
            _logger = logger;
            _backupSaver = backupSaver;
            _logs = new List<string>();
        }

        [HttpGet]
        public async Task<string[]> MakeBackup()
        {
            string sentryConnectionString = Environment.GetEnvironmentVariable("SENTRY_CONNECTION_STRING");

            _logs.Add("Start pg_dump...");
            _logger.LogInformation("Start pg_dump...");

            using(SentrySdk.Init(sentryConnectionString))
            {
                try
                {
                    int backupDeletionPeriodInDays= int.Parse(Environment.GetEnvironmentVariable("FILE_DELETION_PERIOD_IN_DAYS") ?? throw new ArgumentNullException());
                    string message = $"Items uploaded earlier than in the last {backupDeletionPeriodInDays} days have been removed";

                    await _backupSaver.MakeBackupsAsync(backupDeletionPeriodInDays, message);

                    _logs.AddRange(_backupSaver.Logs);
        
                }
                catch(Exception ex)
                {
                    SentrySdk.CaptureException(ex);
                    _logs.Add("BackupSaver completed work with error!");
                    _logger.LogError("BackupSaver completed work with error!");

                    await Task.FromException<Exception>(ex);

                    return _logs.ToArray().Take(500);
                }
            }

            _logs.Add("Successfully");
            _logger.LogInformation("Successfully");

            return _logs.Count > 0 ? _logs.ToArray().Take(500) : new string[]{"No logs"};

        }
    }
}

