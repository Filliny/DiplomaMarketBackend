using System.Text;
using Newtonsoft.Json;

namespace DiplomaMarketBackend.IntegrationTests.Tests;

public class StorageTest : BasicTest
{
    [Fact]
    public async void GetRefillList_Success()
    {
        // Arrange
        // Act
        var result = await _httpClient.GetAsync("api/Storage/refill_list?lang=uk");
        
        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    
    
    [Fact]
    public async void GetClosedOrders_Success()
    {
        // Arrange
        // Act
        var result = await _httpClient.GetAsync("api/Storage/closed-list?lang=uk");
        var content = await result.Content.ReadAsStringAsync();
        
        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void ChangeStatus_Success()
    {
        // Arrange
        // Act
        var result = await _httpClient.PutAsync("api/Storage/change-status?order_id=10&status=2",new StringContent(""));
        var content = await result.Content.ReadAsStringAsync();
        
        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void CloseOrders_Success()
    {
        // Arrange
        var orders = new [] { 10 };
        var content = JsonConvert.SerializeObject(orders);
        // Act
        var result = await _httpClient.PutAsync("api/Storage/close-orders",new StringContent(content,Encoding.UTF8,"application/json"));
        content = await result.Content.ReadAsStringAsync();
        
        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
}