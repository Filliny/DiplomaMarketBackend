

using System.Text;
using Faker;
using Newtonsoft.Json;

namespace DiplomaMarketBackend.IntegrationTests.Tests;

public class ServicesTest:BasicTest
{
    [Fact]
    public async void CreateService_Sucess()
    {
        // Arrange
        var service = new
        {
            name = Faker.Company.Name(),
            address=Faker.Address.ZipCode()+" "+Faker.Address.Country()+" "+Faker.Address.City()+" "+Faker.Address.StreetAddress(),
            phone=Faker.Phone.Number(),
            work_hours=Faker.Lorem.Sentence(),
            city_id = 301,
            brand_id=1,
            category_id=2
        };

        var content = new StringContent(JsonConvert.SerializeObject(service), Encoding.UTF8, "application/json");
        
        // Act
        var result = await _httpClient.PostAsync("api/Services/create", content);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void UpdateService_Sucess()
    {
        // Arrange
        var service = new
        {
            id=1,
            name = Faker.Company.Name(),
            address=Faker.Address.ZipCode()+" "+Faker.Address.Country()+" "+Faker.Address.City()+" "+Faker.Address.StreetAddress(),
            phone=Faker.Phone.Number(),
            work_hours=Faker.Lorem.Sentence(),
            city_id = 300,
            brand_id=1,
            category_id=2
        };

        var content = new StringContent(JsonConvert.SerializeObject(service), Encoding.UTF8, "application/json");
        
        // Act
        var result = await _httpClient.PutAsync("api/Services/update", content);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void DeleteService_Sucess()
    {
        // Arrange
        var response = await _httpClient.GetAsync("api/Services/get-by-brand?brand_id=1");
        var content = await response.Content.ReadAsStringAsync();
        var lines = JsonConvert.DeserializeObject<List<dynamic>>(content);
        var id = lines.Max(s => s["id"]);
        
        // Act
        var result = await _httpClient.DeleteAsync($"api/Services/delete?service_id={id}");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    
    [Fact]
    public async void GetAllBrandServices_Sucess()
    {
        // Arrange
        
        // Act
        var result = await _httpClient.GetAsync("api/Services/get-by-brand?brand_id=1");
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);

    } 
    
    [Fact]
    public async void GetById_Success()
    {
        // Arrange
        
        // Act
        var result = await _httpClient.GetAsync("api/Services/get-by-id?service_id=1");
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);

    } 
}