using Application.Accounts.Commands;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.DTOs;

namespace Application.UnitTest.Common.Commands
{
	public class GetDetailsCommandTests
	{
		private readonly GetDetailsCommandHandler _sut;
		private readonly Mock<IIdentityService> _identityService = new Mock<IIdentityService>();

		public GetDetailsCommandTests()
		{
			_sut = new GetDetailsCommandHandler(_identityService.Object);
		}

		[Fact]
		public async void Handle_ShouldReturnAccountDetailsDTO_WhenUserIdIsPassed()
		{
			//Arrange
			var userCommand = new GetDetailsCommand()
			{
				UserId = Guid.NewGuid().ToString()
			};

			var accountDTO = new AccountDetailsDTO()
			{
				UserName = "test",
				FirstName = "Uzytkownik",
				LastName = "Testowy",
				Email = "test@mail.com",
				PhoneNumber = "123456789"
			};
			
			_identityService.Setup(x => x.GetUserByIdAsync(userCommand.UserId, new CancellationToken()))
				.ReturnsAsync(accountDTO);
			//Act
			var result = await _sut.Handle(userCommand, new CancellationToken());

			//Assert
			Assert.Equal(accountDTO.UserName, result!.UserName);
			Assert.Equal(accountDTO.LastName, result!.LastName);
            Assert.Equal(accountDTO.FirstName, result!.FirstName);
            Assert.Equal(accountDTO.Email, result!.Email);
            Assert.Equal(accountDTO.PhoneNumber, result!.PhoneNumber);
        }

		[Fact]
		public async void Handle_ShouldReturnNull_WhenUserIdIsNull()
        {
            //Arrange
            var userCommand = new GetDetailsCommand()
            {
                UserId = string.Empty
            };

            _identityService.Setup(x => x.GetUserByIdAsync(userCommand.UserId, new CancellationToken()))
                .ReturnsAsync(() => null);
            //Act
            var result = await _sut.Handle(userCommand, new CancellationToken());

			//Assert
			Assert.Null(result);
        }
    }
}