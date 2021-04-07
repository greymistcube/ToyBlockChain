using System;
using System.Collections.Generic;
using ToyBlockChain.Core;

namespace ToyBlockChain.Service
{
    public class Node
    {
        private readonly BlockChain _blockChain;
        private readonly HashSet<string> _book;
        private readonly Dictionary<string, Transaction> _pool;

        public Node()
        {
            _blockChain = new BlockChain();
            _book = new HashSet<string>();
            _pool = new Dictionary<string, Transaction>();
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
            // possibly unnecessarily restricts block validation
            else if (!HasTransactionInPool(block.Transaction))
            {
                throw new ArgumentException(
                    "given transaction is not from the pool");
            }
            else
            {
                _blockChain.AddBlock(block);
            }
            return;
        }

        /// <summary>
        /// Checks if given address is in the book.
        /// </summary>
        public bool HasAddressInBook(string address)
        {
            return _book.Contains(address);
        }

        /// <summary>
        /// Registers an address to the book.
        /// </summary>
        public void RegisterAddress(string address)
        {
            if (HasAddressInBook(address))
            {
                throw new ArgumentException("given address already exists");
            }
            else
            {
                _book.Add(address);
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
        /// Checks if given transaction is in the pool.
        /// Mainly used by a client to see if its generated transaction
        /// has been processed into the blockchain.
        /// </summary>
        public bool HasTransactionInPool(Transaction transaction)
        {
            return _pool.ContainsKey(transaction.HashString);
        }

        /// <summary>
        /// Registers a transaction to the pool. Only metablockchain level
        /// validations are performed.
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
                _pool.Add(transaction.HashString, transaction);
            }
        }

        /// <summary>
        /// Removes a transaction from the pool. Made private so that
        /// this can be called only when moving a transaction to a block
        /// in the blockchain. This makes a registered transaction
        /// uncancellable.
        /// </summary>
        private void RemoveTransaction(Transaction transaction)
        {
            _pool.Remove(transaction.HashString);
        }

        public Block LastBlock()
        {
            return _blockChain.LastBlock();
        }
    }
}
