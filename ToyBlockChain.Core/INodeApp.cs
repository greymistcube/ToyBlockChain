using System;
using System.Collections.Generic;

namespace ToyBlockChain.Core
{
    /// <summary>
    /// Interface used by the main script to access the node.
    /// </summary>
    public interface INodeApp
    {
        void SyncBlockChain(string serializedString);
        void SyncAccountCatalogue(string serializedString);
        void SyncTransactionPool(string serializedString);

        string GetAccountCatalogueSerializedString();
        string GetTransactionPoolSerializedString();

        /// <summary>
        /// Adds given block to the chain.
        /// </summary>
        void AddBlockToChain(Block block);

        /// <summary>
        /// Adds given account to the catalogue.
        /// </summary>
        void AddAccountToCatalogue(Account account);

        /// <summary>
        /// Adds given transaction to the pool.
        /// </summary>
        void AddTransactionToPool(Transaction transaction);
    }

    public partial class Node : INodeApp
    {
        void INodeApp.SyncBlockChain(string serializedString)
        {
            // TODO: Implement.
            throw new NotImplementedException();
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

        void INodeApp.AddBlockToChain(Block block)
        {
            // TODO: Dirty fix.
            AddBlockToBlockChain(block);
            // _blockChain.AddBlock(block);
        }

        void INodeApp.AddAccountToCatalogue(Account account)
        {
            _accountCatalogue.AddAccount(account);
        }

        void INodeApp.AddTransactionToPool(Transaction transaction)
        {
            _transactionPool.AddTransaction(transaction);
        }
    }
}