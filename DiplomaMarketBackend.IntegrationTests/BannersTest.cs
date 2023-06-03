using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaMarketBackend.IntegrationTests
{
    public class BannersTest
    {
        private readonly HttpClient _httpClient;
        private static string? _jwtToken = string.Empty;
        private static string? _userId = string.Empty;
        private static string? _userPhone = string.Empty;

        private readonly string admin_mail = "admin@gmail.com";
        private readonly string admin_pass = "Test123admin$";

        public BannersTest()
        {
            var webApplicationFactory = new CustomWebApplicationFactory<Program>();
            _httpClient = webApplicationFactory.CreateDefaultClient();
        }

        [Theory]
        [InlineData(1)]
        public async void GetCategoryBanners_Succes(int category_id)
        {
            // Arrange

            // Act
            var result = await _httpClient.GetAsync($"/api/Banners/category-banners?category_id={category_id}");

            // Assert
            Assert.NotNull( result );
            Assert.True(result.IsSuccessStatusCode );
        }

        [Fact]
        public async void GetBannersList_Success()
        {
            // Arrange

            // Act
            var result = await _httpClient.GetAsync($"/api/Banners/banners-list");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }

        [Fact]
        public async void AddBanner_Success()
        {
            // Arrange
            var path = Path.Combine(Directory.GetCurrentDirectory(), "files", "icons", "Зоотовари.png");
            FileStream fsSource = new FileStream(path, FileMode.Open, FileAccess.Read);
            FormFile mockFile = new FormFile(fsSource, 0, fsSource.Length, "Зоотовари", "Зоотовари");

            var request = new MultipartFormDataContent
            {
                { new StringContent("1"),"category_id"},
                { new StreamContent(mockFile.OpenReadStream()),"file",mockFile.FileName },

            };

            // Act
            var result = await _httpClient.PostAsync($"/api/Banners/add",request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }

        [Fact]
        public async void RemoveBanner_Success()
        {
            // Arrange
            var response = await _httpClient.GetAsync("/api/banners/category-banners?category_id=1");
            var data = await response.Content.ReadAsStringAsync();
            var actions = JsonConvert.DeserializeObject<List<dynamic>>(data);
            var max_id = actions.Max(x => (int)x["id"]);

            // Act
            var result = await _httpClient.DeleteAsync($"/api/Banners/remove?banner_id={max_id}");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }
    }
}
