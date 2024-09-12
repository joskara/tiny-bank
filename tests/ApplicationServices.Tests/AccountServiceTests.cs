using System.ComponentModel.DataAnnotations;
using ApplicationServices.Implementations;
using ApplicationServices.Interfaces;
using Data.POCOs;
using Data.Repositories;
using Domain;
using Moq;

namespace ApplicationServices.Tests
{
    public class AccountServiceTests
    {
        private readonly Mock<IBaseRepository<AccountPoco>> _accountRepositoryMock;
        private readonly Mock<IBaseRepository<UserPoco>> _userRepositoryMock;
        private readonly IAccountService _accountService;

        public AccountServiceTests()
        {
            _accountRepositoryMock = new Mock<IBaseRepository<AccountPoco>>();
            _userRepositoryMock = new Mock<IBaseRepository<UserPoco>>();
            _accountService = new AccountService(_accountRepositoryMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public void GetAccounts_ShouldReturnAllAccounts()
        {
            // Arrange
            var accountsPoco = new List<AccountPoco>
            {
                new() { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), IsActive = true },
                new() { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), IsActive = false }
            };

            _accountRepositoryMock.Setup(repo => repo.Get()).Returns(accountsPoco);

            // Act
            var result = _accountService.GetAccounts();

            // Assert
            Assert.Equal(2, result.Count());
            _accountRepositoryMock.Verify(repo => repo.Get(), Times.Once);
        }

        [Fact]
        public void GetAccount_WithValidId_ShouldReturnAccount()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var accountPoco = new AccountPoco { Id = accountId, UserId = Guid.NewGuid(), IsActive = true };

            _accountRepositoryMock.Setup(repo => repo.GetById(accountId)).Returns(accountPoco);

            // Act
            var result = _accountService.GetAccount(accountId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(accountId, result.Id);
            _accountRepositoryMock.Verify(repo => repo.GetById(accountId), Times.Once);
        }

        [Fact]
        public void GetAccount_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var accountId = Guid.NewGuid();

            _accountRepositoryMock.Setup(repo => repo.GetById(accountId)).Returns((AccountPoco)null!);

            // Act
            var result = _accountService.GetAccount(accountId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void AddAccount_WithNullAccount_ShouldThrowValidationException()
        {
            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => _accountService.AddAccount(null!));
            Assert.Equal("Please supply a valid Account", exception.Message);
        }

        [Fact]
        public void AddAccount_WithoutValidUser_ShouldThrowValidationException()
        {
            // Arrange
            var account = new Account(Guid.NewGuid(), Guid.NewGuid(), 0, true, DateTime.UtcNow, DateTime.UtcNow);

            _userRepositoryMock.Setup(repo => repo.GetById(account.UserId)).Returns((UserPoco)null!);

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => _accountService.AddAccount(account));
            Assert.Equal("User does not exist", exception.Message);
        }

        [Fact]
        public void AddAccount_WithValidUser_ShouldAddAccount()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var account = new Account(Guid.NewGuid(), userId, 0, true, DateTime.UtcNow, DateTime.UtcNow);

            var userPoco = new UserPoco { Id = userId, IsActive = true };
            _userRepositoryMock.Setup(repo => repo.GetById(userId)).Returns(userPoco);

            _accountRepositoryMock.Setup(repo => repo.Insert(It.IsAny<AccountPoco>())).Returns(account.Id);

            // Act
            var accountId = _accountService.AddAccount(account);

            // Assert
            Assert.Equal(account.Id, accountId);
            _userRepositoryMock.Verify(repo => repo.GetById(userId), Times.Once);
            _accountRepositoryMock.Verify(repo => repo.Insert(It.IsAny<AccountPoco>()), Times.Once);
        }

        [Fact]
        public void UpdateAccountActive_WithInvalidAccountId_ShouldThrowArgumentException()
        {
            // Arrange
            var accountId = Guid.NewGuid();

            _accountRepositoryMock.Setup(repo => repo.GetById(accountId)).Returns((AccountPoco)null!);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _accountService.UpdateAccountActive(accountId, true));
            Assert.Equal("Account to be updated does not exist", exception.Message);
        }

        [Fact]
        public void UpdateAccountActive_WithValidAccountId_ShouldUpdateAccount()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var accountPoco = new AccountPoco { Id = accountId, UserId = Guid.NewGuid(), IsActive = false };

            _accountRepositoryMock.Setup(repo => repo.GetById(accountId)).Returns(accountPoco);

            // Act
            _accountService.UpdateAccountActive(accountId, true);

            // Assert
            _accountRepositoryMock.Verify(repo => repo.GetById(accountId), Times.Once);
            _accountRepositoryMock.Verify(repo => repo.Update(It.IsAny<AccountPoco>()), Times.Once);
        }
    }
}