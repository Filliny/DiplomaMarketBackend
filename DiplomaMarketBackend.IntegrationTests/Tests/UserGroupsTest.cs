using System.Text;
using DiplomaMarketBackend.Models;
using Newtonsoft.Json;

namespace DiplomaMarketBackend.IntegrationTests.Tests
{
    public class UsersGroupsTest:BasicTest
    {
        private static int _groupId;

        public UsersGroupsTest()
             : base()
        {
            Authorize();
        }
        private void Authorize()
        {

            var user = new User
            {
                email = AdminMail,
                password = AdminPass,
            };

            var response = _httpClient.PostAsync($"/authentication/Auth", new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")).Result;

            var result = response.Content.ReadAsStringAsync().Result;
            var data = JsonConvert.DeserializeObject<dynamic>(result);
            if (data != null)
                _jwtToken = data["jwt"];
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);
        }

        [Fact]
        public async void GetGroups_Success()
        {
            //Arran
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
            HttpResponseMessage response = new();
            
            var permRes = await _httpClient.GetAsync($"/api/Groups/permissions");
            var permCont = await permRes.Content.ReadAsStringAsync();
            var perms = JsonConvert.DeserializeObject<List<dynamic>>(permCont)??new List<dynamic>();

            var grp = new
            {
                name = "TestAdmin",
                permissions = new List<dynamic>()
            };
            
            foreach (var perm in perms)
            {
                grp.permissions.Add(new
                {
                    id=perm["id"],
                    read_allowed=true,
                    write_allowed=true,
                });
            }

            var data = JsonConvert.SerializeObject(grp);
            
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
            HttpResponseMessage response = new();
            HttpResponseMessage next = new();
            response = await _httpClient.GetAsync($"/api/Groups/groups");

            var data = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<dynamic>>(data);
            var id = list.Max(d => d["id"]);

            // Act
            response = await _httpClient.DeleteAsync($"/api/Groups/delete?group_id={id}");

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccessStatusCode);
        }

    }
}
