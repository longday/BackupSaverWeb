using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sentry.Extensibility;
using Sentry.Protocol;
using WebUI.Services.Interfaces;
using System.Text;

namespace WebUI.Services
{
    /// <summary>
    /// Class for making PostgresSQL backup files
    /// </summary>
    public sealed class PostgresBackupper : IAsyncBackupper
    {
        public PostgresBackupperConfig Config { get; set; }

        public List<string> Logs{ get; }
        
        public string DbList { get; set; }
        
        private readonly IDiagnosticLogger _logger;

        public PostgresBackupper(string dbList, PostgresBackupperConfig config, IDiagnosticLogger logger)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            DbList = dbList ?? throw new ArgumentNullException(nameof(dbList));
            Logs = new List<string>();
        }

        public async Task<string> MakeBackupAsync()
        {
            Environment.SetEnvironmentVariable("PGPASSWORD", Config.Password);
            
            Logs.Add($"{DateTime.Now}: Set PGPASSWORD...");
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Set PGPASSWORD...");

            Logs.Add($"{DateTime.Now}: Successfully...");
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Successfully...");
            
            string outFilePath = Path.Combine(
                Path.GetTempPath(),
                "backup", 
                $"{DateTime.Now:yyyy-dd-M--HH-mm-ss}");
            
            Directory.CreateDirectory(outFilePath);
            
            string[] databases = DbList.Split(',', StringSplitOptions.RemoveEmptyEntries);
            
            Logs.Add($"{DateTime.Now}: Creating sql files....");
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Creating sql files....");

            await CreateSqlFilesAsync(databases, outFilePath, Config)
                .ConfigureAwait(false);
            
            Logs.Add($"{DateTime.Now}: Successfully...");
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Successfully...");

            Logs.Add($"{DateTime.Now}: Creating result archive....");
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Creating result archive....");

            string archivePath = await CreateArchiveAsync(outFilePath, $"{DateTime.Now:yyyy-dd-M--HH-mm-ss}")
                .ConfigureAwait(false);
            
            Logs.Add($"{DateTime.Now}: Successfully created {Path.GetFileNameWithoutExtension(archivePath)} archive...");
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Successfully created {Path.GetFileNameWithoutExtension(archivePath)} archive...");

            Logs.Add($"{DateTime.Now}: Removing intermediate folder {Path.GetFileNameWithoutExtension(outFilePath)}...");
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Removing intermediate folder {Path.GetFileNameWithoutExtension(outFilePath)}...");
            
            if(Directory.Exists(outFilePath))
                Directory.Delete(outFilePath, true);

            Logs.Add($"{DateTime.Now}: The backups {GetDbListString()} were successfully archived and compressed." +
                        $"Archive weight: {new FileInfo(archivePath).Length} Bytes");
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: The backups {databases} were successfully archived and compressed." +
                        $"Archive weight: {new FileInfo(archivePath).Length} Bytes");
            
            return archivePath;
        }

        private static async Task CreateSqlFilesAsync(string[] databases, string outFilePath, PostgresBackupperConfig config)
        {
            if (databases == null || databases.Length == 0) 
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
        
        private static ProcessStartInfo GetProcessInfoByOs(string batFilePath)
        {
            if(string.IsNullOrWhiteSpace(batFilePath))
                throw new ArgumentNullException(nameof(batFilePath));
            
            ProcessStartInfo info;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                info = new ProcessStartInfo(batFilePath);
            }
            else
            {
                info = new ProcessStartInfo("sh")
                {
                    Arguments = $"{batFilePath}"
                };
            }

            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            info.RedirectStandardError = true;

            return info;
        }

        private static Task ExecuteCommandAsync(string command)
        {
            if(string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command));

            return Task.Run(() =>
            {
                string batFilePath = Path.Combine(Path.GetTempPath(),
                    $"{Guid.NewGuid()}." + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "bat" : "sh"));

                string batchContent = string.Empty;
                batchContent += $"{command}";
                
                File.WriteAllText(batFilePath, batchContent);

                ProcessStartInfo processStartInfo = GetProcessInfoByOs(batFilePath);
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

        private string GetDbListString()
        {
            string[] databases = DbList.Split(',', StringSplitOptions.RemoveEmptyEntries);

            StringBuilder strBuilder = new StringBuilder();

            for (int i = 0; i < databases.Length; i++)
            {
                if(i == databases.Length - 1)
                {
                    strBuilder.Append(databases[i] + $" ");
                    break;
                }

                strBuilder.Append(databases[i] + $",");
            }

            return strBuilder.ToString();
        }
    }
}
