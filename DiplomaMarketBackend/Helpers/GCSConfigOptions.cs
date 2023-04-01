namespace DiplomaMarketBackend.Helpers
{
    public class GCSConfigOptions
    {
        public string? GoogleCloudStorageBucketName { get; set; }
        public string? GoogleCloudStorageURL { get; set; }

        //not needed in docker -  taken from environment
        //public string? GCPStorageAuthFile { get; set; }

    }
}
