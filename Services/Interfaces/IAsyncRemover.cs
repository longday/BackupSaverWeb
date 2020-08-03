using System.Threading.Tasks;
using Amazon.S3;

namespace WebUI.Services.Interfaces
{
    /// <summary>
    /// Interface for removing
    /// </summary>
    public interface IAsyncRemover
    { 
        AmazonS3Client Client { get; set; }
        string Bucket { get; set; }
        Task RemoveAsync(int quantity);
    }
}