using System;
using System.Collections.Generic;
using ToyBlockChain.Core;

namespace ToyBlockChain.Service
{
    public class Node
    {
        private readonly BlockChain _blockChain;
        private readonly HashSet<string> _addressBook;
        private readonly Dictionary<string, Transaction> _transactionPool;
        private readonly bool _logging;

        public Node(bool logging = false)
        {
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
                throw new ArgumentException(
                    "given transaction already exists in the chain");
            }
            // Possibly unnecessarily restricts block validation.
            else if (!HasTransactionInPool(block.Transaction))
            {
                throw new ArgumentException(
                    "given transaction is not from the pool");
            }
            else
            {
                RemoveTransaction(block.Transaction);
                _blockChain.AddBlock(block);
                if (_logging)
                {
                    Console.WriteLine(
                        $"block {block.HashString} added to the blockchain");
                    Console.WriteLine(
                        $"transaction {block.Transaction.HashString} processed");
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
                        $"address {address} added to the address book");
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
            // TODO: Address verification turned off for debugging.
            /*
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
            */
            else
            {
                _transactionPool.Add(transaction.HashString, transaction);
                if (_logging)
                {
                    Console.WriteLine(
                        $"transaction {transaction.HashString} "
                        + "added to the transaction pool");
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
            return _blockChain.TargetDifficulty();
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
