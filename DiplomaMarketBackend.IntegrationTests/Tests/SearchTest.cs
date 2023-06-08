namespace DiplomaMarketBackend.IntegrationTests.Tests;

public class SearchTest:BasicTest
{
    [Fact]
    public async void MainSearch_Success()
    {
        // Arrange
        // Act
        var result = await _httpClient.GetAsync("api/Search/main?search=a&limit=5&lang=uk");
        
        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
}