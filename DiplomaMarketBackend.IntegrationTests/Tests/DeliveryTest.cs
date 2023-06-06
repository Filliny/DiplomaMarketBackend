namespace DiplomaMarketBackend.IntegrationTests.Tests;

public class DeliveryTest:BasicTest
{

    [Fact]
    public async void GetDeliveries_Success()
    {
        // Arrange 
        // Act
        var result = await _httpClient.GetAsync("/api/Delivery/deliveries?lang=uk");
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void SearchCity_Success()
    {
        // Arrange 
        
        // Act
        var result = await _httpClient.GetAsync("/api/Delivery/city?lang=uk");
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void GetCityDeliveries_Success()
    {
        // Arrange 
        var cityId = 222;
        
        // Act
        var result = await _httpClient.GetAsync($"/api/Delivery/city-deliveries?city_id={cityId}&lang=uk");
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void GetDeliveryBranches_Success()
    {
        // Arrange 
        var cityId = 222;
        var deliveryId = 2;
        
        // Act
        var result = await _httpClient.GetAsync($"/api/Delivery/city-deliveries?city_id={cityId}&delivery_id={deliveryId}&lang=uk");
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    
    
}