using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using WebUI.IntegrationTest.Helpers;
using AngleSharp.Html.Dom;
using System.Net;

namespace WebUI.IntegrationTest
{
	public class AccountControllerTests
		: IClassFixture<IntegrationTest<Program>>
	{
        private readonly IntegrationTest<Program> _factory;

        public AccountControllerTests(IntegrationTest<Program> factory)
		{
			_factory = factory;
		}

        private static async Task<HttpResponseMessage> RegisterAsync(HttpClient client)
        {
            var registerPage = await client.GetAsync("/account/register");
            var registerContent = await HtmlHelper.GetDocumentAsync(registerPage);
            var registerDict = new Dictionary<string, string>
            {
                ["firstname"] = "Użytkownik",
                ["lastname"] = "Testowy",
                ["username"] = "test",
                ["email"] = "test@mail.com",
                ["phonenumber"] = "123456789",
                ["password"] = "f6!Z?XRtY{",
                ["confirmpassword"] = "f6!Z?XRtY{"
            };

            var response = await client.SendAsync(
                (IHtmlFormElement)registerContent.QuerySelector("form[id='registerForm']")!,
                (IHtmlButtonElement)registerContent.QuerySelector("button[id='registerBtn']")!,
                registerDict);

            return response;
        }

        [Fact]
        public async Task Post_SuccesLogin_RedirectsToMainPage()
        {
            //Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = true
            });

            await RegisterAsync(client);

            var loginPage = await client.GetAsync("/account/login");
            var content = await HtmlHelper.GetDocumentAsync(loginPage);
            var loginDict = new Dictionary<string, string>
            {
                ["name"] = "test",
                ["password"] = "f6!Z?XRtY{"
            };

            //Act
            var response = await client.SendAsync(
                (IHtmlFormElement)content.QuerySelector("form[id='loginForm']")!,
                (IHtmlButtonElement)content.QuerySelector("button[id='loginBtn']")!,
                loginDict);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var mainPage = await HtmlHelper.GetDocumentAsync(response);

            Assert.Equal("Strona demo HotelMgmt", mainPage.Title);
        }

        [Fact]
        public async Task Post_FailedLogin_RedirectsToLoginPageWithValidationMessage()
        {
            //Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = true
            });

            await RegisterAsync(client);

            var loginPage = await client.GetAsync("/account/login");
            var content = await HtmlHelper.GetDocumentAsync(loginPage);
            var loginDict = new Dictionary<string, string>
            {
                ["name"] = "test",
                ["password"] = "abc123"
            };

            //Act
            var response = await client.SendAsync(
                (IHtmlFormElement)content.QuerySelector("form[id='loginForm']")!,
                (IHtmlButtonElement)content.QuerySelector("button[id='loginBtn']")!,
                loginDict);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var returnPage = await HtmlHelper.GetDocumentAsync(response);

            var validationMessage = (IHtmlListItemElement)returnPage.QuerySelector("li")!;

            Assert.Equal("Nieprawidłowy login lub hasło", validationMessage.TextContent);
            Assert.Equal("Ekran logowania", returnPage.Title);
        }

        [Fact]
        public async Task Post_SuccesRegister_RedirectsToSuccessRegistrationPage()
        {
            //Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = true
            });

            //Act
            var response = await RegisterAsync(client);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(HttpMethod.Post, response.RequestMessage!.Method);

            var successRegistrationView = await HtmlHelper.GetDocumentAsync(response);

            var pageHeader = successRegistrationView.QuerySelector("h1");

            Assert.Equal("Rejestracja zakończona", pageHeader!.TextContent);
        }
    }
}