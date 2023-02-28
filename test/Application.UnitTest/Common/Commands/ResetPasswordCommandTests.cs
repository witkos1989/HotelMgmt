using Application.Accounts.Commands;
using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.UnitTest.Common.Commands
{
	public class ResetPasswordCommandTests
	{
        private readonly ResetPasswordCommandHandler _sut;
        private readonly Mock<IIdentityService> _identityService = new Mock<IIdentityService>();

        public ResetPasswordCommandTests()
		{
            _sut = new ResetPasswordCommandHandler(_identityService.Object);
        }

        [Fact]
        public async void Handler_ShouldReturnSuccessResult_WhenPasswordIsReset()
        {
            //Arrange
            var command = new ResetPasswordCommand()
            {
                Password = "f6!Z?XRtY{",
                ConfirmPassword = "f6!Z?XRtY{",
                Email = "test@mail.com",
                Token = "token"
            };

            var successResult = Result.Success();

            _identityService.Setup(x => x.ResetPasswordAsync(command.Email, command.Token, command.Password, new CancellationToken()))
                .ReturnsAsync(successResult);

            //Act
            var result = await _sut.Handle(command, new CancellationToken());

            //Assert
            Assert.Equal(successResult.Succeeded, result.Succeeded);
        }

        [Fact]
        public async void Handler_ShouldReturnFailedResult_WhenPasswordIsntReset()
        {
            //Arrange
            var command = new ResetPasswordCommand()
            {
                Password = "f6!Z?XRtY{",
                ConfirmPassword = "f6!Z?XRtY{",
                Email = "test@mail.com",
                Token = "token"
            };

            var failedResult = Result.Failure("");

            _identityService.Setup(x => x.ResetPasswordAsync(command.Email, command.Token, command.Password, new CancellationToken()))
                .ReturnsAsync(failedResult);

            //Act
            var result = await _sut.Handle(command, new CancellationToken());

            //Assert
            Assert.Equal(failedResult.Succeeded, result.Succeeded);
            Assert.Equal(failedResult.Errors, result.Errors);
        }
    }
}