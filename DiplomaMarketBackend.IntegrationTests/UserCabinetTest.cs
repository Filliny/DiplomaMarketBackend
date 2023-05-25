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
    public class UserCabinetTest
    {
        private readonly HttpClient _httpClient;
        private static string? _jwtToken = string.Empty;
        private static string? _userId = string.Empty;
        private static string? _userPhone = string.Empty;

        private readonly string admin_mail = "admin@gmail.com";
        private readonly string admin_pass = "Test123admin$";

        public UserCabinetTest()
        {
            var webApplicationFactory = new CustomWebApplicationFactory<Program>();
            _httpClient = webApplicationFactory.CreateDefaultClient();
            Authorize();

        }
        private void Authorize()
        {
            var user = new User
            {
                email = admin_mail,
                password = admin_pass,
            };

            var response = _httpClient.PostAsync($"/authentication/Auth", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")).Result;

            var result = response.Content.ReadAsStringAsync().Result;
            var data = JsonConvert.DeserializeObject<dynamic>(result);
            if (data != null)
                _jwtToken = data["jwt"];
        }

        [Fact]
        public async void GetUserData_Success()
        {
            // Arrange
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);

            // Act
            var response = await _httpClient.GetAsync($"/api/UsersCabinet/get");

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccessStatusCode);

        }


        [Fact]
        public async void UpdateUserData_Success()
        {
            // Arrange
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);
            var response = await _httpClient.GetAsync($"/api/UsersCabinet/get");
            var msg = await response.Content.ReadAsStringAsync();

            // Act
            var result = await _httpClient.PutAsync($"/api/UsersCabinet/update", new StringContent(msg, Encoding.UTF8, "application/json"));

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);

        }

        [Fact]
        public async void GetReceivers_Success()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);

            // Act
            var result = await _httpClient.GetAsync($"/api/UsersCabinet/get-receivers");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }

        [Fact]
        public async void CreateReceiver_Success()
        {
            // Arrange
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);

            //to generate 1 more receiver than will be deleted after tests - so next test will have already receiver
            HttpResponseMessage? result = null;

            for (int i = 0; i < 2; i++)
            {
                var receiver = new
                {
                    first_name = Faker.Name.First(),
                    last_name = Faker.Name.Last(),
                    middle_name = Faker.Name.Middle(),
                    profile_name = Faker.Name.First() + " " + Faker.Name.Last(),
                    email = Faker.Internet.FreeEmail(),
                    phone = Faker.Phone.Number(),
                };

                var msg = receiver.ToFormData();

                // Act
                result = await _httpClient.PostAsync($"/api/UsersCabinet/create-receiver", msg);
            }

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }

        [Fact]
        public async void UpdateReceiver_Success()
        {
            // Arrange
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);
            var responseMessage = await _httpClient.GetAsync($"/api/UsersCabinet/get-receivers");
            var content = await responseMessage.Content.ReadAsStringAsync();
            var recivers = JsonConvert.DeserializeObject<List<dynamic>>(content);

            // Act
            HttpResponseMessage? result = null;
            if (recivers != null)
            {
                var receiver = new
                {
                    id = recivers.First()["id"],
                    first_name = Faker.Name.First(),
                    last_name = Faker.Name.Last(),
                    middle_name = Faker.Name.Middle(),
                    profile_name = Faker.Name.First() + " " + Faker.Name.Last(),
                    email = Faker.Internet.FreeEmail(),
                    phone = Faker.Phone.Number(),
                };

                var msg = receiver.ToFormData();
                result = await _httpClient.PutAsync($"/api/UsersCabinet/update-receiver", msg);
            }

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }

        [Fact]
        public async void DeleteReceiver_Success()
        {
            // Arrange
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);
            var responseMessage = await _httpClient.GetAsync($"/api/UsersCabinet/get-receivers");
            var content = await responseMessage.Content.ReadAsStringAsync();
            var recivers = JsonConvert.DeserializeObject<List<dynamic>>(content);

            // Act
            HttpResponseMessage? result = null;
            if (recivers != null)
            {
                result = await _httpClient.DeleteAsync($"/api/UsersCabinet/delete-receiver?id={recivers.First()["id"]}");
            }

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }

        [Fact]
        public async void GetDeliveryAddress_Success()
        {
            // Arrange
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);

            // Act
            var result = await _httpClient.GetAsync($"/api/UsersCabinet/address");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }


        [Fact]
        public async void UpdateDeliveryAddress_Success()
        {
            // Arrange
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);

            var addr = new
            {
                city_name = Faker.Address.City(),
                street = Faker.Address.StreetName(),
                building = Faker.Address.StreetAddress(),
                apartment = Faker.Address.SecondaryAddress(),
            };

            var msg= addr.ToFormData();
            // Act
            var result = await _httpClient.PutAsync($"/api/UsersCabinet/address-update", msg);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }

        [Fact]
        public async void ContactsUpdate_Success()
        {
            // Arrange
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);

            var contacts = new
            {
                phone= Faker.Phone.Number(),
                email = Faker.Internet.Email(),
            };

            var msg = contacts.ToFormData();

            // Act
            var result = await _httpClient.PutAsync($"/api/UsersCabinet/contacts-update", msg);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }

        [Fact]
        public async void LoginChange_Success()
        {
            // Arrange
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);

            var contacts = new
            {
                phone = Faker.Phone.Number(),
                email = Faker.Internet.Email(),
            };

            var msg = contacts.ToFormData();

            // Act
            var result = await _httpClient.PostAsync($"/api/UsersCabinet/login-change?front_callback=http://nasoloda.pp.ua:3000/namecallback", msg);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }

        [Fact]
        public async void LoginChangeConfirm_Fail()
        {
            // Arrange
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);

            var contacts = new
            {
                new_email = Faker.Internet.Email(),
                email_code = "eertuyrtyjlkltyh45645uyhdrfhdhjrtu45rhedtwer23e523t",
            };

            var msg = contacts.ToFormData();

            // Act
            var result = await _httpClient.PostAsync($"/api/UsersCabinet/login-change-confirm",msg);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccessStatusCode);
        }
    }
}