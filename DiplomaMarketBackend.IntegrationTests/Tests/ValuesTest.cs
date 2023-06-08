

using System.Text;
using DiplomaMarketBackend.IntegrationTests.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiplomaMarketBackend.IntegrationTests.Tests;

public class ValuesTest:BasicTest
{
    [Fact]
    public async void GetCategoryCharacteristics_Success()
    {
        // Arrange
        //Act

        var result = await _httpClient.GetAsync("api/Values/category-characteristics?category_id=2&lang=uk");
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void GetCategoryCharacteristicsValues_Success()
    {
        // Arrange
        //Act

        var result = await _httpClient.GetAsync("api/Values/category-chars-values?category_id=2&lang=uk");
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
        
    [Fact]
    public async void GetCharacteristic_Success()
    {
        // Arrange
        //Act

        var result = await _httpClient.GetAsync("api/Values/charakteristic?id=1");
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
          
    [Fact]
    public async void GetOnlyValues_Success()
    {
        // Arrange
        //Act

        var result = await _httpClient.GetAsync("api/Values/characteristic-values?id=1&lang=uk");
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void CreateCharacteristic_Success()
    {
        // Arrange
        var characteristic = new
        {
            category_id=2,
            names = new Dictionary<string,string>{{"UK","NewCharacteristuic"},{"RU","NovayaCharacyteristica"}},
            values = new List<dynamic>
            {
                new{translations = new Dictionary<string,string>{{"UK","Value1"},{"RU","Znachenie1"}} },
                new{translations = new Dictionary<string,string>{{"UK","Value2"},{"RU","Znachenie2"}} }
            },
            group = new { id = 1 },
            is_active=true,
            comparable = "main",
            filter_type="checkbox",
            show_in_filter = true
        };

        var content = new StringContent(JsonConvert.SerializeObject(characteristic), Encoding.UTF8, "application/json");
        //Act

        var result = await _httpClient.PostAsync("api/Values/create",content);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void UpdateCharacteristic_Success()
    {
        // Arrange
        var response = await _httpClient.GetAsync("api/Values/category-characteristics?category_id=2&lang=uk");
        var resp_content = await response.Content.ReadAsStringAsync();
        JObject? data = JsonConvert.DeserializeObject<JObject>(resp_content);
        var list = JsonConvert.DeserializeObject<List<JObject>>(data["data"].ToString());
        var id = list?.Max(c => c["id"]);
        
        var characteristic = new
        {
            id,
            category_id=2,
            names = new Dictionary<string,string>{{"UK","NewCharacteristuicUpdate"},{"RU","NovayaCharacyteristicaUpdate"}},
            values = new List<dynamic>
            {
                new{translations = new Dictionary<string,string>{{"UK","Value1upd"},{"RU","Znachenie1upd"}} },
                new{translations = new Dictionary<string,string>{{"UK","Value2upd"},{"RU","Znachenie2upd"}} }
            },
            group = new { id = 1 },
            is_active=true,
            comparable = "main",
            filter_type="checkbox",
            show_in_filter = false
        };

        var content = new StringContent(JsonConvert.SerializeObject(characteristic), Encoding.UTF8, "application/json");
        
        //Act

        var result = await _httpClient.PutAsync("api/Values/update",content);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void DeleteCharacteristic_Success()
    {
        // Arrange
        var response = await _httpClient.GetAsync("api/Values/category-characteristics?category_id=2&lang=uk");
        var resp_content = await response.Content.ReadAsStringAsync();
        JObject? data = JsonConvert.DeserializeObject<JObject>(resp_content);
        var list = JsonConvert.DeserializeObject<List<JObject>>(data["data"].ToString());
        var id = list?.Max(c => c["id"]);
        
        //Act

        var result = await _httpClient.DeleteAsync($"api/Values/delete?id={id}");
        var content = result.Content.ReadAsStringAsync();
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void GetFilter_Success()
    {
        // Arrange
        //Act

        var result = await _httpClient.GetAsync("api/Values/characteristic-values?id=1&lang=uk");
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
}