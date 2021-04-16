using System;
using System.Collections.Generic;

namespace ToyBlockChain.Core
{
    /// <summary>
    /// Interface used by the main script to access the node.
    /// </summary>
    public interface INodeApp
    {
        void SyncBlockChain();
        void SyncAccountCatalogue();
        void SyncTransactionPool();

        void AddAccountToCatalogue(Account account);
        void AddTransactionToPool(Transaction transaction);
    }

    public partial class Node : INodeApp
    {
        void INodeApp.SyncBlockChain()
        {
        }

        void INodeApp.SyncAccountCatalogue()
        {
        }

        void INodeApp.SyncTransactionPool()
        {
        }

        /// <summary>
        /// Adds given account to the catalogue.
        /// </summary>
        void INodeApp.AddAccountToCatalogue(Account account)
        {
            _accountCatalogue.AddAccount(account);
        }

        /// <summary>
        /// Adds given transaction to the pool.
        /// </summary>
        void INodeApp.AddTransactionToPool(Transaction transaction)
        {
            _transactionPool.AddTransaction(transaction);
        }
    }
}