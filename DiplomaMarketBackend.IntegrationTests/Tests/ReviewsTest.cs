using DiplomaMarketBackend.IntegrationTests.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiplomaMarketBackend.IntegrationTests.Tests;

public class ReviewsTest:BasicTest
{
    [Fact]
    public async void AddReview_Success()
    {
        // Arrange
        var review = new
        {
            name = Faker.Name.First(),
            comment = Faker.Lorem.Paragraph(),
            rate = 5,
            article_id = 1,
            pros = Faker.Lorem.GetFirstWord(),
            cons = Faker.Lorem.Sentence(),
            email = Faker.Internet.Email(),
            video_url = Faker.Internet.Url(),
            get_email_on_answers = true,
        };
        
        // Act
        var result = await _httpClient.PostAsync("api/Reviews/post_review", review.ToFormData());
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async void GetReview_Success()
    {
        // Arrange
        // Act
        var result = await _httpClient.GetAsync("api/Reviews/get-review?review_id=1");
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void UpdateReview_Success()
    {
        // Arrange
        var review = new
        {
            id= 1,
            name = Faker.Name.First(),
            comment = Faker.Lorem.Paragraph(),
            rate = 5,
            article_id = 1,
            pros = Faker.Lorem.GetFirstWord(),
            cons = Faker.Lorem.Sentence(),
            email = Faker.Internet.Email(),
            video_url = Faker.Internet.Url(),
            get_email_on_answers = true,
        };
        // Act
        var result = await _httpClient.PutAsync("api/Reviews/update_review",review.ToFormData());
        var content = await result.Content.ReadAsStringAsync();
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async void DeleteReview_Success()
    {
        // Arrange
        var response = await _httpClient.GetAsync("api/Reviews/get?lang=uk&article_id=1&type=review&limit=10&page=1");
        var content = await response.Content.ReadAsStringAsync();
        JObject? data = JsonConvert.DeserializeObject<JObject>(content);
        var reviews = data["data"]["reviews"];
        var id = reviews.Max(r => r["id"]).ToString();
        
        // Act 
        var result = await _httpClient.DeleteAsync($"api/Reviews/delete_review?review_id={id}");
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);

    }
    
    [Fact]
    public async void AddAnswer_Success()
    {
        // Arrange
        var answer = new
        {
            name = Faker.Name.First(),
            comment = Faker.Lorem.Paragraph(),
            review_id =1,
            article_id = 1,
            email = Faker.Internet.Email()
        };
        
        // Act
        var result = await _httpClient.PostAsync("api/Reviews/post_answer", answer.ToFormData());
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async void DeleteAnswer_Success()
    {
        // Arrange
        var response = await _httpClient.GetAsync("api/Reviews/get?lang=uk&article_id=1&type=review&limit=10&page=1");
        var content = await response.Content.ReadAsStringAsync();
        JObject? data = JsonConvert.DeserializeObject<JObject>(content);
        var reviews = data["data"]["reviews"];
        var answers = reviews?.First()["answers"];
        var id = answers.Max(a => a["id"]);
        
        // Act
        var result = await _httpClient.DeleteAsync($"api/Reviews/delete_answer?answer_id={id}");
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
    
    [Fact]
    public async void GetArticleReviews_Success()
    {
        // Arrange
        
        // Act
        var result = await _httpClient.GetAsync("api/Reviews/get?lang=uk&article_id=1&type=review&limit=10&page=1");
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }

}