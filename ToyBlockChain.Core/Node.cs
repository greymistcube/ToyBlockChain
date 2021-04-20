using System;
using System.Collections.Generic;
using ToyBlockChain.Util;

namespace ToyBlockChain.Core
{
    /// <summary>
    /// The class representing a node in a blockchain ecosystem.
    /// Generally handles higher level logic, such as enforcing
    /// policy / metablockchain level validation.
    /// </summary>
    public partial class Node
    {
        private readonly BlockChain _blockChain;
        private readonly AccountCatalogue _accountCatalogue;
        private readonly TransactionPool _transactionPool;

        public Node()
        {
            _blockChain = new BlockChain();
            _accountCatalogue = new AccountCatalogue();
            _transactionPool = new TransactionPool();
        }

        /// <summary>
        /// Adds a block to the blockchain. Only metablockchain level
        /// validations are performed. Additional validations occur
        /// when given block is passed to the <see cref="BlockChain"/> class.
        /// </summary>
        public void AddBlock(Block block)
        {
            if (HasTransactionInChain(block.Transaction))
            {
                Logger.Log(
                    $"block {block.HashString[0..16]} contains "
                    + "a transaction already in the blockchain",
                    Logger.INFO, ConsoleColor.Red);
            }
            // Possibly unnecessarily restricts block validation.
            else if (!HasTransactionInPool(block.Transaction))
            {
                Logger.Log(
                    $"block {block.HashString[0..16]} contains "
                    + "an unknown transaction",
                    Logger.INFO, ConsoleColor.Red);
            }
            // Ensures block timestamps are in order.
            else if (
                _blockChain.LastBlock() != null
                && !(_blockChain.LastBlock().BlockHeader.Timestamp
                    <= block.BlockHeader.Timestamp))
            {
                Logger.Log(
                    $"block {block.HashString[0..16]} has an "
                    + "invalid timestamp",
                    Logger.INFO, ConsoleColor.Red);
            }
            // Transaction must be removed from the pool
            // before getting added to the blockchain.
            // Once the blockchain has been updated, adjust
            // the target difficulty.
            else
            {
                RemoveTransaction(block.Transaction);
                _blockChain.AddBlock(block);
            }
            return;
        }

        /// <summary>
        /// Checks if given transaction is in the blockchain.
        /// Mainly used to prevent the same transaction getting
        /// added to the chain more than once.
        /// </summary>
        public bool HasTransactionInChain(Transaction transaction)
        {
            return _blockChain.HasTransaction(transaction);
        }

        /// <summary>
        /// Checks if given transaction is in the transaction pool.
        /// </summary>
        public bool HasTransactionInPool(Transaction transaction)
        {
            return _transactionPool.HasTransaction(transaction);
        }

        /// <summary>
        /// Removes a transaction from the transaction pool.
        /// Made private so that this can be called only when moving
        /// a transaction to a block in the blockchain.
        /// This makes a registered transaction uncancellable.
        /// </summary>
        private void RemoveTransaction(Transaction transaction)
        {
            _transactionPool.RemoveTransaction(transaction);
        }

        public Block LastBlock()
        {
            return _blockChain.LastBlock();
        }

        public AccountCatalogue AccountCatalogue
        {
            get
            {
                return _accountCatalogue;
            }
        }

        public TransactionPool TransactionPool
        {
            get
            {
                return _transactionPool;
            }
        }
    }
}
