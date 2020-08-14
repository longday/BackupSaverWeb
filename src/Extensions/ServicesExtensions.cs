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
            string host = Environment.GetEnvironmentVariable("DB_HOST") ?? string.Empty;
            string port = Environment.GetEnvironmentVariable("DB_PORT") ?? string.Empty;
            string username = Environment.GetEnvironmentVariable("DB_USER_NAME") ?? string.Empty;
            string password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? string.Empty;
            string dbIgnore = Environment.GetEnvironmentVariable("DB_IGNORE") ?? string.Empty;

            var config = new PostgresBackupperConfig(host, port, username, password);

            services.AddSingleton<IAsyncBackupper>(sp => new PostgresBackupper(dbIgnore, config, new ConsoleDiagnosticLogger(SentryLevel.Info)));

            string bucket = Environment.GetEnvironmentVariable("BUCKET") ?? string.Empty;
            string s3ConnectionString = Environment.GetEnvironmentVariable("S3_CONNECTION_STRING") ?? string.Empty;
            string accessKey = Environment.GetEnvironmentVariable("ACCESS_KEY") ?? string.Empty;
            string secretKey = Environment.GetEnvironmentVariable("SECRET_KEY") ?? string.Empty;
            
            var amazonConfig = new AmazonS3Config()
            {
                ServiceURL = s3ConnectionString,
                ForcePathStyle = true
            };
            
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            var client = new AmazonS3Client(credentials, amazonConfig);

            services.AddSingleton<IAsyncRemover>(sp => new S3ObjectRemover(client, bucket));
            services.AddSingleton<IAsyncSaver>(sp => new S3StorageSaver(client, bucket, new ConsoleDiagnosticLogger(SentryLevel.Info)));

            string telegramConnectionString = Environment.GetEnvironmentVariable("TELEGRAM_CONNECTION_STRING") ?? string.Empty;

            services.AddSingleton<IAsyncReporter>(sp => new TelegramReporter(telegramConnectionString, new ConsoleDiagnosticLogger(SentryLevel.Info)));
            services.AddSingleton<BackupSaver>();
        }
    }
}
