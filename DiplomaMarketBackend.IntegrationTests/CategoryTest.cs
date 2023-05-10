using DiplomaMarketBackend.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Text;

namespace DiplomaMarketBackend.IntegrationTests
{
    [TestCaseOrderer(
    ordererTypeName: "DiplomaMarketBackend.IntegrationTests.AlphabeticalOrderer",
    ordererAssemblyName: "DiplomaMarketBackend.IntegrationTests")]
    public class CategoryTest
    {
        private readonly HttpClient _httpClient;
        private static int? _id;
        public CategoryTest()
        {
            var webApplicationFactory = new CustomWebApplicationFactory<Program>();
            _httpClient = webApplicationFactory.CreateDefaultClient();
        }

        [Fact]
        public async void CategoryTreeGet_Success()
        {
            var response = await _httpClient.GetAsync($"/api/Categories/full?lang=UK");
            var result = await response.Content.ReadAsStringAsync();

            dynamic? jdata = JsonConvert.DeserializeObject<dynamic>(result);

            if (jdata == null) Assert.Fail("Json result malformed or absent");

            var data = jdata["data"];
            data = data.ToString().Trim().TrimStart('{').TrimEnd('}');
            var categories = JsonConvert.DeserializeObject<List<OutCategory>>(data);

            Assert.True(!string.IsNullOrEmpty(result));
            Assert.True(categories.Count != 0);

        }

        [Theory]
        [InlineData(89)]
        public async void GetParentCandidates_Success(int? id)
        {
            var response = await _httpClient.GetAsync($"/api/Categories/parent-candidates?category_id={id}&lang=UK");
            var result = await response.Content.ReadAsStringAsync();



            dynamic? jdata = JsonConvert.DeserializeObject<dynamic>(result);

            if (jdata == null) Assert.Fail("Json result malformed or absent");

            var data = jdata["data"];
            data = data.ToString().Trim().TrimStart('{').TrimEnd('}');
            List<dynamic> categories = JsonConvert.DeserializeObject<List<dynamic>>(data);

            foreach (dynamic category in categories)
            {
                var idsf = (int)category.id;
            }

            Assert.True(!string.IsNullOrEmpty(result));
            Assert.DoesNotContain(categories, c => (int)c.id == id);//check that list does not contain given category
        }



        [Fact]
        public async void Admin1_CreateCategory_Success()
        {
            // Arrange

            var path = Path.Combine(Directory.GetCurrentDirectory(), "files", "icons", "Зоотовари.png");
            FileStream fsSource = new FileStream(path, FileMode.Open, FileAccess.Read);
            FileStream fsSource2 = new FileStream(path, FileMode.Open, FileAccess.Read);
            FormFile mockFile = new FormFile(fsSource, 0, fsSource.Length, "Зоотовари", "Зоотовари");
            fsSource.Position = 0;
            FormFile mockFile2 = new FormFile(fsSource2, 0, fsSource.Length, "Зоотовари", "Зоотовари");


            Category category = new Category
            {
                names = "{\"UK\":\"Тестова категорія\",\"RU\":\"Тестовая категория\"}",
                parent_id = 1,
                showin_category_id = 58,
                root_icon = mockFile,
                category_image = mockFile,
                is_active = false,
            };

            var content = new MultipartFormDataContent
            {
                { new StringContent(category.names, Encoding.UTF8, MediaTypeNames.Text.Plain), "names" },
                { new StringContent(category.parent_id.ToString()??"", Encoding.UTF8, MediaTypeNames.Text.Plain), "parent_id" },
                { new StringContent(category.showin_category_id.ToString()??"", Encoding.UTF8, MediaTypeNames.Text.Plain), "showin_category_id" },
                { new StringContent(category.is_active.ToString()??"", Encoding.UTF8, MediaTypeNames.Text.Plain), "is_active" },
                { new StreamContent(mockFile.OpenReadStream()), "root_icon", mockFile.FileName },
                { new StreamContent(mockFile2.OpenReadStream()), "category_image", mockFile.FileName }
            };

            // Act

            var response = await _httpClient.PostAsync($"/api/Categories/create", content);
            var result = await response.Content.ReadAsStringAsync();
            var data = new Category();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                data = JsonConvert.DeserializeObject<Category>(result);

            if (data != null) _id = data.id; //save income id to further tests


            // Assert
            Assert.True(!string.IsNullOrEmpty(result));
            Assert.NotNull(data);
            Assert.NotEqual(0, data.id);

        }


        [Fact]
        public async void Admin2_TestCreatedCategory_Sucess()
        {

            // Arrange

            // Act
            var response = await _httpClient.GetAsync($"/api/Categories/category?category_id={_id}&lang=UK");
            var result = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<Category>(result);


            //Assert
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(!string.IsNullOrEmpty(result));
            Assert.NotNull(data);
            Assert.NotNull(data.existing_image);
            Assert.NotNull(data.existing_icon);
        }

        [Fact]
        public async void Admin3_UpdateCategory_Sucess()
        {


            var path = Path.Combine(Directory.GetCurrentDirectory(), "files", "icons", "Зоотовари.png");
            FileStream fsSource = new FileStream(path, FileMode.Open, FileAccess.Read);
            FileStream fsSource2 = new FileStream(path, FileMode.Open, FileAccess.Read);
            FormFile mockFile = new FormFile(fsSource, 0, fsSource.Length, "Зоотовари", "Зоотовари");
            fsSource.Position = 0;
            FormFile mockFile2 = new FormFile(fsSource2, 0, fsSource.Length, "Зоотовари", "Зоотовари");


            Category category = new Category
            {
                names = "{\"UK\":\"Тестова категорія update\",\"RU\":\"Тестовая категория\"}",
                parent_id = 1,
                showin_category_id = 58,
                root_icon = mockFile,
                category_image = mockFile,
                is_active = true,
            };

            var content = new MultipartFormDataContent
            {
                { new StringContent(_id.ToString()??"", Encoding.UTF8, MediaTypeNames.Text.Plain), "id" },
                { new StringContent(category.names, Encoding.UTF8, MediaTypeNames.Text.Plain), "names" },
                { new StringContent(category.parent_id.ToString()??"", Encoding.UTF8, MediaTypeNames.Text.Plain), "parent_id" },
                { new StringContent(category.showin_category_id.ToString()??"", Encoding.UTF8, MediaTypeNames.Text.Plain), "showin_category_id" },
                { new StringContent(category.is_active.ToString()??"", Encoding.UTF8, MediaTypeNames.Text.Plain), "is_active" },
                { new StreamContent(mockFile.OpenReadStream()), "root_icon", mockFile.FileName },
                { new StreamContent(mockFile2.OpenReadStream()), "category_image", mockFile.FileName }
            };

            // Act

            var response = await _httpClient.PutAsync($"/api/Categories/update", content);
            var result = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.True(!string.IsNullOrEmpty(result));
            Assert.True(response.IsSuccessStatusCode);


        }

        [Fact]
        public async void Admin4_TestUpdatedCategory_Sucess()
        {

            // Arrange

            // Act
            var response = await _httpClient.GetAsync($"/api/Categories/category?category_id={_id}&lang=UK");
            var result = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<Category>(result);


            //Assert
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(!string.IsNullOrEmpty(result));
            Assert.NotNull(data);
            Assert.Contains("update", data.names);

        }

        [Fact]
        public async void Admin5_TestDeleteCategory_Sucess()
        {
            // Arrange

            // Act
            var response = await _httpClient.DeleteAsync($"/api/Categories/delete?category_id={_id}");

            //Assert
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);

        }


    }
}