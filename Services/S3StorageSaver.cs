using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        
        private readonly ILogger<S3StorageSaver> _logger;

        public S3StorageSaver()
        {
            
        }
        
        public S3StorageSaver(AmazonS3Client client, string bucket, ILogger<S3StorageSaver> logger)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SaveAsync(string source)
        {
            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));
            
            _logger.Log(LogLevel.Information, "Creating directory hierarchy...");
            
            await CreateDirectoryHierarchyAsync();
            
            _logger.Log(LogLevel.Information, "Successfully...");
            _logger.Log(LogLevel.Information, "Constructing putBackupFileRequest....");

            var putBackupFileRequest = ConstructPutObjectRequest(Bucket,
                $"{DateTime.Now.Year}" + "/" + $"{DateTime.Now.Month}" + "/" + $"{DateTime.Now.Day}" + "/" +
                Path.GetFileNameWithoutExtension(source), source);

            _logger.Log(LogLevel.Information, "Successfully");
            _logger.Log(LogLevel.Information, $"Upload {Path.GetFileNameWithoutExtension(source)}...");
            
            await Client.PutObjectAsync(putBackupFileRequest);
            
            _logger.Log(LogLevel.Information, "Successfully");
        }

        private async Task CreateDirectoryHierarchyAsync()
        {
            _logger.Log(LogLevel.Information, "Constructing putYearFolderRequest...");
            var putYearFolderRequest = ConstructPutObjectRequest(Bucket, $"{DateTime.Now.Year}" + "/");
            
            _logger.Log(LogLevel.Information, "Successfully");
            _logger.Log(LogLevel.Information, "Creating year folder");
            
            await Client.PutObjectAsync(putYearFolderRequest);
            
            _logger.Log(LogLevel.Information, "Successfully");
            _logger.Log(LogLevel.Information, "Constructing putMonthFolderRequest...");
            
            var putMonthFolderRequest =
                ConstructPutObjectRequest(Bucket, $"{DateTime.Now.Year}" + "/" + $"{DateTime.Now.Month}" + "/");
            
            _logger.Log(LogLevel.Information, "Successfully");
            _logger.Log(LogLevel.Information, "Creating month folder...");
            
            await Client.PutObjectAsync(putMonthFolderRequest);

            _logger.Log(LogLevel.Information, "Successfully");
            _logger.Log(LogLevel.Information, "Constructing putDayFolderRequest...");
            
            var putDayFolderRequest = ConstructPutObjectRequest(Bucket,
                $"{DateTime.Now.Year}" + "/" + $"{DateTime.Now.Month}" + "/" + $"{DateTime.Now.Day}" + "/");
            
            _logger.Log(LogLevel.Information, "Successfully");
            _logger.Log(LogLevel.Information, "Creating day folder...");
            
            await Client.PutObjectAsync(putDayFolderRequest);

            _logger.Log(LogLevel.Information, "Successfully");
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