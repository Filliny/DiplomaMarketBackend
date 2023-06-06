namespace DiplomaMarketBackend.IntegrationTests.Tests;

public class FavoritesTest:BasicTest
{
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