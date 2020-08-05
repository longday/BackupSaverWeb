using System;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using Sentry.Infrastructure;
using Sentry.Protocol;
using WebUI.Services;
using WebUI.Services.Interfaces;

namespace WebUI.Extensions
{
    public static class ServicesExtensions
    {
        public static void AddBackupServices(this IServiceCollection services)
        {
            var environmentVariables = Environment.GetEnvironmentVariables();

            string host = environmentVariables["HOST"] as string;
            string port = environmentVariables["PORT"] as string;
            string username = environmentVariables["USER_NAME"] as string;
            string password = environmentVariables["PASSWORD"] as string;
            string dbList = environmentVariables["DB_LIST"] as string;

            var config = new PostgresBackupperConfig(host, port, username, password);

            services.AddScoped<IAsyncBackupper>(sp => new PostgresBackupper(dbList, config, new ConsoleDiagnosticLogger(SentryLevel.Info)));

            string bucket = environmentVariables["BUCKET"] as string;
            string s3ConnectionString = environmentVariables["S3_CONNECTION_STRING"] as string;
            string accessKey = environmentVariables["ACCESS_KEY"] as string;
            string secretKey = environmentVariables["SECRET_KEY"] as string;
            
            var amazonConfig = new AmazonS3Config()
            {
                ServiceURL = s3ConnectionString,
                ForcePathStyle = true
            };
            
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            var client = new AmazonS3Client(credentials, amazonConfig);

            services.AddScoped<IAsyncRemover>(sp => new S3ObjectRemover(client, bucket));
            services.AddScoped<IAsyncSaver>(sp => new S3StorageSaver(client, bucket, new ConsoleDiagnosticLogger(SentryLevel.Info)));

            string telegramConnectionString = environmentVariables["TELEGRAM_CONNECTION_STRING"] as string;

            services.AddScoped<IAsyncReporter>(sp => new TelegramReporter(telegramConnectionString, new ConsoleDiagnosticLogger(SentryLevel.Info)));
            services.AddScoped<BackupSaver>();
        }
    }
}
