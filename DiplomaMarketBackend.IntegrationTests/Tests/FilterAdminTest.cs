using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;

namespace DiplomaMarketBackend.IntegrationTests.Tests;

public class FilterAdminTest:BasicTest
{
    [Fact]
    public async void GetList_Success()
    {
        // Arrange
        // Act
        var result = await _httpClient.GetAsync("api/FiltersAdmin/get-list");
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void GetCandidates_Success()
    {
        // Arrange
        // Act
        var result = await _httpClient.GetAsync("api/FiltersAdmin/get-candidates");
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void GetSettings_Success()
    {
        // Arrange
        // Act
        var result = await _httpClient.GetAsync("api/FiltersAdmin/get-settings?category_id=1");
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void CreateSetting_Success()
    {
        // Arrange
        int id=1;
        dynamic? data;
        HttpResponseMessage? setResult;
        do
        {
            setResult = await _httpClient.GetAsync($"api/FiltersAdmin/get-settings?category_id={id}");
            var setStr = await setResult.Content.ReadAsStringAsync();
            if (setStr == null) throw new ArgumentNullException(nameof(setStr));
            data = JsonConvert.DeserializeObject<dynamic>(setStr);
            id++;
            
        } while (data?["category_name"] != null);


        if (data != null)
        {
            data["category_id"] = id-1;
        }
        string str = JsonConvert.SerializeObject(data);
        
        // Act
        var result = await _httpClient.
            PostAsync("api/FiltersAdmin/create", 
                new StringContent(str
                    ,
                    Encoding.UTF8,
                    "application/json"
                    ));
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
       
    }
    
    [Fact]
    public async void UpdateSetting_Success()
    {
        // Arrange
        int id=1;
        dynamic? data;
        HttpResponseMessage? setResult;
        do
        {
            setResult = await _httpClient.GetAsync($"api/FiltersAdmin/get-settings?category_id={id}");
            var setStr = await setResult.Content.ReadAsStringAsync();
            if (setStr == null) throw new ArgumentNullException(nameof(setStr));
            data = JsonConvert.DeserializeObject<dynamic>(setStr);
            id++;
            
        } while (data?["category_name"] == null);


        if (data != null)   
        {
            data["show_price"] = false;
        }
        string str = JsonConvert.SerializeObject(data);
        
        // Act
        var result = await _httpClient.
            PutAsync("api/FiltersAdmin/update", 
                new StringContent(
                    str,
                    Encoding.UTF8,
                    "application/json"
                ));
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
       
    }

    [Fact]
    public async void GetHiddenFilters_Success()
    {
        // Arrange
        
        // Act 
        var result = await _httpClient.
            GetAsync("api/FiltersAdmin/get-unselected?category_id=2&lang=uk");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void GetShowedFilters_Success()
    {
        // Arrange
        
        // Act 
        var result = await _httpClient.
            GetAsync("api/FiltersAdmin/get-selected?category_id=2&lang=uk");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void UpdateList_Success()
    {
        // Arrange
        var list_result = await _httpClient.
            GetAsync("api/FiltersAdmin/get-unselected?category_id=2&lang=uk");
        var content = await list_result.Content.ReadAsStringAsync();

        // Act 
        var result = await _httpClient.
            PatchAsync("api/FiltersAdmin/filters-update",
                new StringContent(content,Encoding.UTF8,"application/json"));

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
}