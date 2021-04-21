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

        /// <summary>
        /// Adds given block to the chain.
        /// </summary>
        void AddBlockToBlockChain(Block block);

        Block GetLastBlock();

        int GetTargetDifficulty();

        /// <summary>
        /// Returns a shallow copy of the transaction pool as a
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
            return _blockChain.GetLastBlock();
        }

        int INodeMiner.GetTargetDifficulty()
        {
            return _blockChain.GetTargetDifficulty();
        }

        void INodeMiner.AddBlockToBlockChain(Block block)
        {
            AddBlockToBlockChain(block);
        }

        Dictionary<string, Transaction> INodeMiner.GetTransactionPool()
        {
            return new Dictionary<string, Transaction>(_transactionPool.Pool);
        }
    }
}