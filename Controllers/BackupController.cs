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

            _logs.Add($"{DateTime.Now}: Start pg_dump...");
            _logger.LogInformation($"{DateTime.Now}: Start pg_dump...");

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
                    _logs.Add($"{DateTime.Now}: BackupSaver completed work with error!");
                    _logger.LogError($"{DateTime.Now}: BackupSaver completed work with error!");

                    await Task.FromException<Exception>(ex);

                    return _logs.Take(500).ToArray();
                }
            }

            _logs.Add($"{DateTime.Now}: Successfully");
            _logger.LogInformation($"{DateTime.Now}: Successfully");

            return _logs.Count > 0 ? _logs.Take(500).ToArray() : new string[]{"No logs"};

        }
    }
}

