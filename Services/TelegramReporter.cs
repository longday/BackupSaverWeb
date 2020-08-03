using WebUI.Services.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;
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
        }

        public async Task ReportAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            _logger.Log(SentryLevel.Info, $"Sending message to {ConnectionString}....");
            
            await Client.PostAsync(ConnectionString, new StringContent("{\"text\":\"BackupSaver: " + message + "\"}"));
            
            _logger.Log(SentryLevel.Info, "Successfully...");
        }
    }
}