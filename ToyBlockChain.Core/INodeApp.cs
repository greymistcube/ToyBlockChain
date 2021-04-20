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
        void SyncAccountCatalogue(string serializedString);
        void SyncTransactionPool(string serializedString);

        string GetAccountCatalogueSerializedString();
        string GetTransactionPoolSerializedString();

        void AddBlockToChain(Block block);
        void AddAccountToCatalogue(Account account);
        void AddTransactionToPool(Transaction transaction);
    }

    public partial class Node : INodeApp
    {
        void INodeApp.SyncBlockChain()
        {
        }

        void INodeApp.SyncAccountCatalogue(string serializedString)
        {
            _accountCatalogue.Sync(serializedString);
        }

        void INodeApp.SyncTransactionPool(string serializedString)
        {
            _transactionPool.Sync(serializedString);
        }

        string INodeApp.GetAccountCatalogueSerializedString()
        {
            return _accountCatalogue.ToSerializedString();
        }

        string INodeApp.GetTransactionPoolSerializedString()
        {
            return _transactionPool.ToSerializedString();
        }

        /// <summary>
        /// Adds given block to the chain.
        /// </summary>
        void INodeApp.AddBlockToChain(Block block)
        {
            // TODO: dirty fix
            this.AddBlock(block);
            // _blockChain.AddBlock(block);
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