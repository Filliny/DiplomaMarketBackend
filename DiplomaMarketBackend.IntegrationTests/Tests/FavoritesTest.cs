using DiplomaMarketBackend.Models;
using Newtonsoft.Json;
using System.Text;

namespace DiplomaMarketBackend.IntegrationTests.Tests;

public class FavoritesTest:BasicTest
{
    public FavoritesTest()
            : base()
    {
        Authorize();
    }
    private void Authorize()
    {

        var user = new User
        {
            email = AdminMail,
            password = AdminPass,
        };

        var response = _httpClient.PostAsync($"/authentication/Auth", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")).Result;

        var result = response.Content.ReadAsStringAsync().Result;
        var data = JsonConvert.DeserializeObject<dynamic>(result);
        if (data != null)
            _jwtToken = data["jwt"];
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);
    }

    [Fact]
    public async void Get_Success()
    {
        // Arrange
        // Act
        var result = await _httpClient.GetAsync("api/Favorites/get");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async void Add_Success()
    {
        // Arrange

        // Act
        var result = await _httpClient.PostAsync("/api/Favorites/add?article_id=1",new StringContent(""));

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void Remove_Success()
    {
        // Arrange

        // Act
        var result = await _httpClient.DeleteAsync("api/Favorites/remove?article_id=1");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }

}