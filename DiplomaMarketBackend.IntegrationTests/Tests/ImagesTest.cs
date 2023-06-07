using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiplomaMarketBackend.IntegrationTests.Tests;

public class ImagesTest:BasicTest
{
    [Fact]
    public async void GetImage()
    {
        // Arrange
        var response = await _httpClient.GetAsync("api/Goods/get-main?lang=uk&goodsId=1");
        var content = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<dynamic>(content);
        data = data["data"];
        JArray pictures = data["images"];
        var url = pictures.First()["preview"]?["url"]?.ToString();
        var req_str = url.Substring(url.LastIndexOf('/'));
        
        // Act
        var result = await _httpClient.GetAsync($"api/Goods/preview" + req_str);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
}