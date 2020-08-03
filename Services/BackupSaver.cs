using System;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
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
            
            ConfigureDependencies();
        }

        private void ConfigureDependencies()
        {
            var environmentVariables = Environment.GetEnvironmentVariables();

            string host = environmentVariables["HOST"] as string;
            string port = environmentVariables["PORT"] as string;
            string username = environmentVariables["USER_NAME"] as string;
            string password = environmentVariables["PASSWORD"] as string;
            
            _backupper.DbList = environmentVariables["DB_LIST"] as string;
            _backupper.Config = new PostgresBackupperConfig(host, port, username, password);

            string bucket = environmentVariables["BUCKET"] as string;
            string s3ConnectionString = environmentVariables["S3_CONNECTION_STRING"] as string;
            string accessKey = environmentVariables["ACCESS_KEY"] as string;
            string secretKey = environmentVariables["SECRET_KEY"] as string;
            
            var config = new AmazonS3Config()
            {
                ServiceURL = s3ConnectionString,
                ForcePathStyle = true
            };
            
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            var client = new AmazonS3Client(credentials, config);
            
            _saver.Bucket = bucket;
            _saver.Client = client;

            _remover.Bucket = bucket;
            _remover.Client = client;

            string telegramConnectionString = environmentVariables["TELEGRAM_CONNECTION_STRING"] as string;

            _reporter.ConnectionString = telegramConnectionString;

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