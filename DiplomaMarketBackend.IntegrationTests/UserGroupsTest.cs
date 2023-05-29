using DiplomaMarketBackend.Models;
using Newtonsoft.Json;
using System.Text;

namespace DiplomaMarketBackend.IntegrationTests
{
    public class UsersGroupsTest
    {
        private readonly HttpClient _httpClient;
        private static string? _jwtToken = string.Empty;
        private static string? groupJson = string.Empty;
        private static int _groupId;
        private static string? _userPhone = string.Empty;

        private readonly string admin_mail = "admin@gmail.com";
        private readonly string admin_pass = "Test123admin$";


        public UsersGroupsTest()
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
        public async void GetGroups_Success()
        {
            //Arrange
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);

            // Act
            var response = await _httpClient.GetAsync($"/api/Groups/groups");

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async void GetPermissions_Success()
        {
            //Arrange
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);

            // Act
            var response = await _httpClient.GetAsync($"/api/Groups/permissions");

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async void GetGroup_Success()
        {
            //Arrange
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);

            // Act
            HttpResponseMessage response = new();
            int id = 1;
            do
            {
                response = await _httpClient.GetAsync($"/api/Groups/get-group?group_id={id}");
                id++;
            } while (!response.IsSuccessStatusCode);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async void CreateGroup_Success()
        {
            //Arrange
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);
            HttpResponseMessage response = new();
            int id = 1;
            do
            {
                response = await _httpClient.GetAsync($"/api/Groups/get-group?group_id={id}");
                id++;
            } while (!response.IsSuccessStatusCode);
            var data = await response.Content.ReadAsStringAsync();
            var msg = new StringContent(data, Encoding.UTF8, "application/json");
            
            // Act
            response  = await _httpClient.PostAsync($"/api/Groups/create",msg);
            var content = await response.Content.ReadAsStringAsync();
            var jdata = JsonConvert.DeserializeObject<dynamic>(content);
            if(jdata != null)
                _groupId = jdata["entity"]["id"];


            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccessStatusCode);
            Assert.True(_groupId != 0);
        }

        [Fact]
        public async void UpdateGroup_Success()
        {
            //Arrange
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);
            HttpResponseMessage response = new();
            HttpResponseMessage next = new();
            int id = 1;
            do //get last group when next will be  404
            {
                response = await _httpClient.GetAsync($"/api/Groups/get-group?group_id={id}");
                id++;
                next = await _httpClient.GetAsync($"/api/Groups/get-group?group_id={id}");

            } while (next.IsSuccessStatusCode);
            var data = await response.Content.ReadAsStringAsync();
            var msg = new StringContent(data, Encoding.UTF8, "application/json");

            // Act
            response = await _httpClient.PutAsync($"/api/Groups/update", msg);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async void DeleteGroup_Success()
        {
            //Arrange
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);
            HttpResponseMessage response = new();
            HttpResponseMessage next = new();
            int id = 1;
            do //get last group when next will be  404
            {
                response = await _httpClient.GetAsync($"/api/Groups/get-group?group_id={id}");
                id++;
                next = await _httpClient.GetAsync($"/api/Groups/get-group?group_id={id}");

            } while (next.IsSuccessStatusCode);
            var data = await response.Content.ReadAsStringAsync();
            var msg = new StringContent(data, Encoding.UTF8, "application/json");

            // Act
            response = await _httpClient.DeleteAsync($"/api/Groups/delete?group_id={id-1}");

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccessStatusCode);
        }

    }
}
