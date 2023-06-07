using System.Text;
using Newtonsoft.Json;

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
    
    [Fact]
    public async void GetAllCategoryArticlesPaged_Success()
    {
        // Arrange
        var pages = JsonConvert.SerializeObject(new int[] {1, 2, 3}); 
        
        // Act 
        var result = await _httpClient.PostAsync(
            "api/Goods/category-articles-paged?category_Id=2&goods_on_page=10&lang=uk",
            new StringContent(pages,
                Encoding.UTF8,
                "application/json"));
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void GetMainArticleData_Success()
    {
        // Arrange
        
        // Act 
        var result = await _httpClient.GetAsync(
            "api/Goods/get-main?goodsId=1&lang=uk");
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void GetCaharacteristic_Success()
    {
        // Arrange
        
        // Act 
        var result = await _httpClient.GetAsync(
            "api/Goods/get-characteristic?goodsId=1&lang=uk");
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void GetAllActionsGoods_Success()
    {
        // Arrange
        
        // Act 
        var result = await _httpClient.GetAsync(
            "api/Goods/all-actions?lang=uk&goods_on_page=7&page=2");
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void GetBestPointsGoods_Success()
    {
        // Arrange
        
        // Act 
        var result = await _httpClient.GetAsync(
            "api/Goods/best-points?lang=uk&goods_on_page=7&page=2");
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void GetMostFavories_Success()
    {
        // Arrange
        
        // Act 
        var result = await _httpClient.GetAsync(
            "api/Goods/most-favorites?lang=uk&goods_on_page=7&page=1");
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void GetMostAwaitable_Success()
    {
        // Arrange
        
        // Act 
        var result = await _httpClient.GetAsync(
            "api/Goods/most-awaitable?lang=uk&goods_on_page=7&page=1");
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void GetFromList_Success()
    {
        // Arrange
        var articles = JsonConvert.SerializeObject(new int[] {1, 2, 3}); 
        
        // Act 
        var result = await _httpClient.PostAsync(
            "api/Goods/from-list?lang=uk",
            new StringContent(articles,
                Encoding.UTF8,
                "application/json"));
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void GetFromListLight_Success()
    {
        // Arrange
        var articles = JsonConvert.SerializeObject(new int[] {1, 2, 3}); 
        
        // Act 
        var result = await _httpClient.PostAsync(
            "api/Goods/from-list-light?lang=uk",
            new StringContent(articles,
                Encoding.UTF8,
                "application/json"));
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    
    [Fact]
    public async void GetCartItem_Success()
    {
        // Arrange
        
        // Act 
        var result = await _httpClient.GetAsync(
            "api/Goods/cart-item?lang=uk&id=1");
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
}