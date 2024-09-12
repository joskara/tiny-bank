using ApplicationServices.Interfaces;
using ApplicationServices.Mappers;
using Data.POCOs;
using Data.Repositories;
using Domain;
using Infrastructure.Enums;

namespace ApplicationServices.Implementations;

public class TransactionService(
    IBaseRepository<TransactionPoco> transactionRepository,
    IBaseRepository<AccountPoco> accountRepository) : ITransactionService
{
    public IEnumerable<Transaction> GetTransactions()
    {
        var transactionsPoco =  transactionRepository.Get();
        return transactionsPoco.ToDomain();
    }

    public Transaction? GetTransaction(Guid transactionId)
    {
        var transactionPoco = transactionRepository.GetById(transactionId);

        if (transactionPoco == null)
        {
            throw new ArgumentException($"Transaction with id {transactionId} not found");
        }

        return transactionPoco.ToDomain();
    }

    // Validate if origin and destination accounts, if not null, they exist
    // validate if according to transaction type, the appropriate field is filled.
    // This method as well has waaaaay more responsibility that it should.
    // With enough time I would have split this into a strategy pattern or similar to have
    // classes responsible for validating and executing them instead of the service.
    public Guid AddTransaction(Transaction transaction)
    {
        ValidateTransaction(transaction);
        transaction.UpdateStatus(TransactionStatus.InProgress);
        
        var transactionId = transactionRepository.Insert(transaction.ToPoco());

        ExecuteTransaction(transaction);
        
        transaction.UpdateStatus(TransactionStatus.Done);
        transactionRepository.Update(transaction.ToPoco());
        
        return transactionId;
    }

    private void ExecuteTransaction(Transaction transaction)
    {
        switch (transaction.Type)
        {
            case TransactionType.Deposit:
                if (transaction.DestinationAccountId != null)
                {
                    var destinationAccount = accountRepository.GetById(transaction.DestinationAccountId.Value);
                    var destinationAccountDomain = destinationAccount.ToDomain();
                    
                    destinationAccountDomain.Deposit(transaction.Amount);
                    
                    accountRepository.Update(destinationAccountDomain.ToPoco());
                }
                break;
            case TransactionType.Withdrawal:
                if (transaction.OriginAccountId != null)
                {
                    var originAccount = accountRepository.GetById(transaction.OriginAccountId.Value);
                    var originAccountDomain = originAccount.ToDomain();
                    
                    originAccountDomain.Withdrawal(transaction.Amount);
                    
                    accountRepository.Update(originAccountDomain.ToPoco());
                }
                break;
            case TransactionType.Transfer:
                if (transaction.OriginAccountId != null)
                {
                    var originAccount = accountRepository.GetById(transaction.OriginAccountId.Value);
                    var originAccountDomain = originAccount.ToDomain();
                    
                    originAccountDomain.Withdrawal(transaction.Amount);
                    
                    accountRepository.Update(originAccountDomain.ToPoco());
                }
                
                if (transaction.DestinationAccountId != null)
                {
                    var destinationAccount = accountRepository.GetById(transaction.DestinationAccountId.Value);
                    var destinationAccountDomain = destinationAccount.ToDomain();
                    
                    destinationAccountDomain.Deposit(transaction.Amount);
                    
                    accountRepository.Update(destinationAccountDomain.ToPoco());
                }
                break;
            case TransactionType.Error:
            default:
                throw new ApplicationException("The transaction type is not supported, and executing it failed.");
        }
    }

    private void ValidateTransaction(Transaction transaction)
    {
        if (transaction.Amount == 0)
        {
            throw new ArgumentException($"Transaction should not have Amount be zero");
        }
        
        switch (transaction.Type)
        {
            case TransactionType.Deposit:
                if (transaction.OriginAccountId.HasValue && transaction.OriginAccountId != Guid.Empty)
                {
                    throw new ArgumentException($"Transaction with Type {transaction.Type} should not have an OriginAccount defined");
                }

                if (!transaction.DestinationAccountId.HasValue || transaction.DestinationAccountId == Guid.Empty)
                {
                    throw new ArgumentException($"Transaction with Type {transaction.Type} should have an OriginAccount defined");
                }

                break;
            case TransactionType.Withdrawal:
                if (transaction.DestinationAccountId.HasValue && transaction.DestinationAccountId != Guid.Empty)
                {
                    throw new ArgumentException($"Transaction with Type {transaction.Type} should not have a DestinationAccount defined");
                }

                if (!transaction.OriginAccountId.HasValue || transaction.OriginAccountId == Guid.Empty)
                {
                    throw new ArgumentException($"Transaction with Type {transaction.Type} should have an OriginAccount defined");
                }
                
                var originAccountWithdrawal = accountRepository.GetById(transaction.OriginAccountId.Value);
                if (originAccountWithdrawal.Balance < transaction.Amount)
                {
                    throw new ArgumentException($"OriginAccount does not have the necessary funds to do this operation. Please review the transaction amount.");
                }

                break;
            case TransactionType.Transfer:
                if (!transaction.OriginAccountId.HasValue ||
                    transaction.OriginAccountId == Guid.Empty ||
                    !transaction.DestinationAccountId.HasValue ||
                    transaction.DestinationAccountId == Guid.Empty)
                {
                    throw new ArgumentException($"Transaction with Type {transaction.Type} should have both an Origin and DestinationAccount defined");
                }
                
                var originAccountTransfer = accountRepository.GetById(transaction.OriginAccountId.Value);
                if (originAccountTransfer.Balance < transaction.Amount)
                {
                    throw new ArgumentException($"OriginAccount does not have the necessary funds to do this operation. Please review the transaction amount.");
                }

                break;
            case TransactionType.Error:
            default:
                throw new ArgumentException($"Transaction with invalid Type");
        }
    }
}