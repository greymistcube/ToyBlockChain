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
        private const int DEFAULT_DIFFICULTY = 4;
        private const int MOVING_AVERAGE_LENGTH = 4;
        private const int MINING_INTERVAL_LOWER_LIMIT = 4;
        private const int MINING_INTERVAL_UPPER_LIMIT = 8;
        private readonly BlockChain _blockChain;
        private readonly AccountCatalogue _accountCatalogue;
        private readonly TransactionPool _transactionPool;

        private int _difficulty;

        public Node()
        {
            _difficulty = DEFAULT_DIFFICULTY;
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
                AdjustDifficulty();
            }
            return;
        }

        /// <summary>
        /// Adds given account to the catalogue.
        /// </summary>
        public void AddAccountToCatalogue(Account account)
        {
            _accountCatalogue.AddAccount(account);
        }

        /// <summary>
        /// Adds given transaction to the pool.
        /// </summary>
        public void AddTransactionToPool(Transaction transaction)
        {
            _transactionPool.AddTransaction(transaction);
        }

        /// <summary>
        /// Adjusts the target difficulty for the next prospective block.
        /// Uses a simple moving average of time spend to mine the last
        /// <c>MOVING_AVERAGE_LENGTH - 1</c> blocks.
        ///
        /// If the average is lower than
        /// <see cref="MINING_INTERVAL_LOWER_LIMIT"/>, then the target
        /// difficulty is raised. If the average is higher than
        /// <see cref="MINING_INTERVAL_UPPER_LIMIT"/>, then the target
        /// difficulty is lowerd.
        ///
        /// Note that the terget difficulty is never lowered below
        /// <see cref="DEFAULT_DIFFICULTY"/>.
        /// </summary>
        private void AdjustDifficulty()
        {
            // Get the last MOVING_AVERAGE_LENGTH number of blocks.
            List<Block> chain = _blockChain.Chain;
            List<Block> subChain = chain.GetRange(
                Math.Max(0, chain.Count - MOVING_AVERAGE_LENGTH),
                Math.Min(chain.Count, MOVING_AVERAGE_LENGTH));

            if (subChain.Count > 1)
            {
                long start = subChain[0].BlockHeader.Timestamp;
                long end = subChain[subChain.Count - 1].BlockHeader.Timestamp;
                double simpleMovingAverage = (
                    (end - start) / (subChain.Count - 1));
                if (simpleMovingAverage < MINING_INTERVAL_LOWER_LIMIT)
                {
                    _difficulty += 1;
                }
                else if (simpleMovingAverage > MINING_INTERVAL_UPPER_LIMIT)
                {
                    // Prevents the difficulty getting too low.
                    _difficulty = Math.Max(DEFAULT_DIFFICULTY, _difficulty - 1);
                }
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

        public int TargetDifficulty()
        {
            return _difficulty;
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
