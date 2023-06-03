using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DiplomaMarketBackend.IntegrationTests
{
    public class BrandsTest
    {
        private readonly HttpClient _httpClient;
        private static int _id;

        public BrandsTest()
        {
            var webApplicationFactory = new CustomWebApplicationFactory<Program>();
            _httpClient = webApplicationFactory.CreateDefaultClient();
        }

        [Fact]
        public async void GetPopularBrands_Success()
        {
            // Arrange

            // Act
            var result = await _httpClient.GetAsync($"/api/Brands/popular?lang=uk&number_to_show=10");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }

        [Fact]
        public async void GetCategoryBrands_Success()
        {
            // Arrange

            // Act
            var result = await _httpClient.GetAsync($"/api/Brands/popular?category_id=1&lang=uk&count=10");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }

        [Fact]
        public async void GetFilterBrands_Success()
        {
            // Act
            var result = await _httpClient.GetAsync($"/api/Brands/popular?category_id=1&lang=uk&count=10");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }

        [Fact]
        public async void GetServicesBrands_Success()
        {
            // Act
            var result = await _httpClient.GetAsync($"/api/Brands/brands-service?search_symbol=A&lang=uk");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }


        [Fact]
        public async void CreateBrand_Success()
        {
            // Arrange
            var path = Path.Combine(Directory.GetCurrentDirectory(), "files", "icons", "Zoo.png");
            FileStream fsSource = new FileStream(path, FileMode.Open, FileAccess.Read);
            FormFile mockFile = new FormFile(fsSource, 0, fsSource.Length, "Zoo", "Zoo");

            var request = new MultipartFormDataContent
            {
                { new StringContent("New_brand"),"name"},
                { new StreamContent(mockFile.OpenReadStream()),"logo",mockFile.FileName },

            };

            // Act
            var result = await _httpClient.PostAsync($"/api/Brands/create", request);
            var data = await result.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<dynamic>(data);
            _id = res != null? res["entity"]["id"]:1;

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }

        [Fact]
        public async void UpdateBrand_Success()
        {
            // Arrange
            var path = Path.Combine(Directory.GetCurrentDirectory(), "files", "icons", "Zoo.png");
            FileStream fsSource = new FileStream(path, FileMode.Open, FileAccess.Read);
            FormFile mockFile = new FormFile(fsSource, 0, fsSource.Length, "Zoo", "Zoo");



            var request = new MultipartFormDataContent
            {
                { new StringContent(_id.ToString()),"id"},
                { new StringContent("Microsoft"),"name"},
                { new StreamContent(mockFile.OpenReadStream()),"logo",mockFile.FileName },

            };

            // Act
            var result = await _httpClient.PutAsync($"/api/Brands/update", request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }

        [Fact]
        public async void DeleteBrand_Success()
        {
            // Arrange
            while(_id == 0) { }

            // Act
            var result = await _httpClient.DeleteAsync($"/api/Brands/delete?brand_id={_id}");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }
    }
}
