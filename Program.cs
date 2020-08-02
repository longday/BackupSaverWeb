using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebUI
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            var bucket = Environment.GetEnvironmentVariable("BUCKET") ?? string.Empty;
            var s3ConnectionString = Environment.GetEnvironmentVariable("S3_CONNECTION_STRING") ?? string.Empty;
            var accessKey = Environment.GetEnvironmentVariable("ACCESS_KEY") ?? string.Empty;
            var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY") ?? string.Empty;
            var telegramConnectionString = Environment.GetEnvironmentVariable("TELEGRAM_CONNECTION_STRING") ?? string.Empty;
            var username = Environment.GetEnvironmentVariable("USER_NAME") ?? string.Empty;
            var password = Environment.GetEnvironmentVariable("PASSWORD") ?? string.Empty;
            var host = Environment.GetEnvironmentVariable("HOST") ?? string.Empty;
            var port = Environment.GetEnvironmentVariable("PORT") ?? string.Empty;
            var dbList = Environment.GetEnvironmentVariable("DB_LIST") ?? string.Empty;

            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddSentry();
                });
    }
}
