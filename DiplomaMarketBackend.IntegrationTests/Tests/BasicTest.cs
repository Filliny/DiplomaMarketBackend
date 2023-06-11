using System.Text;
using DiplomaMarketBackend.IntegrationTests.Helpers;
using DiplomaMarketBackend.Models;
using Newtonsoft.Json;

namespace DiplomaMarketBackend.IntegrationTests.Tests;

public class BasicTest
{
    protected readonly HttpClient _httpClient;
    protected string? _jwtToken = string.Empty;
    protected const string AdminMail = "admin@gmail.com";
    protected const string AdminPass = "Test123admin$";
    
    public BasicTest()
    {
        var webApplicationFactory = new CustomWebApplicationFactory<Program>();
        _httpClient = webApplicationFactory.CreateDefaultClient();

    }
    
    private async Task Authorize()
    {

        var user = new User
        {
            email = AdminMail,
            password = AdminPass,
        };

        var response = await _httpClient.PostAsync($"/authentication/Auth", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));

        var result = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<dynamic>(result);
        if (data != null)
            _jwtToken = data["jwt"];
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);
    }
}