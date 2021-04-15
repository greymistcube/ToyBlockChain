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
    public class Node
    {
        private const int DEFAULT_DIFFICULTY = 4;
        private const int MOVING_AVERAGE_LENGTH = 4;
        private const int MINING_INTERVAL_LOWER_LIMIT = 4;
        private const int MINING_INTERVAL_UPPER_LIMIT = 8;
        private readonly BlockChain _blockChain;
        private readonly AccountCatalogue _accountCatalogue;
        private readonly Dictionary<string, Transaction> _transactionPool;

        private int _difficulty;

        public Node()
        {
            _difficulty = DEFAULT_DIFFICULTY;
            _blockChain = new BlockChain();
            _accountCatalogue = new AccountCatalogue();
            _transactionPool = new Dictionary<string, Transaction>();
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
                Logger.Log(
                    $"block {block.HashString[0..16]} with "
                    + $"transaction {block.Transaction.HashString[0..16]} "
                    + "added to the blockchain",
                    Logger.INFO, ConsoleColor.Green);
                Logger.Log($"{block}", Logger.DEBUG, ConsoleColor.White);
            }
            return;
        }

        public void AddAccount(Account account)
        {
            _accountCatalogue.AddAccount(account);
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
        /// Registers an account to the account catalogue.
        /// </summary>
        public void RegisterAddress(Account account)
        {
            if (_accountCatalogue.HasAccount(account))
            {
                throw new ArgumentException("given address already exists");
            }
            else
            {
                _accountCatalogue.AddAccount(account);
                Logger.Log(
                    $"address {account.Address[0..16]} "
                    + "added to the address book",
                    Logger.INFO, ConsoleColor.White);
            }
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
            return _transactionPool.ContainsKey(transaction.HashString);
        }

        /// <summary>
        /// Registers a transaction to the transaction pool.
        /// Only metablockchain level validations are performed.
        /// </summary>
        public void RegisterTransaction(Transaction transaction)
        {
            if (HasTransactionInChain(transaction))
            {
                throw new ArgumentException(
                    "given transaction already exists in the chain");
            }
            else if (HasTransactionInPool(transaction))
            {
                throw new ArgumentException(
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
                _transactionPool.Add(transaction.HashString, transaction);
                Logger.Log(
                    $"transaction {transaction.HashString[0..16]} "
                    + $"from sender {transaction.Sender[0..16]} "
                    + "added to the transaction pool",
                    Logger.INFO, ConsoleColor.Yellow);
            }
        }

        /// <summary>
        /// Removes a transaction from the transaction pool.
        /// Made private so that this can be called only when moving
        /// a transaction to a block in the blockchain.
        /// This makes a registered transaction uncancellable.
        /// </summary>
        private void RemoveTransaction(Transaction transaction)
        {
            _transactionPool.Remove(transaction.HashString);
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

        public List<Transaction> TransactionPool
        {
            get
            {
                return new List<Transaction>(_transactionPool.Values);
            }
        }
    }
}
