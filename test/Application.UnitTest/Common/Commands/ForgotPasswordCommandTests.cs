using Application.Accounts.Commands;
using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.UnitTest.Common.Commands
{
	public class ForgotPasswordCommandTests
	{
        private readonly ForgotPasswordCommandHandler _sut;
        private readonly Mock<IIdentityService> _identityService = new Mock<IIdentityService>();
        private readonly Mock<ICustomMailSender> _customMailSender = new Mock<ICustomMailSender>();

        public ForgotPasswordCommandTests()
		{
			_sut = new ForgotPasswordCommandHandler(_identityService.Object, _customMailSender.Object);
		}

		[Fact]
		public async void Handler_ShouldReturnSuccessResult_WhenPasswordTokenIsGenerated()
		{
            //Arrange
            var forgotPasswordCommand = new ForgotPasswordCommand
            {
                Email = "test@mail.com"
            };

            var succededResult = Result.Success();

            var token = "token";

            _identityService.Setup(x => x.GeneratePasswordResetTokenAsync(forgotPasswordCommand.Email, new CancellationToken()))
                .ReturnsAsync(token);

            //Act
            var result = await _sut.Handle(forgotPasswordCommand, new CancellationToken());

            //Assert
            Assert.Equal(Result.Success().Succeeded, result.Succeeded);
            Assert.Equal(Result.Success().Errors, result.Errors);
        }
	}
}