using Application.Accounts.Commands;
using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.UnitTest.Common.Commands
{
	public class LoginCommandTests
	{
		private readonly LoginCommandHandler _sut;
        private readonly Mock<ISigningManager> _signingManager = new Mock<ISigningManager>();

		public LoginCommandTests()
		{
			_sut = new LoginCommandHandler(_signingManager.Object);
        }

		[Fact]
		public async void Handle_ShouldReturnSuccessResultAndBool_WhenUserIsSuccesfullyLoggedIn()
		{
			//Arrange
			var loginCommand = new LoginCommand()
			{
				Name = "test",
				Password = "f6!Z?XRtY{"
            };
            var succededResult = Result.Success();

            var signInResult = (succededResult, false);

            _signingManager.Setup(x => x.SignInAsync(loginCommand.Name,loginCommand.Password, new CancellationToken()))
				.ReturnsAsync(signInResult);

			//Act
			var result = await _sut.Handle(loginCommand, new CancellationToken());

			//Assert
            Assert.Equal(succededResult.Succeeded, result.Item1.Succeeded);
            Assert.Equal(succededResult.Errors, result.Item1.Errors);
            Assert.False(result.Item2);
		}

		[Fact]
		public async void Handle_ShouldReturnsFailure_WhenUserCantLogIn()
		{
            //Arrange
            var loginCommand = new LoginCommand()
			{
				Name = string.Empty,
				Password = string.Empty
            };
            var failedResult = Result.Failure("Nieprawidłowy login lub hasło");

            var signInResult = (failedResult, false);

            _signingManager.Setup(x => x.SignInAsync(string.Empty, string.Empty, new CancellationToken()))
                .ReturnsAsync(signInResult);

            //Act
            var result = await _sut.Handle(loginCommand, new CancellationToken());

            //Assert
            Assert.Equal(failedResult.Succeeded, result.Item1.Succeeded);
            Assert.Equal(failedResult.Errors, result.Item1.Errors);
            Assert.False(result.Item2);
        }
	}
}