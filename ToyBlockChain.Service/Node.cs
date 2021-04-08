using System;
using System.Collections.Generic;
using ToyBlockChain.Core;

namespace ToyBlockChain.Service
{
    public class Node
    {
        private const int DEFAULT_DIFFICULTY = 4;
        private const int MOVING_AVERAGE_LENGTH = 4;
        private const int MINING_INTERVAL_LOWER_LIMIT = 4;
        private const int MINING_INTERVAL_UPPER_LIMIT = 8;
        private readonly BlockChain _blockChain;
        private readonly HashSet<string> _addressBook;
        private readonly Dictionary<string, Transaction> _transactionPool;
        private readonly bool _logging;
        private int _difficulty;

        public Node(bool logging = false)
        {
            _difficulty = DEFAULT_DIFFICULTY;
            _blockChain = new BlockChain();
            _addressBook = new HashSet<string>();
            _transactionPool = new Dictionary<string, Transaction>();
            _logging = logging;
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
                if (_logging)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(
                        $"block {block.HashString[0..16]} contains "
                        + "a transaction already in the blockchain");
                    Console.ResetColor();
                }
                return;
            }
            // Possibly unnecessarily restricts block validation.
            else if (!HasTransactionInPool(block.Transaction))
            {
                if (_logging)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(
                        $"block {block.HashString[0..16]} contains "
                        + "an unknown transaction");
                    Console.ResetColor();
                }
            }
            // Ensures block timestamps are in order.
            else if (
                _blockChain.LastBlock() != null
                && !(_blockChain.LastBlock().BlockHeader.Timestamp
                    <= block.BlockHeader.Timestamp))
            {
                if (_logging)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(
                        $"block {block.HashString[0..16]} has an "
                        + "invalid timestamp");
                    Console.ResetColor();
                }
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
                if (_logging)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(
                        $"block {block.HashString[0..16]} with "
                        + $"transaction {block.Transaction.HashString[0..16]} "
                        + "added to the blockchain");
                    Console.ResetColor();
                    Console.WriteLine(block);
                }
            }
            return;
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
                double sma = (end - start) / (subChain.Count - 1);
                if (sma < MINING_INTERVAL_LOWER_LIMIT)
                {
                    _difficulty += 1;
                }
                else if (sma > MINING_INTERVAL_UPPER_LIMIT)
                {
                    // Prevents the difficulty getting too low.
                    _difficulty = Math.Max(DEFAULT_DIFFICULTY, _difficulty - 1);
                }
            }
            return;
        }

        /// <summary>
        /// Checks if given address is in the address book.
        /// </summary>
        public bool HasAddressInBook(string address)
        {
            return _addressBook.Contains(address);
        }

        /// <summary>
        /// Registers an address to the address book.
        /// </summary>
        public void RegisterAddress(string address)
        {
            if (HasAddressInBook(address))
            {
                throw new ArgumentException("given address already exists");
            }
            else
            {
                _addressBook.Add(address);
                if (_logging)
                {
                    Console.WriteLine(
                        $"address {address[0..16]} added to the address book");
                }
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
            else if (!HasAddressInBook(transaction.Sender))
            {
                throw new ArgumentException(
                    "sender address not found in the book");
            }
            else if (!HasAddressInBook(transaction.Recipient))
            {
                throw new ArgumentException(
                    "recipient address not found in the book");
            }
            else
            {
                _transactionPool.Add(transaction.HashString, transaction);
                if (_logging)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(
                        $"transaction {transaction.HashString[0..16]} "
                        + $"from sender {transaction.Sender[0..16]} "
                        + "added to the transaction pool");
                    Console.ResetColor();
                }
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

        public List<string> AddressBook
        {
            get
            {
                return new List<string>(_addressBook);
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
