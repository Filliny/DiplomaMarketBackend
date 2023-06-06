namespace DiplomaMarketBackend.IntegrationTests.Tests;

public class GoodsTest:BasicTest
{
    [Fact]
    public async void GetCategoryArticles_Success()
    {
        // Arrange
        // Act 
        var result = await _httpClient.GetAsync("api/Goods/category-articles?category_Id=2&lang=uk");
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void GetAllCategoryArticles_Success()
    {
        // Arrange
        // Act 
        var result = await _httpClient.GetAsync("api/Goods/get?category_Id=2&lang=uk");
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
}