using WebUI.Services.Interfaces;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace WebUI.Services
{
    /// <summary>
    /// Class for removing objects from AmazonS3
    /// </summary>
    public sealed class S3ObjectRemover : IAsyncRemover
    {
        public  AmazonS3Client Client { get; set; }
        
        public string Bucket { get; set; }

        public S3ObjectRemover(AmazonS3Client client, string bucket)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));
        }

        public async Task RemoveAsync(int quantity)
        {
            if(quantity < 0)
                throw new ArgumentException(nameof(quantity));

            var getRequest = new ListObjectsV2Request()
            {
                BucketName = Bucket,
            };

            var getResponse = await Client.ListObjectsV2Async(getRequest).ConfigureAwait(continueOnCapturedContext: false);
            if (getResponse.S3Objects.Count > 0)
            {
                var oldestDay = getResponse.S3Objects.Max(o => o.LastModified.Day) - quantity;
                var keys = getResponse.S3Objects.Where(o => o.LastModified.Day <= oldestDay)
                    .Select(o => new KeyVersion() {Key = o.Key}).ToList();

                if (keys.Count > 0)
                {
                    var deleteRequest = new DeleteObjectsRequest()
                    {
                        BucketName = Bucket,
                        Objects = keys
                    };

                    await Client.DeleteObjectsAsync(deleteRequest)
                        .ConfigureAwait(continueOnCapturedContext: false);
                }
                else
                {
                    await Task.CompletedTask;
                }
            }
            else
            {
                await Task.CompletedTask;
            }
        }
    }
}