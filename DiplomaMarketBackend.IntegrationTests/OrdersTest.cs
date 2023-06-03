using DiplomaMarketBackend.Entity.Models;
using DiplomaMarketBackend.Models;
using Newtonsoft.Json;

namespace DiplomaMarketBackend.IntegrationTests
{
    public class OrdersTest
    {

        private readonly HttpClient _httpClient;
        private static int? _id;
        public OrdersTest()
        {
            var webApplicationFactory = new CustomWebApplicationFactory<Program>();
            _httpClient = webApplicationFactory.CreateDefaultClient();
        }

        [Fact]
        public async void CheckCertificate_Success()
        {
            // Arrange
            var code = "11110000";

            // Act
            var result = await _httpClient.GetAsync($"/api/Orders/certificate-check?certificate_code={code}");

            // Assert
            Assert.NotNull( result );
            Assert.True(result.IsSuccessStatusCode );
        }

        [Fact]
        public async void CheckPromo_Success()
        {
            // Arrange
            var code = "LADNAHATA";

            // Act
            var result = await _httpClient.GetAsync($"/api/Orders/promo-check?promo_code={code}");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }

        [Fact]
        public async void GetPaymentMethods_Success()
        {
            // Arrange

            // Act
            var result = await _httpClient.GetAsync($"/api/Orders/payment-methods?lang=uk");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }

        [Fact]
        public async void CreateNewOrderLogged_Success()
        {
            // Arrange
            var resp = await _httpClient.PostAsync("/authentication/Auth", new StringContent("{\r\n  \"email\": \"admin@gmail.com\",\r\n  \"password\": \"Test123admin$\"\r\n}",System.Text.Encoding.UTF8,"application/json"));
            var data  = await  resp.Content.ReadAsStringAsync();
            var jdata = JsonConvert.DeserializeObject<dynamic>(data);
            string? jwt = null;
            if ( jdata != null) jwt = jdata["jwt"];
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            var order= new Order { 
            
                receiver = new Order.Receiver { first_name = Faker.Name.First(),last_name=Faker.Name.Last(),middle_name =Faker.Name.Last(),profile_name="",email=Faker.Internet.Email(),phone = Faker.Phone.Number() },
                payData = new Order.PayData { payment_type_id = 2},

                goods = new List<Order.Item> { new Order.Item { article_id=1,price=1500,quantity=1 },new Order.Item {article_id=2,price=1000,quantity=2 } },
                certificates = new List<string> {"11110000"},
                promo_codes = new List<string> { "LADNAHATA"},
                delivery_branch_id = 1,
                delivery_adress = new Order.DeliveryAdress { city_name="Kyiv",street="Shevchenka",building="24",apartment="17"},
                total_price = 3300 
            };

            var msg = new StringContent(JsonConvert.SerializeObject(order),System.Text.Encoding.UTF8,"application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/Orders/new", msg);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async void CreateNewOrderGuest_Success()
        {
            // Arrange
             var order = new Order
            {
                user = new Order.OrderUser { first_name=Faker.Name.First(),last_name = Faker.Name.Last(), email =Faker.Internet.Email(),phone = Faker.Phone.Number()},
                receiver = new Order.Receiver { first_name = Faker.Name.First(), last_name = Faker.Name.Last(), middle_name = Faker.Name.Last(), profile_name = "", email = Faker.Internet.Email(), phone = Faker.Phone.Number() },
                payData = new Order.PayData { payment_type_id = 2 },

                goods = new List<Order.Item> { new Order.Item { article_id = 1, price = 1500, quantity = 1 }, new Order.Item { article_id = 2, price = 1000, quantity = 2 } },
                certificates = new List<string> { "11110000" },
                promo_codes = new List<string> { "LADNAHATA" },
                delivery_branch_id = 1,
                delivery_adress = new Order.DeliveryAdress { city_name = "Kyiv", street = "Shevchenka", building = "24", apartment = "17" },
                total_price = 3300
            };

            var msg = new StringContent(JsonConvert.SerializeObject(order), System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("/api/Orders/new", msg);
            var content = await response.Content.ReadAsStringAsync();
            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async void GetOrder_Success()
        {
            // Arrange

            // Act
            var result = await _httpClient.GetAsync($"/api/Orders/get?order_id=1");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }

        [Fact]
        public async void EditOrder_Success()
        {
            // Arrange
            var response = await _httpClient.GetAsync($"/api/Orders/get?order_id=1");
            var data = await response.Content.ReadAsStringAsync();
            var order = JsonConvert.DeserializeObject<Order>(data);

            if(order != null)
            {
                order.status = OrderStatus.Shipped.ToString();
                order.user = null;
            }

            var json = JsonConvert.SerializeObject(order);
            var msg = new StringContent(json,System.Text.Encoding.UTF8,"application/json");

            // Act
            var result = await _httpClient.PutAsync("api/Orders/update", msg);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessStatusCode);
        }
    }
}
