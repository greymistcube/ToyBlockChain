using System;
using System.Collections.Generic;

namespace ToyBlockChain.Core
{
    /// <summary>
    /// Interface used by a client to access the node.
    /// </summary>
    public interface INodeClient
    {
        /// <summary>
        /// Checks if given transaction is in the pool.
        /// </summary>
        bool HasTransactionInPool(Transaction transaction);

        /// <summary>
        /// Checks if given transaction is in the pool.
        /// </summary>
        bool HasTransactionInPool(string transactionHashString);

        /// <summary>
        /// Checks if a transaction with the same sender is in the pool.
        /// </summary>
        bool HasSenderInPool(Transaction transaction);

        /// <summary>
        /// Checks if a transaction with the same sender is in the pool.
        /// </summary>
        bool HasSenderInPool(string senderAddress);

        /// <summary>
        /// Adds given transaction to the pool.
        /// </summary>
        void AddTransactionToPool(Transaction transaction);

        /// <summary>
        /// Returns a shallow copy of the account catalogue as a
        /// <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        Dictionary<string, Account> GetAccountCatalogue();
    }

    public partial class Node : INodeClient
    {
        bool INodeClient.HasTransactionInPool(Transaction transaction)
        {
            return _transactionPool.HasTransaction(transaction);
        }

        bool INodeClient.HasTransactionInPool(string transactionHashString)
        {
            return _transactionPool.HasTransaction(transactionHashString);
        }

        bool INodeClient.HasSenderInPool(Transaction transaction)
        {
            return _transactionPool.HasSender(transaction);
        }

        bool INodeClient.HasSenderInPool(string senderAddress)
        {
            return _transactionPool.HasSender(senderAddress);
        }

        void INodeClient.AddTransactionToPool(Transaction transaction)
        {
            AddTransactionToPool(transaction);
        }

        Dictionary<string, Account> INodeClient.GetAccountCatalogue()
        {
            return new Dictionary<string, Account>(_accountCatalogue.Catalogue);
        }
    }
}
