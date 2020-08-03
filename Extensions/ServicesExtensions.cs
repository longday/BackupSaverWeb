using Microsoft.Extensions.DependencyInjection;
using Sentry.Extensibility;
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
            services.AddScoped<IAsyncBackupper, PostgresBackupper>();
            services.AddScoped<IAsyncRemover, S3ObjectRemover>();
            services.AddScoped<IAsyncSaver, S3StorageSaver>();
            services.AddScoped<IAsyncReporter, TelegramReporter>();
            services.AddScoped<IDiagnosticLogger>(sp => new ConsoleDiagnosticLogger(SentryLevel.Info));
            services.AddScoped<BackupSaver>();
        }
    }
}