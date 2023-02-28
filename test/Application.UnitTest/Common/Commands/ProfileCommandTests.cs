using Application.Accounts.Commands;
using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.UnitTest.Common.Commands
{
	public class ProfileCommandTests
	{
		private readonly ProfileCommandHandler _sut;
		private readonly Mock<IIdentityService> _identityService = new Mock<IIdentityService>();
		private readonly Mock<ISigningManager> _signingManager = new Mock<ISigningManager>();

        public ProfileCommandTests()
		{
			_sut = new ProfileCommandHandler(_identityService.Object, _signingManager.Object);
		}

		[Fact]
		public async void Handler_ShouldReturnFailedResult_WhenUserIsDuplicated()
		{
			//Arrange
			var command = new ProfileCommand()
			{
				UserName = "test",
				FirstName = "Uzytkownik",
				LastName = "Testowy",
				Email = "test@mail.com",
				PhoneNumber = "123456789",
				LoggedUserId = Guid.NewGuid().ToString()
			};

			var accountDTO = new AccountDetailsDTO()
			{
                UserName = "test",
                FirstName = "Uzytkownik",
                LastName = "Testowy",
                Email = "test@mail.com",
                PhoneNumber = "123456789",
				Id = command.LoggedUserId
            };

			var failedResult = Result.Failure("Użytkownik " + command.UserName + " już istnieje");

			var createUserResult = (Result.Success(), "token");

			_identityService.Setup(x => x.CreateUserAsync(command.UserName, command.FirstName, command.LastName, command.Email, command.Email, "f6!Z?XRtY{", new CancellationToken()))
				.ReturnsAsync(createUserResult);

			_identityService.Setup(x => x.GetUserByIdAsync(command.LoggedUserId, new CancellationToken()))
				.ReturnsAsync(accountDTO);

            _identityService.Setup(x => x.CheckForExistingUserNameAsync(command.LoggedUserId, command.UserName, new CancellationToken()))
                .ReturnsAsync(true);

			//Act
			var result = await _sut.Handle(command, new CancellationToken());

			//Assert
			Assert.Equal(failedResult.Succeeded, result.Succeeded);
			Assert.Equal(failedResult.Errors, result.Errors);
		}

		[Fact]
        public async void Handler_ShouldReturnFailedResult_WhenUserMailIsDuplicated()
        {
            //Arrange
            var command = new ProfileCommand()
            {
                UserName = "test",
                FirstName = "Uzytkownik",
                LastName = "Testowy",
                Email = "test@mail.com",
                PhoneNumber = "123456789",
                LoggedUserId = Guid.NewGuid().ToString()
            };

            var accountDTO = new AccountDetailsDTO()
            {
                UserName = "test",
                FirstName = "Uzytkownik",
                LastName = "Testowy",
                Email = "test@mail.com",
                PhoneNumber = "123456789",
                Id = command.LoggedUserId
            };

            var failedResult = Result.Failure("W systemie jest już zarejestrowany użytkownik z takim adresem email");

            var createUserResult = (Result.Success(), "token");

            _identityService.Setup(x => x.CreateUserAsync(command.UserName, command.FirstName, command.LastName, command.Email, command.Email, "f6!Z?XRtY{", new CancellationToken()))
                .ReturnsAsync(createUserResult);

            _identityService.Setup(x => x.GetUserByIdAsync(command.LoggedUserId, new CancellationToken()))
                .ReturnsAsync(accountDTO);

            _identityService.Setup(x => x.CheckForExistingUserMailAsync(command.LoggedUserId, command.Email, new CancellationToken()))
                .ReturnsAsync(true);

            //Act
            var result = await _sut.Handle(command, new CancellationToken());

            //Assert
            Assert.Equal(failedResult.Succeeded, result.Succeeded);
            Assert.Equal(failedResult.Errors, result.Errors);
        }

        [Fact]
        public async void Handler_ShouldReturnFailedResult_WhenUserIsNull()
        {
            //Arrange
            var command = new ProfileCommand()
            {
                UserName = "test",
                FirstName = "Uzytkownik",
                LastName = "Testowy",
                Email = "test@mail.com",
                PhoneNumber = "123456789",
                LoggedUserId = string.Empty
            };

            var failedResult = Result.Failure("Nie można odczytać danych użytkownika");

            _identityService.Setup(x => x.GetUserByIdAsync(command.LoggedUserId, new CancellationToken()))
                .ReturnsAsync(() => null);

            //Act
            var result = await _sut.Handle(command, new CancellationToken());

            //Assert
            Assert.Equal(failedResult.Succeeded, result.Succeeded);
            Assert.Equal(failedResult.Errors, result.Errors);
        }

        [Fact]
        public async void Handler_ShouldReturnFailedResult_WhenUpdateWasntSuccessful()
        {
            //Arrange
            var command = new ProfileCommand()
            {
                UserName = "test",
                FirstName = "Uzytkownik",
                LastName = "Testowy",
                Email = "test@mail.com",
                PhoneNumber = "123456789",
                LoggedUserId = Guid.NewGuid().ToString()
            };

            var accountDTO = new AccountDetailsDTO()
            {
                UserName = "test",
                FirstName = "Uzytkownik",
                LastName = "Testowy",
                Email = "test@mail.com",
                PhoneNumber = "123456789",
                Id = command.LoggedUserId
            };

            var failedResult = (Result.Failure("Podczas zapisu danych wystąpił błąd"), false);

            _identityService.Setup(x => x.GetUserByIdAsync(command.LoggedUserId, new CancellationToken()))
                .ReturnsAsync(accountDTO);

            _identityService.Setup(x => x.UpdateUserAsync(accountDTO, It.IsAny<AccountDetailsDTO?>()!, new CancellationToken()))
                .ReturnsAsync(failedResult);

            _signingManager.Setup(x => x.SignOutAsync(new CancellationToken()));

            //Act
            var result = await _sut.Handle(command, new CancellationToken());

            //Assert
            Assert.Equal(failedResult.Item1.Succeeded, result.Succeeded);
            Assert.Equal(failedResult.Item1.Errors, result.Errors);
        }

        [Fact]
        public async void Handler_ShouldSignOut_WhenUserNameIsChanged()
        {
            //Arrange
            var command = new ProfileCommand()
            {
                UserName = "testowy",
                FirstName = "Uzytkownik",
                LastName = "Testowy",
                Email = "test@mail.com",
                PhoneNumber = "123456789",
                LoggedUserId = Guid.NewGuid().ToString()
            };

            var accountDTO = new AccountDetailsDTO()
            {
                UserName = "test",
                FirstName = "Uzytkownik",
                LastName = "Testowy",
                Email = "test@mail.com",
                PhoneNumber = "123456789",
                Id = command.LoggedUserId
            };

            var shouldLogoutResult = (Result.Success(), true);

            _identityService.Setup(x => x.GetUserByIdAsync(command.LoggedUserId, new CancellationToken()))
                .ReturnsAsync(accountDTO);

            _identityService.Setup(x => x.UpdateUserAsync(accountDTO, It.IsAny<AccountDetailsDTO?>()!, new CancellationToken()))
                .ReturnsAsync(shouldLogoutResult);

            _signingManager.Setup(x => x.SignOutAsync(new CancellationToken()));

            //Act
            var result = await _sut.Handle(command, new CancellationToken());

            //Assert
            Assert.Equal(shouldLogoutResult.Item1.Succeeded, result.Succeeded);
        }

        [Fact]
        public async void Handler_ShouldReturnSuccessResult_WhenUserDetailsIsChanged()
        {
            //Arrange
            var command = new ProfileCommand()
            {
                UserName = "test",
                FirstName = "Uzytkownik",
                LastName = "Test",
                Email = "test@mail.com",
                PhoneNumber = "987654321",
                LoggedUserId = Guid.NewGuid().ToString()
            };

            var accountDTO = new AccountDetailsDTO()
            {
                UserName = "test",
                FirstName = "Uzytkownik",
                LastName = "Testowy",
                Email = "test@mail.com",
                PhoneNumber = "123456789",
                Id = command.LoggedUserId
            };

            var shouldLogoutResult = (Result.Success(), false);

            _identityService.Setup(x => x.GetUserByIdAsync(command.LoggedUserId, new CancellationToken()))
                .ReturnsAsync(accountDTO);

            _identityService.Setup(x => x.UpdateUserAsync(accountDTO, It.IsAny<AccountDetailsDTO?>()!, new CancellationToken()))
                .ReturnsAsync(shouldLogoutResult);

            //Act
            var result = await _sut.Handle(command, new CancellationToken());

            //Assert
            Assert.Equal(shouldLogoutResult.Item1.Succeeded, result.Succeeded);
        }
    }
}