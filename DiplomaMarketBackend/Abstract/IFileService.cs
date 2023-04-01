using MongoDB.Driver;

namespace DiplomaMarketBackend.Abstract
{
    public interface IFileService
    {
        public Task<string> SaveFileFromStream(string bucketName, string fileName, Stream stream);

        public Task<byte[]> GetFile(string fileId, string bucketName);

        public Task<string> SaveFileFromUrl(string bucketName, string Url);

    }
}
