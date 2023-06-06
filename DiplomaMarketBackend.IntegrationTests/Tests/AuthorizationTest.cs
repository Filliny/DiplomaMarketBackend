using System.Text;
using DiplomaMarketBackend.IntegrationTests.Helpers;
using DiplomaMarketBackend.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace DiplomaMarketBackend.IntegrationTests.Tests
{
    public class AuthorizationTest : BasicTest
    {
        [Fact]
        public async void Authorization_Success()
        {
            // Arrange
            var user = new User
            {
                email = AdminMail,
                password = AdminPass,
            };

            // Act
            var response = await _httpClient.PostAsync($"/authentication/Auth",
                new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));

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