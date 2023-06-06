using System.Net;
using System.Text;
using DiplomaMarketBackend.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace DiplomaMarketBackend.IntegrationTests.Tests;

[TestCaseOrderer(
    ordererTypeName: "DiplomaMarketBackend.IntegrationTests.Helpers.AlphabeticalOrderer",
    ordererAssemblyName: "DiplomaMarketBackend.IntegrationTests")]
public class UserAdminTest:BasicTest
{
    private static string? _userId = string.Empty;
    private static string? _userPhone = string.Empty;

    [Fact]
    public async void Admin1_AuthorizeTest_Success()
    {
        // Arrange
        var user = new User
        {
            email = AdminMail,
            password = AdminPass,
        };

        // Act
        var response = await _httpClient.PostAsync($"/authentication/Auth", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));

        var result = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<dynamic>(result);
        if (data != null)
            _jwtToken = data["jwt"];
        // Assert
        Assert.Equal("OK", response.ReasonPhrase);
        Assert.NotNull(data);
        Assert.True(!_jwtToken.IsNullOrEmpty());
    }

    [Fact]
    public async void Admin2_CreateUserTest_Success()
    {
        // Arrange
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);

        var email = "";
        HttpResponseMessage search_result = new();
        do
        {
            email = Faker.Internet.Email();
            search_result = await _httpClient.GetAsync($"/api/UsersAdmin/get-email?email={email}");

        } while (search_result.IsSuccessStatusCode);

        var user = new UserFull
        {
            user_name = Faker.Name.FullName(),
            email = email,
            first_name = Faker.Name.First(),
            last_name = Faker.Name.Last(),
            middle_name = Faker.Name.Middle(),
            email_notify = false,
            password = "PassWoRd16$",
            birth_day = Faker.Identification.DateOfBirth(),
            phone_number = Faker.Phone.Number(),
            customer_group_id = 2,
            //roles = new string[] { "user", "manager" },
            preferred_language_id = "UK",

        };

        _userPhone = user.phone_number; //save for update check


        // Act
        var response = await _httpClient.PostAsync($"/api/UsersAdmin/create", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));

        var result = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<dynamic>(result);

        var entity_data = data["entity"].ToString();

        UserFull userData = JsonConvert.DeserializeObject<UserFull>(entity_data);

        _userId = userData.Id;
            
        // Assert
        Assert.True(!_userId.IsNullOrEmpty());
        Assert.True(userData.roles.Count == 2);
        Assert.True(userData.roles.Contains("CategoriesWrite"));
        Assert.True(userData.roles.Contains("CategoriesRead"));
            

    }

    [Fact]
    public async void Admin3_UpdateUser_Success()
    {
        // Arrange 
        var response = await _httpClient.GetAsync($"/api/UsersAdmin/get?user_id={_userId}");
        UserFull? user = new UserFull();

        if (response != null && response.IsSuccessStatusCode)
        {
            var serial = await response.Content.ReadAsStringAsync();
            user = JsonConvert.DeserializeObject<UserFull>(serial) ?? new UserFull();
            user.phone_number = Faker.Phone.Number();
            user.customer_group_id = 1;
        }

        // Act
        response = await _httpClient.PutAsync($"/api/UsersAdmin/update", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));

        response = await _httpClient.GetAsync($"/api/UsersAdmin/get?user_id={_userId}");

        if (response != null && response.StatusCode == HttpStatusCode.OK)
        {
            var serial = await response.Content.ReadAsStringAsync();
            user = JsonConvert.DeserializeObject<UserFull>(serial) ?? new UserFull();
        }

        Assert.NotEqual(_userPhone, user.phone_number);
        Assert.True(user.roles.Count == 28);
    }

    [Fact]
    public async void Admin4_DeleteUser_Success() 
    {
        var response = await _httpClient.DeleteAsync($"/api/UsersAdmin/delete?user_id={_userId}");

        Assert.True(response.IsSuccessStatusCode);
    }
}