using WebUI.Services.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Sentry.Extensibility;
using Sentry.Protocol;

namespace WebUI.Services
{
    /// <summary>
    /// Class for reporting messages in telegram 
    /// </summary>
    public sealed class TelegramReporter : IAsyncReporter
    {
        public string ConnectionString { get; set; }

        public List<string> Logs{ get; }
        
        private readonly IDiagnosticLogger _logger;
        
        private static readonly HttpClient Client;

        static TelegramReporter()
        {
            Client = new HttpClient();
        }

        public TelegramReporter(string connectionString, IDiagnosticLogger logger)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Logs = new List<string>();
        }

        public async Task ReportAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            Logs.Add($"{DateTime.Now}: Sending message to {ConnectionString}....");
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Sending message to {ConnectionString}....");
            
            await Client.PostAsync(ConnectionString, new StringContent("{\"text\":\"BackupSaver: " + message + "\"}"));
            
            Logs.Add($"{DateTime.Now}: Successfully...");
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Successfully...");
        }
    }
}
