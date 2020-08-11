using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sentry.Extensibility;
using Sentry.Protocol;
using WebUI.Services.Interfaces;
using System.Text;
using WebUI.Extensions;

namespace WebUI.Services
{
    /// <summary>
    /// Class for making PostgresSQL backup files
    /// </summary>
    public sealed class PostgresBackupper : IAsyncBackupper
    {
        public PostgresBackupperConfig Config { get; set; }

        public List<Log> Logs{ get; }
        
        public string DbIgnore{ get; set; }
        
        private readonly IDiagnosticLogger _logger;

        public PostgresBackupper(string dbIgnore, PostgresBackupperConfig config, IDiagnosticLogger logger)
        {
            DbIgnore = dbIgnore ?? throw new ArgumentNullException(nameof(dbIgnore));
            Config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Logs = new List<Log>();
        }

        public async Task<string> MakeBackupAsync()
        {
            Environment.SetEnvironmentVariable("PGPASSWORD", Config.Password);
            
            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Set PGPASSWORD..."));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Set PGPASSWORD...");

            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Successfully..."));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Successfully...");
            
            string outFilePath = Path.Combine(
                Path.GetTempPath(),
                "backup", 
                $"{DateTime.Now:yyyy-dd-M--HH-mm-ss}");
            
            Directory.CreateDirectory(outFilePath);

            if (!Directory.EnumerateFiles(outFilePath, "*.*", SearchOption.AllDirectories).Any())
                throw new DirectoryIsEmptyException("Failed to make dumps. Check the connection");
            
            var databases = await GetDbListAsync();

            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Creating sql files...."));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Creating sql files....");

            await CreateSqlFilesAsync(databases, outFilePath, Config)
                .ConfigureAwait(false);
            
            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Successfully..."));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Successfully...");

            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Creating result archive...."));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Creating result archive....");

            string archivePath = await CreateArchiveAsync(outFilePath, $"{DateTime.Now:yyyy-dd-M--HH-mm-ss}")
                .ConfigureAwait(false);
            
            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Successfully created {Path.GetFileNameWithoutExtension(archivePath)} archive..."));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Successfully created {Path.GetFileNameWithoutExtension(archivePath)} archive...");

            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Removing intermediate folder {Path.GetFileNameWithoutExtension(outFilePath)}..."));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Removing intermediate folder {Path.GetFileNameWithoutExtension(outFilePath)}...");
            
            if(Directory.Exists(outFilePath))
                Directory.Delete(outFilePath, true);

            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: The backups {databases.ToFormatString()} were successfully archived and compressed." +
                        $"Archive weight: {new FileInfo(archivePath).Length} Bytes"));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: The backups {databases.ToFormatString()} were successfully archived and compressed." +
                        $"Archive weight: {new FileInfo(archivePath).Length} Bytes");
            
            return archivePath;
        }

        private static async Task CreateSqlFilesAsync(string[] databases, string outFilePath, PostgresBackupperConfig config)
        {
            if (databases == null) 
                throw new ArgumentNullException(nameof(databases));
            
            if (string.IsNullOrWhiteSpace(outFilePath))
                throw new ArgumentNullException(nameof(outFilePath));
            
            if (config == null) 
                throw new ArgumentNullException(nameof(config));

            foreach (var db in databases)
            {
                string resultPath = Path.Combine(outFilePath, 
                    $"{db}.sql");
                
                string dumpCommand = $"pg_dump -h {config.Host} -p {config.Port} -U " +
                                     $"{config.Username} -Fp -d {db} > {resultPath}";
                
                await ExecuteCommandAsync(dumpCommand)
                    .ConfigureAwait(continueOnCapturedContext: false);
                
                if(FileIsEmpty(resultPath)) 
                    throw new FileIsEmptyException("Dump file cannot be empty!");
            }
        }

        private static async Task<string> CreateArchiveAsync(string path, string outFileName)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            
            if (string.IsNullOrWhiteSpace(outFileName))
                throw new ArgumentNullException(nameof(outFileName));

            string archivePath = Path.Combine(Path.GetTempPath(), $"{outFileName}.tar.bz2");
            string createArchiveCommand = $"tar -cvjf {archivePath} {path}";
            
            await ExecuteCommandAsync(createArchiveCommand)
                .ConfigureAwait(continueOnCapturedContext: false);

            return archivePath;
        }
        
        private static Task ExecuteCommandAsync(string command)
        {
            if(string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command));

            return Task.Run(() =>
            {
                string batFilePath = Path.Combine(Path.GetTempPath(),
                    $"{Guid.NewGuid()}.sh");

                string batchContent = string.Empty;
                batchContent += $"{command}";
                
                File.WriteAllText(batFilePath, batchContent);

                ProcessStartInfo processStartInfo = new ProcessStartInfo("sh")
                {
                    Arguments = $"{batFilePath}"
                };

                Process process = Process.Start(processStartInfo);

                if (process != null)
                {
                    process.WaitForExit();

                    process.Close();
                }

                if(File.Exists(batFilePath))
                    File.Delete(batFilePath);
            });
        }
        
        private static bool FileIsEmpty(string path)
        {
            if(string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            return File.ReadAllLines(path).Length <= 1;
        }

        private async Task<string[]> GetDbListAsync()
        {
            string[] ignoredDatabases = DbIgnore.Split(',', StringSplitOptions.RemoveEmptyEntries);
            
            Environment.SetEnvironmentVariable("PGPASSWORD", Config.Password);

            string tmpFilePath = Path.Combine(Path.GetTempPath(), "db.txt");
            string getDbListQuery = $"psql -h {Config.Host} -p {Config.Port} -U {Config.Username} -c" +
                                    $"'SELECT datname FROM pg_database' > {tmpFilePath}";

            await ExecuteCommandAsync(getDbListQuery)
                  .ConfigureAwait(false);

            var fileParser = new DbTableFileParser();

            var databases = fileParser.GetParsedValues(tmpFilePath, ignoredDatabases);

            if(File.Exists(tmpFilePath))
                File.Delete(tmpFilePath);

            return databases;
        }
    }
}
