using System.Threading.Tasks;
using Amazon.S3;

namespace WebUI.Services.Interfaces
{
    /// <summary>
    /// Interface for async saving
    /// </summary>
    public interface IAsyncSaver
    {
        public  AmazonS3Client Client { get; set; }
        
        public string Bucket { get; set; }
        
        Task SaveAsync(string source);
    }
}