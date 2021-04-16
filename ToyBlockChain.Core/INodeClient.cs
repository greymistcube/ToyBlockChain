using System;
using System.Collections.Generic;

namespace ToyBlockChain.Core
{
    public interface INodeClient
    {
        /// <summary>
        /// Checks if given transaction is in the pool.
        /// </summary>
        bool HasTransactionInPool(Transaction transaction);

        /// <summary>
        /// Adds given transaction to the pool.
        /// </summary>
        void AddTransactionToPool(Transaction transaction);

        /// <summary>
        /// Get a shallow copy of the account catalogue as a dictionary.
        /// </summary>
        Dictionary<string, Account> GetAccountCatalogue();
    }

    public partial class Node : INodeClient
    {
        bool INodeClient.HasTransactionInPool(Transaction transaction)
        {
            return _transactionPool.HasTransaction(transaction);
        }

        void INodeClient.AddTransactionToPool(Transaction transaction)
        {
            if (HasTransactionInChain(transaction))
            {
                throw new ArgumentException(
                    "given transaction already exists in the chain");
            }
            else if (HasTransactionInPool(transaction))
            {
                throw new TransactionInPoolException(
                    "given transaction already exists in the pool");
            }
            else if (!_accountCatalogue.HasAccount(transaction.Sender))
            {
                throw new ArgumentException(
                    "sender address not found in the book");
            }
            else if (!_accountCatalogue.HasAccount(transaction.Recipient))
            {
                throw new ArgumentException(
                    "recipient address not found in the book");
            }
            else
            {
                _transactionPool.AddTransaction(transaction);
            }
        }

        Dictionary<string, Account> INodeClient.GetAccountCatalogue()
        {
            return new Dictionary<string, Account>(_accountCatalogue.Catalogue);
        }
    }
}
