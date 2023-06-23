using DiplomaMarketBackend.IntegrationTests.Tests;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace DiplomaMarketBackend.IntegrationTests.UITests;

public class ArticlesTest:BasicTest
{
    [Fact]
    public async void ArticleDisplayTest_Success()
    {
        var address = _httpClient.BaseAddress;
        // Arrange
        var url = "http://localhost:3000/rozetka-clone-front/article/4";
        var options = new ChromeOptions();
        options.AddArguments("headless", "--whitelisted-ips");
        IWebDriver driver = new ChromeDriver(options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
        
        // Act
        driver.Navigate().GoToUrl(url);
        
        var left = driver.FindElement(By.ClassName("article-left"));
        var name = driver.FindElement(By.TagName("h3")).Text;

        // Assert
        
        Assert.NotEmpty(name);
    }
    
}