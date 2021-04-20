using System;
using System.Collections.Generic;

namespace ToyBlockChain.Core
{
    /// <summary>
    /// Interface used by a miner to access the node.
    /// </summary>
    public interface INodeMiner
    {
        /// <summary>
        /// Checks if given transaction is in the pool.
        /// </summary>
        bool HasTransactionInPool(Transaction transaction);

        void AddBlockToBlockChain(Block block);

        Block GetLastBlock();

        int GetTargetDifficulty();

        /// <summary>
        /// Get a shallow copy of the transaction pool as a
        /// <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        Dictionary<string, Transaction> GetTransactionPool();
    }

    public partial class Node : INodeMiner
    {
        bool INodeMiner.HasTransactionInPool(Transaction transaction)
        {
            return _transactionPool.HasTransaction(transaction);
        }

        Block INodeMiner.GetLastBlock()
        {
            return _blockChain.LastBlock();
        }

        int INodeMiner.GetTargetDifficulty()
        {
            return GetTargetDifficulty();
        }

        void INodeMiner.AddBlockToBlockChain(Block block)
        {
            _blockChain.AddBlock(block);
        }

        Dictionary<string, Transaction> INodeMiner.GetTransactionPool()
        {
            return new Dictionary<string, Transaction>(_transactionPool.Pool);
        }
    }
}