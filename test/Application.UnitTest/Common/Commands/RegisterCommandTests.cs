using Application.Accounts.Commands;
using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.UnitTest.Common.Commands
{
	public class RegisterCommandTests
	{
		private readonly RegisterCommandHandler _sut;
		private readonly Mock<IIdentityService> _identityService = new Mock<IIdentityService>();
		private readonly Mock<ICustomMailSender> _customMailSender = new Mock<ICustomMailSender>();

		public RegisterCommandTests()
		{
			_sut = new RegisterCommandHandler(_identityService.Object, _customMailSender.Object);
		}

		[Fact]
		public async void Handle_ShouldReturnSuccesResult_WhenUserIsRegistered()
		{
			//Arrange
			var command = new RegisterCommand()
			{
				UserName = "test",
				FirstName = "Uzytkownik",
				LastName = "Testowy",
				Email = "test@mail.com",
				PhoneNumber = "123456789",
				Password = "f6!Z?XRtY{",
				ConfirmPassword = "f6!Z?XRtY{"
            };

			var createResult = (Result.Success(), Guid.NewGuid().ToString());

            _identityService.Setup(x => x.CreateUserAsync(command.UserName, command.FirstName, command.LastName, command.Email, command.PhoneNumber, command.Password, new CancellationToken()))
				.ReturnsAsync(createResult);

			//Act
			var result = await _sut.Handle(command, new CancellationToken());

			//Assert
			Assert.Equal(Result.Success().Succeeded, result.Succeeded);
			Assert.Equal(Result.Success().Errors, result.Errors);
		}
	}
}