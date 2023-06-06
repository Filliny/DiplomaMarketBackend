using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
namespace DiplomaMarketBackend.IntegrationTests.Tests
{
    public class GoodsAdminTest:BasicTest
    {
        

        [Theory]
        [InlineData(1)]
        public async void GetArticleTest_Success(int id)
        {
            // Act
            var response = await _httpClient.GetAsync($"/api/GoodsAdmin/get?id={id}");

            Assert.True(response.IsSuccessStatusCode);

        }

        [Fact]
        public async void CreateArticle_Success()
        {
            // Arrange

            var path = Path.Combine(Directory.GetCurrentDirectory(), "files", "icons", "Zoo.png");
            FileStream fsSource = new FileStream(path, FileMode.Open, FileAccess.Read);
            FormFile mockFile = new FormFile(fsSource, 0, fsSource.Length, "Zoo", "Zoo");


            var response = await _httpClient.GetAsync($"/api/GoodsAdmin/get?id=1");
            var article = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(article);
            var cat_result = data["data"].ToString();

            var request = new MultipartFormDataContent
            {
                { new StringContent(cat_result),"article_json"},
                { new StreamContent(mockFile.OpenReadStream()),"images",mockFile.FileName },

            };

            // Act
            var result = await  _httpClient.PostAsync("/api/GoodsAdmin/create", request);
            var content = await result.Content.ReadAsStringAsync();
            
            // Assert
            Assert.True(result.StatusCode == System.Net.HttpStatusCode.Created);

        }


        [Fact]
        public async void UpdateArticle_Success()
        {
            // Arrange

            var path = Path.Combine(Directory.GetCurrentDirectory(), "files", "icons", "Zoo.png");
            FileStream fsSource = new FileStream(path, FileMode.Open, FileAccess.Read);
            FormFile mockFile = new FormFile(fsSource, 0, fsSource.Length, "Zoo", "Zoo");


            var response = await _httpClient.GetAsync($"/api/GoodsAdmin/get?id=1");
            var article = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(article);
            var cat_result = data["data"].ToString();

            var request = new MultipartFormDataContent
            {
                { new StringContent(cat_result),"article_json"},
                { new StreamContent(mockFile.OpenReadStream()),"images",mockFile.FileName },
            };

            // Act
            var result = await _httpClient.PutAsync("/api/GoodsAdmin/update", request);
            var content = await result.Content.ReadAsStringAsync();
            
            // Assert
            Assert.True(result.StatusCode == System.Net.HttpStatusCode.Accepted);

        }

        [Fact]
        public async void DeleteArticle_Fail()
        {
            // Act
            var result = await _httpClient.DeleteAsync("/api/GoodsAdmin/delete?id=9999999");

            // Assert
            Assert.True(result.StatusCode == System.Net.HttpStatusCode.InternalServerError);

        }

    }
}
