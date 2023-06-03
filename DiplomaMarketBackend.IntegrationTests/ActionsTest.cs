using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DiplomaMarketBackend.IntegrationTests
{
    public class ActionsTest
    {
        private readonly HttpClient _httpClient;
        private static int? _id;
        public ActionsTest()
        {
            var webApplicationFactory = new CustomWebApplicationFactory<Program>();
            _httpClient = webApplicationFactory.CreateDefaultClient();
        }

        [Fact]
        public async void GetActions_Success()
        {
            // Arrange

            // Act
            var result = await _httpClient.GetAsync("/api/Actions/all?lang=uk");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }

        [Fact]
        public async void GetAction_Success()
        {
            // Arrange
            var response = await _httpClient.GetAsync("/api/Actions/all?lang=uk");
            var data = await response.Content.ReadAsStringAsync();
            var actions = JsonConvert.DeserializeObject<List<dynamic>>(data);
            var max_id = actions.Max(x => (int)x["id"]);


            // Act
            var result = await _httpClient.GetAsync($"/api/Actions/get?action_id={max_id}");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }

        [Fact]
        public async void CreateAction_Success()
        {
            // Arrange

            var path = Path.Combine(Directory.GetCurrentDirectory(), "files", "icons", "Зоотовари.png");
            FileStream fsSource = new FileStream(path, FileMode.Open, FileAccess.Read);
            FileStream fsSource1 = new FileStream(path, FileMode.Open, FileAccess.Read);
            FormFile mockFile = new FormFile(fsSource, 0, fsSource.Length, "Зоотовари", "Зоотовари");
            FormFile mockFile1 = new FormFile(fsSource1, 0, fsSource1.Length, "Зоотовари", "Зоотовари");


            var response = await _httpClient.GetAsync($"/api/Actions/get?action_id={1}");
            var data = await response.Content.ReadAsStringAsync();
            data = data.Replace("active", "ended");

            var request = new MultipartFormDataContent
            {
                { new StringContent(data),"action_json"},
                { new StreamContent(mockFile.OpenReadStream()),"big_banner",mockFile.FileName },
                { new StreamContent(mockFile1.OpenReadStream()),"small_banner",mockFile1.FileName },

            };

            // Act
            var result = await _httpClient.PostAsync($"/api/Actions/create", request);
            result = await _httpClient.PostAsync($"/api/Actions/create", request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);

        }


        [Fact]
        public async void UpdateAction_Success()
        {
            // Arrange

            var path = Path.Combine(Directory.GetCurrentDirectory(), "files", "icons", "Зоотовари.png");
            FileStream fsSource = new FileStream(path, FileMode.Open, FileAccess.Read);
            FileStream fsSource1 = new FileStream(path, FileMode.Open, FileAccess.Read);
            FormFile mockFile = new FormFile(fsSource, 0, fsSource.Length, "Зоотовари", "Зоотовари");
            FormFile mockFile1 = new FormFile(fsSource1, 0, fsSource1.Length, "Зоотовари", "Зоотовари");

            var maxidresponse = await _httpClient.GetAsync("/api/Actions/all?lang=uk");
            var mdata = await maxidresponse.Content.ReadAsStringAsync();
            var actions = JsonConvert.DeserializeObject<List<dynamic>>(mdata);
            var max_id = actions.Max(x => (int)x["id"]);

            var response = await _httpClient.GetAsync($"/api/Actions/get?action_id={max_id}");
            var data = await response.Content.ReadAsStringAsync();
            data = data.Replace("ended", "active");

            var request = new MultipartFormDataContent
            {
                { new StringContent(data),"action_json"},
                { new StreamContent(mockFile.OpenReadStream()),"big_banner",mockFile.FileName },
                { new StreamContent(mockFile1.OpenReadStream()),"small_banner",mockFile1.FileName },

            };

            // Act
            var result = await _httpClient.PutAsync($"/api/Actions/update", request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);

        }


        [Fact]
        public async void DeleteAction_Success()
        {
            // Arrange
            var response = await _httpClient.GetAsync("/api/Actions/all?lang=uk");
            var data = await response.Content.ReadAsStringAsync();
            var actions = JsonConvert.DeserializeObject<List<dynamic>>(data);
            var ended = actions.First(a => ((string)a["status"]).Equals("ended"));
            var max_id = (int)ended["id"];


            // Act
            var result = await _httpClient.DeleteAsync($"/api/Actions/delete?action_id={max_id}");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);

        }
    }
}
