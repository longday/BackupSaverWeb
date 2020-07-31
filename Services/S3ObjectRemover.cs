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
        private readonly AmazonS3Client _client;
        private readonly string _bucket;

        public S3ObjectRemover(AmazonS3Client client, string bucket)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));
        }

        public async Task RemoveAsync(int quantity)
        {
            if(quantity < 0)
                throw new ArgumentException(nameof(quantity));

            var getRequest = new ListObjectsV2Request()
            {
                BucketName = _bucket,
            };

            var getResponse = await _client.ListObjectsV2Async(getRequest).ConfigureAwait(continueOnCapturedContext: false);
            if (getResponse.S3Objects.Count > 0)
            {
                var oldestDay = getResponse.S3Objects.Max(o => o.LastModified.Day) - quantity;
                var keys = getResponse.S3Objects.Where(o => o.LastModified.Day <= oldestDay)
                    .Select(o => new KeyVersion() {Key = o.Key}).ToList();

                if (keys.Count > 0)
                {
                    var deleteRequest = new DeleteObjectsRequest()
                    {
                        BucketName = _bucket,
                        Objects = keys
                    };

                    await _client.DeleteObjectsAsync(deleteRequest)
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