using ApplicationServices.Implementations;
using ApplicationServices.Interfaces;
using Data.POCOs;
using Data.Repositories;
using Domain;
using Infrastructure.Enums;
using Moq;

namespace ApplicationServices.Tests
{
    public class TransactionServiceTests
    {
        private readonly Mock<IBaseRepository<TransactionPoco>> _transactionRepositoryMock;
        private readonly Mock<IBaseRepository<AccountPoco>> _accountRepositoryMock;
        private readonly ITransactionService _transactionService;

        public TransactionServiceTests()
        {
            _transactionRepositoryMock = new Mock<IBaseRepository<TransactionPoco>>();
            _accountRepositoryMock = new Mock<IBaseRepository<AccountPoco>>();
            _transactionService = new TransactionService(_transactionRepositoryMock.Object, _accountRepositoryMock.Object);
        }

        [Fact]
        public void GetTransactions_ShouldReturnAllTransactions()
        {
            // Arrange
            var transactionsPoco = new List<TransactionPoco>
            {
                new() { Id = Guid.NewGuid(), Amount = 100, Type = TransactionType.Deposit, Status = TransactionStatus.Done },
                new() { Id = Guid.NewGuid(), Amount = 200, Type = TransactionType.Withdrawal, Status = TransactionStatus.Done }
            };

            _transactionRepositoryMock.Setup(repo => repo.Get()).Returns(transactionsPoco);

            // Act
            var result = _transactionService.GetTransactions();

            // Assert
            Assert.Equal(2, result.Count());
            _transactionRepositoryMock.Verify(repo => repo.Get(), Times.Once);
        }

        [Fact]
        public void GetTransaction_WithValidId_ShouldReturnTransaction()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var transactionPoco = new TransactionPoco { Id = transactionId, Amount = 100, Type = TransactionType.Deposit, Status = TransactionStatus.Done };

            _transactionRepositoryMock.Setup(repo => repo.GetById(transactionId)).Returns(transactionPoco);

            // Act
            var result = _transactionService.GetTransaction(transactionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(transactionId, result.Id);
            _transactionRepositoryMock.Verify(repo => repo.GetById(transactionId), Times.Once);
        }

        [Fact]
        public void GetTransaction_WithInvalidId_ShouldThrowArgumentException()
        {
            // Arrange
            var transactionId = Guid.NewGuid();

            _transactionRepositoryMock.Setup(repo => repo.GetById(transactionId)).Returns((TransactionPoco)null!);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _transactionService.GetTransaction(transactionId));
            Assert.Equal($"Transaction with id {transactionId} not found", exception.Message);
        }

        [Fact]
        public void AddTransaction_WithInvalidTransaction_ShouldThrowArgumentException()
        {
            // Arrange
            var transaction = new Transaction(
                Guid.NewGuid(), 
                Guid.NewGuid(), 
                Guid.NewGuid(), 
                0, 
                TransactionType.Error, 
                TransactionStatus.InProgress,
                DateTime.UtcNow, 
                Guid.NewGuid());

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _transactionService.AddTransaction(transaction));
            Assert.Equal("Transaction should not have Amount be zero", exception.Message);
        }

        [Fact]
        public void AddTransaction_WithValidDepositTransaction_ShouldExecuteTransaction()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var destinationAccountId = Guid.NewGuid();
            var transaction = new Transaction(
                transactionId, 
                null,
                destinationAccountId,
                100,
                TransactionType.Deposit, 
                TransactionStatus.InProgress,
                DateTime.UtcNow, 
                Guid.NewGuid());

            var destinationAccountPoco = new AccountPoco { Id = destinationAccountId, Balance = 1000 };

            _accountRepositoryMock.Setup(repo => repo.GetById(destinationAccountId)).Returns(destinationAccountPoco);
            _transactionRepositoryMock.Setup(repo => repo.Insert(It.IsAny<TransactionPoco>())).Returns(transactionId);
            
            // Act
            var result = _transactionService.AddTransaction(transaction);

            // Assert
            Assert.Equal(transactionId, result);
            _accountRepositoryMock.Verify(repo => repo.GetById(destinationAccountId), Times.Once);
            _accountRepositoryMock.Verify(repo => repo.Update(It.IsAny<AccountPoco>()), Times.Once);
            _transactionRepositoryMock.Verify(repo => repo.Insert(It.IsAny<TransactionPoco>()), Times.Once);
            _transactionRepositoryMock.Verify(repo => repo.Update(It.IsAny<TransactionPoco>()), Times.Once);
        }

        [Fact]
        public void AddTransaction_WithInvalidWithdrawalTransaction_ShouldThrowArgumentException()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var originAccountId = Guid.NewGuid();
            var transaction = new Transaction(
                transactionId, 
                originAccountId,
                null,
                50,
                TransactionType.Withdrawal, 
                TransactionStatus.InProgress,
                DateTime.UtcNow, 
                Guid.NewGuid());

            var originAccountPoco = new AccountPoco { Id = originAccountId, Balance = 25 };

            _accountRepositoryMock.Setup(repo => repo.GetById(originAccountId)).Returns(originAccountPoco);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _transactionService.AddTransaction(transaction));
            Assert.Equal("OriginAccount does not have the necessary funds to do this operation. Please review the transaction amount.", exception.Message);
        }

        [Fact]
        public void AddTransaction_WithValidTransferTransaction_ShouldExecuteTransaction()
        {
            // Arrange
            var transactionId = Guid.NewGuid();
            var originAccountId = Guid.NewGuid();
            var destinationAccountId = Guid.NewGuid();

            var transaction = new Transaction(
                transactionId, 
                originAccountId,
                destinationAccountId,
                100,
                TransactionType.Transfer,
                TransactionStatus.InProgress,
                DateTime.UtcNow,
                Guid.NewGuid());

            var originAccountPoco = new AccountPoco { Id = originAccountId, Balance = 200 };
            var destinationAccountPoco = new AccountPoco { Id = destinationAccountId, Balance = 300 };

            _accountRepositoryMock.Setup(repo => repo.GetById(originAccountId)).Returns(originAccountPoco);
            _accountRepositoryMock.Setup(repo => repo.GetById(destinationAccountId)).Returns(destinationAccountPoco);
            _transactionRepositoryMock.Setup(repo => repo.Insert(It.IsAny<TransactionPoco>())).Returns(transactionId);

            // Act
            var result = _transactionService.AddTransaction(transaction);

            // Assert
            Assert.Equal(transactionId, result);
            _accountRepositoryMock.Verify(repo => repo.GetById(originAccountId), Times.Exactly(2));
            _accountRepositoryMock.Verify(repo => repo.GetById(destinationAccountId), Times.Once);
            _accountRepositoryMock.Verify(repo => repo.Update(It.IsAny<AccountPoco>()), Times.Exactly(2));
            _transactionRepositoryMock.Verify(repo => repo.Insert(It.IsAny<TransactionPoco>()), Times.Once);
            _transactionRepositoryMock.Verify(repo => repo.Update(It.IsAny<TransactionPoco>()), Times.Once);
        }
    }
}