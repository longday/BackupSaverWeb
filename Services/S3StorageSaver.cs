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

        public List<string> Logs{ get; }
        
        private readonly IDiagnosticLogger _logger;

        public S3StorageSaver(AmazonS3Client client, string bucket, IDiagnosticLogger logger)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Logs = new List<string>();
        }

        public async Task SaveAsync(string source)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));
            
            Logs.Add("Creating directory hierarchy...");
            _logger.Log(SentryLevel.Info, "Creating directory hierarchy...");
            
            await CreateDirectoryHierarchyAsync();
            
            Logs.Add("Successfully...");
            _logger.Log(SentryLevel.Info, "Successfully...");

            Logs.Add("Constructing putBackupFileRequest....");
            _logger.Log(SentryLevel.Info, "Constructing putBackupFileRequest....");

            var putBackupFileRequest = ConstructPutObjectRequest(Bucket,
                $"{DateTime.Now.Year}" + "/" + $"{DateTime.Now.Month}" + "/" + $"{DateTime.Now.Day}" + "/" +
                Path.GetFileNameWithoutExtension(source), source);

            Logs.Add("Successfully");
            _logger.Log(SentryLevel.Info, "Successfully");

            Logs.Add($"Upload {Path.GetFileNameWithoutExtension(source)}...");
            _logger.Log(SentryLevel.Info, $"Upload {Path.GetFileNameWithoutExtension(source)}...");
            
            await Client.PutObjectAsync(putBackupFileRequest);
            
            Logs.Add("Successfully");
            _logger.Log(SentryLevel.Info, "Successfully");
        }

        private async Task CreateDirectoryHierarchyAsync()
        {
            Logs.Add("Constructing putYearFolderRequest...");
            _logger.Log(SentryLevel.Info, "Constructing putYearFolderRequest...");
            var putYearFolderRequest = ConstructPutObjectRequest(Bucket, $"{DateTime.Now.Year}" + "/");
            
            Logs.Add("Successfully");
            _logger.Log(SentryLevel.Info, "Successfully");

            Logs.Add("Creating year folder");
            _logger.Log(SentryLevel.Info, "Creating year folder");
            
            await Client.PutObjectAsync(putYearFolderRequest);
            
            Logs.Add("Successfully");
            _logger.Log(SentryLevel.Info, "Successfully");
            Logs.Add("Constructing putMonthFolderRequest...");
            _logger.Log(SentryLevel.Info, "Constructing putMonthFolderRequest...");
            
            var putMonthFolderRequest =
                ConstructPutObjectRequest(Bucket, $"{DateTime.Now.Year}" + "/" + $"{DateTime.Now.Month}" + "/");
            
            Logs.Add("Successfully");
            _logger.Log(SentryLevel.Info, "Successfully");
            Logs.Add("Creating month folder...");
            _logger.Log(SentryLevel.Info, "Creating month folder...");
            
            await Client.PutObjectAsync(putMonthFolderRequest);

            Logs.Add("Successfully");
            _logger.Log(SentryLevel.Info, "Successfully");
            Logs.Add("Constructing putDayFolderRequest...");
            _logger.Log(SentryLevel.Info, "Constructing putDayFolderRequest...");
            
            var putDayFolderRequest = ConstructPutObjectRequest(Bucket,
                $"{DateTime.Now.Year}" + "/" + $"{DateTime.Now.Month}" + "/" + $"{DateTime.Now.Day}" + "/");
            
            Logs.Add("Successfully");
            _logger.Log(SentryLevel.Info, "Successfully");
            Logs.Add("Creating day folder...");
            _logger.Log(SentryLevel.Info, "Creating day folder...");
            
            await Client.PutObjectAsync(putDayFolderRequest);

            Logs.Add("Successfully");
            _logger.Log(SentryLevel.Info, "Successfully");
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
