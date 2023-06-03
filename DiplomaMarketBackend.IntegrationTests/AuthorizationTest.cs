using DiplomaMarketBackend.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomaMarketBackend.IntegrationTests
{
    public class AuthorizationTest
    {
        private readonly HttpClient _httpClient;
        private static string? _jwtToken = string.Empty;
        private static string? _userId = string.Empty;
        private static string? _userPhone = string.Empty;

        private readonly string admin_mail = "admin@gmail.com";
        private readonly string admin_pass = "Test123admin$";

        public AuthorizationTest()
        {
            var webApplicationFactory = new CustomWebApplicationFactory<Program>();
            _httpClient = webApplicationFactory.CreateDefaultClient();
        }

        [Fact]
        public async void Authorization_Sucess()
        {
            // Arrange
            var user = new User
            {
                email = admin_mail,
                password = admin_pass,
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
    }
}
