using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;
using System.Threading.Tasks;
using Sentry.Extensibility;
using System.Collections.Generic;
using Sentry.Protocol;
using WebUI.Services.Interfaces;

namespace WebUI.Services
{
    /// <summary>
    /// Class for saving files in AmazonS3
    /// </summary>
    public sealed class S3StorageSaver : IAsyncSaver
    {
        public AmazonS3Client Client { get; set; }
        
        public string Bucket { get; set; }

        public List<Log> Logs{ get; }
        
        private readonly IDiagnosticLogger _logger;

        public S3StorageSaver(AmazonS3Client client, string bucket, IDiagnosticLogger logger)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Logs = new List<Log>();
        }

        public async Task SaveAsync(string source)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));
            
            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Creating directory hierarchy in AmazonS3..."));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Creating directory hierarchy...");
            
            await CreateDirectoryHierarchyAsync();
            
            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Successfully creating directory hierarchy in AmazonS3..."));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Successfully creating directory hierarchy in AmazonS3...");

            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Constructing putBackupFileRequest...."));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Constructing putBackupFileRequest....");

            var putBackupFileRequest = ConstructPutObjectRequest(Bucket,
                $"{DateTime.Now.Year}" + "/" + $"{DateTime.Now.Month}" + "/" + $"{DateTime.Now.Day}" + "/" +
                Path.GetFileNameWithoutExtension(source), source);

            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Successfully"));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Successfully");

            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Upload {Path.GetFileNameWithoutExtension(source)}..."));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Upload {Path.GetFileNameWithoutExtension(source)}...");
            
            await Client.PutObjectAsync(putBackupFileRequest);
            
            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Successfully saved {Path.GetFileNameWithoutExtension(source)}." +
                     $"Weight: {new FileInfo(source).Length} Bytes"));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Successfully saved {Path.GetFileNameWithoutExtension(source)}." +
                                          $"Weight: {new FileInfo(source).Length} Bytes");
        }

        private async Task CreateDirectoryHierarchyAsync()
        {
            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Constructing putYearFolderRequest..."));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Constructing putYearFolderRequest...");
            var putYearFolderRequest = ConstructPutObjectRequest(Bucket, $"{DateTime.Now.Year}" + "/");
            
            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Successfully"));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Successfully");

            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Creating year folder"));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Creating year folder");
            
            await Client.PutObjectAsync(putYearFolderRequest);
            
            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Successfully"));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Successfully");
            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Constructing putMonthFolderRequest..."));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Constructing putMonthFolderRequest...");
            
            var putMonthFolderRequest =
                ConstructPutObjectRequest(Bucket, $"{DateTime.Now.Year}" + "/" + $"{DateTime.Now.Month}" + "/");
            
            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Successfully"));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Successfully");
            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Creating month folder..."));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Creating month folder...");
            
            await Client.PutObjectAsync(putMonthFolderRequest);

            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Successfully"));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Successfully");
            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Constructing putDayFolderRequest..."));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Constructing putDayFolderRequest...");
            
            var putDayFolderRequest = ConstructPutObjectRequest(Bucket,
                $"{DateTime.Now.Year}" + "/" + $"{DateTime.Now.Month}" + "/" + $"{DateTime.Now.Day}" + "/");
            
            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Successfully"));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Successfully");
            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Creating day folder..."));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Creating day folder...");
            
            await Client.PutObjectAsync(putDayFolderRequest);

            Logs.Add(new Log(DateTime.Now, $"{DateTime.Now}: Successfully"));
            _logger.Log(SentryLevel.Info, $"{DateTime.Now}: Successfully");
        }

        private static PutObjectRequest ConstructPutObjectRequest(string bucket, string key)
        {
            if(string.IsNullOrWhiteSpace(nameof(key)))
                throw new ArgumentNullException(nameof(key));
            
            if(string.IsNullOrWhiteSpace(bucket))
                throw new ArgumentNullException(nameof(bucket));

            return new PutObjectRequest()
            {
                BucketName = bucket,
                Key = key,
                ContentBody = string.Empty,
                StorageClass = S3StorageClass.Standard,
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.None
            };
        }
        
        private static PutObjectRequest ConstructPutObjectRequest(string bucket, string key, string filePath)
        {
            if(string.IsNullOrWhiteSpace(nameof(key)))
                throw new ArgumentNullException(nameof(key));
            
            if(string.IsNullOrWhiteSpace(bucket))
                throw new ArgumentNullException(nameof(bucket));
            
            if(string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            return new PutObjectRequest()
            {
                BucketName = bucket,
                Key = key,
                ContentBody = string.Empty,
                FilePath = filePath,
                StorageClass = S3StorageClass.Standard,
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.None
            };
        }
    }
}
