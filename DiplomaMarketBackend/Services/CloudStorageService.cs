using DiplomaMarketBackend.Abstract;
using DiplomaMarketBackend.Helpers;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;

namespace DiplomaMarketBackend.Services
{

    public class CloudStorageService : ICloudStorageService
    {

        private readonly GCSConfigOptions _options;
        private readonly ILogger<CloudStorageService> _logger;


        public CloudStorageService(IOptions<GCSConfigOptions> options,
                                    ILogger<CloudStorageService> logger)
        {
            _options = options.Value;
            _logger = logger;

        }

        public async Task<string> UploadFileAsync(IFormFile fileToUpload, string fileNameToSave)
        {
            try
            {
                _logger.LogInformation($"Uploading: file {fileNameToSave} to storage {_options.GoogleCloudStorageBucketName}");
                using (var memoryStream = new MemoryStream())
                {
                    await fileToUpload.CopyToAsync(memoryStream);
                    // Create Storage Client from Google Credential from environment
                    using (var storageClient = StorageClient.Create())
                    {
                        // upload file stream
                        var uploadedFile = await storageClient.UploadObjectAsync(_options.GoogleCloudStorageBucketName, fileNameToSave, fileToUpload.ContentType, memoryStream);
                        _logger.LogInformation($"Uploaded: file {fileNameToSave} to storage {_options.GoogleCloudStorageBucketName}");
                        var url = _options.GoogleCloudStorageURL + _options.GoogleCloudStorageBucketName + "/" + fileNameToSave;
                        return url.ToString();
                        //return uploadedFile.MediaLink;
                        
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while uploading file {fileNameToSave}: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteFileAsync(string fileNameToDelete)
        {
            try
            {
                using (var storageClient = StorageClient.Create())
                {
                    await storageClient.DeleteObjectAsync(_options.GoogleCloudStorageBucketName, fileNameToDelete);
                }
                _logger.LogInformation($"File {fileNameToDelete} deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured while deleting file {fileNameToDelete}: {ex.Message}");
                throw;
            }
        }
    }
}
