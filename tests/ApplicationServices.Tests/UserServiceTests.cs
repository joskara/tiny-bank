using Moq;
using System.ComponentModel.DataAnnotations;
using ApplicationServices.Interfaces;
using ApplicationServices.Implementations;
using Data.POCOs;
using Data.Repositories;
using Domain;

namespace ApplicationServices.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IBaseRepository<UserPoco>> _usersRepositoryMock;
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            _usersRepositoryMock = new Mock<IBaseRepository<UserPoco>>();
            _userService = new UserService(_usersRepositoryMock.Object);
        }

        [Fact]
        public void GetUsers_ShouldReturnAllUsers()
        {
            // Arrange
            var usersPoco = new List<UserPoco>
            {
                new() { Id = Guid.NewGuid(), FirstName = "User1", LastName = "LastUser1", FiscalNumber = "123" },
                new() { Id = Guid.NewGuid(), FirstName = "User2", LastName = "LastUser2", FiscalNumber = "456" }
            };

            _usersRepositoryMock.Setup(repo => repo.Get()).Returns(usersPoco);

            // Act
            var result = _userService.GetUsers();

            // Assert
            Assert.Equal(2, result.ToList().Count);
            _usersRepositoryMock.Verify(repo => repo.Get(), Times.Once);
        }

        [Fact]
        public void GetUser_WithValidId_ShouldReturnUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userPoco = new UserPoco { Id = userId, FirstName = "User1", LastName = "LastUser1", FiscalNumber = "123" };

            _usersRepositoryMock.Setup(repo => repo.GetById(userId)).Returns(userPoco);

            // Act
            var result = _userService.GetUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            _usersRepositoryMock.Verify(repo => repo.GetById(userId), Times.Once);
        }

        [Fact]
        public void GetUser_WithInvalidId_ShouldThrowArgumentException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _usersRepositoryMock.Setup(repo => repo.GetById(userId)).Returns((UserPoco)null!);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _userService.GetUser(userId));
            Assert.Equal($"User with id {userId} not found", exception.Message);
        }

        [Fact]
        public void AddUser_WithUniqueFiscalNumber_ShouldAddUser()
        {
            // Arrange
            var user = new User(Guid.NewGuid(),"User1", "LastUser1", "123", true, DateTime.UtcNow, DateTime.UtcNow );

            _usersRepositoryMock.Setup(repo => repo.GetByFilter("FiscalNumber", user.FiscalNumber)).Returns((UserPoco)null!);

            _usersRepositoryMock.Setup(repo => repo.Insert(It.IsAny<UserPoco>())).Returns(user.Id);

            // Act
            var userId = _userService.AddUser(user);

            // Assert
            Assert.Equal(user.Id, userId);
            _usersRepositoryMock.Verify(repo => repo.GetByFilter("FiscalNumber", user.FiscalNumber), Times.Once);
            _usersRepositoryMock.Verify(repo => repo.Insert(It.IsAny<UserPoco>()), Times.Once);
        }

        [Fact]
        public void AddUser_WithExistingFiscalNumber_ShouldThrowValidationException()
        {
            // Arrange
            var user = new User(Guid.NewGuid(),"User1", "LastUser1", "123", true, DateTime.UtcNow, DateTime.UtcNow );
            var existingUserPoco = new UserPoco { Id = user.Id, FirstName = "User1", LastName = "LastUser1", FiscalNumber = "123" };

            _usersRepositoryMock.Setup(repo => repo.GetByFilter("FiscalNumber", user.FiscalNumber)).Returns(existingUserPoco);

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => _userService.AddUser(user));
            Assert.Equal("User with the same FiscalNumber already exists", exception.Message);
        }

        [Fact]
        public void UpdateUserActive_WithValidUserId_ShouldUpdateUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userPoco = new UserPoco { Id = userId, FirstName = "User1", LastName = "LastUser1", FiscalNumber = "123", IsActive = false };

            _usersRepositoryMock.Setup(repo => repo.GetById(userId)).Returns(userPoco);

            // Act
            _userService.UpdateUserActive(userId, true);

            // Assert
            _usersRepositoryMock.Verify(repo => repo.GetById(userId), Times.Once);
            _usersRepositoryMock.Verify(repo => repo.Update(It.IsAny<UserPoco>()), Times.Once);
        }

        [Fact]
        public void UpdateUserActive_WithInvalidUserId_ShouldThrowArgumentException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _usersRepositoryMock.Setup(repo => repo.GetById(userId)).Returns((UserPoco)null!);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _userService.UpdateUserActive(userId, true));
            Assert.Equal("User to be updated does not exist", exception.Message);
        }
    }
}