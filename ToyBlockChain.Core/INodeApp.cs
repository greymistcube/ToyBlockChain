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
        void SyncTransactionPool(string serializedString);

        string GetBlockChainSerializedString();
        string GetAccountCatalogueSerializedString();
        string GetTransactionPoolSerializedString();

        /// <summary>
        /// Adds given block to the chain.
        /// </summary>
        void AddBlockToChain(Block block);

        /// <summary>
        /// Adds given transaction to the pool.
        /// </summary>
        void AddTransactionToPool(Transaction transaction);
    }

    public partial class Node : INodeApp
    {
        void INodeApp.SyncBlockChain(string serializedString)
        {
            if (serializedString != null && serializedString.Length > 0)
            {
                // TODO: Bad placement.
                _accountCatalogue.Dump();
                _blockChain.Dump();
                _transactionPool.Dump();
                string[] blockStrings = serializedString
                    .Split(BlockChain.SEPARATOR);
                foreach (string blockString in blockStrings)
                {
                    Block block = new Block(blockString);
                    AddBlockToBlockChain(block);
                }
            }
        }

        void INodeApp.SyncTransactionPool(string serializedString)
        {
            _transactionPool.Sync(serializedString);
        }

        string INodeApp.GetBlockChainSerializedString()
        {
            return _blockChain.ToSerializedString();
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
            AddBlockToBlockChain(block);
        }

        void INodeApp.AddTransactionToPool(Transaction transaction)
        {
            AddTransactionToPool(transaction);
        }
    }
}