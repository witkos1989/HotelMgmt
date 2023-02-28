using Application.Accounts.Commands;
using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.UnitTest.Common.Commands
{
	public class ConfirmMailCommandTests
	{
		private readonly ConfirmMailCommandHandler _sut;
		private readonly Mock<IIdentityService> _identityService = new Mock<IIdentityService>();

		public ConfirmMailCommandTests()
		{
			_sut = new ConfirmMailCommandHandler(_identityService.Object);
		}

		[Fact]
		public async void Handle_ShouldReturnSucceededResult_WhenEmailWasConfirmed()
		{
			//Arrange
			var command = new ConfirmMailCommand("test@mail.com", "token");

			var successResult = Result.Success();

			_identityService.Setup(x => x.ConfirmEmailAsync(command.Email, command.Token, new CancellationToken()))
				.ReturnsAsync(successResult);

			//Act
			var result = await _sut.Handle(command, new CancellationToken());

			//Assert
			Assert.Equal(Result.Success().Succeeded, result.Succeeded);
            Assert.Equal(Result.Success().Errors, result.Errors);
        }

        [Fact]
        public async void Handle_ShouldReturnFailedResult_WhenMailIsntConfirmed()
        {
            //Arrange
            var command = new ConfirmMailCommand("test@mail.com", "");

            var failedResult = Result.Failure("");

            _identityService.Setup(x => x.ConfirmEmailAsync(command.Email, command.Token, new CancellationToken()))
                .ReturnsAsync(failedResult);

            //Act
            var result = await _sut.Handle(command, new CancellationToken());

            //Assert
            Assert.Equal(failedResult.Succeeded, result.Succeeded);
            Assert.Equal(failedResult.Errors, result.Errors);
        }
    }
}