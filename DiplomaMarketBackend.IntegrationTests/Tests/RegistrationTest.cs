using System.Text;
using DiplomaMarketBackend.IntegrationTests.Helpers;
using Newtonsoft.Json;

namespace DiplomaMarketBackend.IntegrationTests.Tests;

[TestCaseOrderer(
    ordererTypeName: "DiplomaMarketBackend.IntegrationTests.Helpers.AlphabeticalOrderer",
    ordererAssemblyName: "DiplomaMarketBackend.IntegrationTests")]
public class RegistrationTest:BasicTest
{
    private static string? email;
    private static string? code;

    public RegistrationTest()
        :base()
    {
        Authorize();
    }
    private void Authorize()
    {
        var user = new 
        {
            email = AdminMail,
            password = AdminPass,
        };

        var response =  _httpClient.PostAsync($"/authentication/Auth", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")).Result;

        var result = response.Content.ReadAsStringAsync().Result;
        var data = JsonConvert.DeserializeObject<dynamic>(result);
        if (data != null)
            _jwtToken = data["jwt"];
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);
    }
    

    [Fact]
    public async void Test1_RegisterUser_Success()
    {
        // Arrange
        
        var user = new
        {
            user_name = Faker.Internet.Email(),
            email = Faker.Internet.Email(),
            password = AdminPass
        };
        email = user.email;

        var data = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
        
        // Act
        var result = await _httpClient.PostAsync("authentication/Reg/register-user", data);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);

    }

    [Fact]
    public async void Test2_PasswordRecovery_Success()
    {
        // Arrange
        string frontCallback = "https://nasoloda.pp.ua/password-recovery";
        string user_mail = email ?? AdminMail;
        
        // Act
        var result = await _httpClient.
            PostAsync($"authentication/Reg/pass-recovery?email={user_mail}&front_callback={frontCallback}",
                new StringContent(""));
        var content = await result.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<dynamic>(content);

        var entity = JsonConvert.DeserializeObject<dynamic>(data["entity"].ToString());
        code = entity["code"].ToString();
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(code);

    }

    [Fact]
    public async void Test3_RecoveryConfirm_Success()
    {
        // Arrange

        var form = new
        {
            email,
            new_password = AdminPass,
            email_code = code
        };
        
        //Act

        var result = await _httpClient.PostAsync("authentication/Reg/recovery_confirm", form.ToFormData());
        var contetnt = result.Content.ReadAsStringAsync();
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);



    }

    [Fact]
    public async void Test4_UpdatePassword_Success()
    {
        // Arrange
        var data = new
        {
            old_password = AdminPass,
            new_password = AdminPass
        };
        
        // Act
        var result = await _httpClient.PostAsync("authentication/Reg/pass-update",
            data.ToFormData());

        var content = await result.Content.ReadAsStringAsync();
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }

}