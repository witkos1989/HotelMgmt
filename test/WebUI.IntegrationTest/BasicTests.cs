using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace WebUI.IntegrationTest
{
    public class BasicTests
    : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public BasicTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Home/Privacy")]
        [InlineData("/Account/Login")]
        [InlineData("/Account/Register")]
        [InlineData("/Account/ForgotPassword")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType!.ToString());
        }
    }
}