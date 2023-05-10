using DiplomaMarketBackend.Abstract;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System.Net;

namespace DiplomaMarketBackend.Services
{
    public class FileService : IFileService
    {
        IMongoDatabase? _database;
        ILogger<FileService> _logger;

        public FileService(IConfiguration configuration, ILogger<FileService> logger)
        {
            var currConnectionString = "FileServerMongo";
            _logger = logger;
            try
            {
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    currConnectionString = "FileServerMongoLocal";
                }
                else if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Testing")
                {
                    currConnectionString = "FileServerMongoTesting";
                }

                var _mogoConnection = configuration.GetConnectionString(currConnectionString);
                var _mongoDatabase = configuration.GetValue<String>("FileServerDbName");
                _database = new MongoClient(_mogoConnection).GetDatabase(_mongoDatabase);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e.Message);
                throw;
            }

        }

        /// <summary>
        /// Returns File from MongoDb from given bucket name and id from url
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="bucketName"></param>
        /// <returns>Returns byte[] represents file requested or null if </returns>
        public async Task<byte[]> GetFile(string fileId, string bucketName)
        {
            try
            {
                var gridFS = getBucket(bucketName);

                return await gridFS.DownloadAsBytesAsync(new ObjectId(fileId));
            }
            catch (Exception)
            {

                return new byte[] { };
            }

        }


        /// <summary>
        /// Saves file in MongoDb and returns its Id
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fileName"></param>
        /// <param name="stream"></param>
        /// <returns>File Id</returns>
        public async Task<string> SaveFileFromStream(string bucketName, string fileName, Stream stream)
        {
            var gridFS = getBucket(bucketName);

            var id = await gridFS.UploadFromStreamAsync(fileName, stream);

            return id.ToString();
        }

        /// <summary>
        /// Saves file from given URL to database
        /// </summary>
        /// <param name="bucketName">desired bucket name</param>
        /// <param name="Url">file url where to take file</param>
        /// <returns>GridFs file id</returns>
        public async Task<string> SaveFileFromUrl(string bucketName, string Url)
        {
            _logger.LogWarning("Gettin file :" + Url);

            try
            {
                var gridFS = getBucket(bucketName);
                string fileName = Path.GetFileName(Url).GetHashCode().ToString();
                ObjectId id;
                using (var webclient = new WebClient())
                {
                    var content = webclient.DownloadData(Url);
                    using (var stream = new MemoryStream(content))
                    {
                        id = await gridFS.UploadFromStreamAsync(fileName, stream);
                    }
                }

                return id.ToString();
            }
            catch (WebException e)
            {
                _logger.LogWarning(e.Message);
                return "";

            }catch (Exception e){

                _logger.LogWarning(e.Message + "\n\n In file service");
                throw;
            }

        }

        public void DeleteFile(string bucketName, string fileId)
        {
            var gridFS = getBucket(bucketName);
            gridFS.Delete(new ObjectId(fileId));
        }


        private IGridFSBucket getBucket(string bucketName)
        {
           
            return new GridFSBucket(_database);

        }
         

       
    }
}
