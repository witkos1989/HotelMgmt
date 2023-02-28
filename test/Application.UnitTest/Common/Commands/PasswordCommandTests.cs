using Application.Accounts.Commands;
using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.UnitTest.Common.Commands
{
	public class PasswordCommandTests
	{
        private readonly PasswordCommandHandler _sut;
        private readonly Mock<IIdentityService> _identityService = new Mock<IIdentityService>();

        public PasswordCommandTests()
		{
			_sut = new PasswordCommandHandler(_identityService.Object);
		}

		[Fact]
		public async void Handler_ShouldReturnSuccessResult_WhenPasswordIsChanged()
		{
			//Arrange
			var command = new PasswordCommand()
			{
				Password = "f6!Z?XRtY{",
				ConfirmPassword = "f6!Z?XRtY{",
				OldPassword = "f6!Z?XRt1{",
				LoggedUserId = Guid.NewGuid().ToString()
            };

			var successResult = Result.Success();

			_identityService.Setup(x => x.ChangePasswordAsync(command.LoggedUserId, command.OldPassword, command.Password, new CancellationToken()))
				.ReturnsAsync(successResult);

			//Act
			var result = await _sut.Handle(command, new CancellationToken());

			//Assert
			Assert.Equal(successResult.Succeeded, result.Succeeded);
		}

        [Fact]
        public async void Handler_ShouldReturnFailedResult_WhenLoggedUserIdIsntPassed()
        {
            //Arrange
            var command = new PasswordCommand()
            {
                Password = "f6!Z?XRtY{",
                ConfirmPassword = "f6!Z?XRtY{",
                OldPassword = "f6!Z?XRt1{",
                LoggedUserId = string.Empty
            };

            var failedResult = Result.Failure("Nie można zaktualizować hasła");

            _identityService.Setup(x => x.ChangePasswordAsync(command.LoggedUserId, command.OldPassword, command.Password, new CancellationToken()))
                .ReturnsAsync(failedResult);

            //Act
            var result = await _sut.Handle(command, new CancellationToken());

            //Assert
            Assert.Equal(failedResult.Succeeded, result.Succeeded);
            Assert.Equal(failedResult.Errors, result.Errors);
        }
    }
}