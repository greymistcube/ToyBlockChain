using System;
using System.Collections.Generic;
using ToyBlockChain.Util;

namespace ToyBlockChain.Core
{
    public class InvalidBlockException : Exception
    {
        public InvalidBlockException()
        {
        }

        public InvalidBlockException(string message) : base(message)
        {
        }
    }

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
        /// Adds given block to the chain.
        /// </summary>
        internal void AddBlockToBlockChain(Block block)
        {
            _transactionPool.RemoveTransaction(block.Transaction);
            _blockChain.AddBlock(block);
        }

        /// <summary>
        /// Adds given account to the catalogue.
        /// </summary>
        internal void AddAccountToCatalogue(Account account)
        {
            _accountCatalogue.AddAccount(account);
        }

        /// <summary>
        /// Adds given transaction to the pool.
        /// </summary>
        internal void AddTransactionToPool(Transaction transaction)
        {
            if (_blockChain.HasTransaction(transaction))
            {
                throw new ArgumentException(
                    "given transaction already exists in the chain");
            }
            if (!_accountCatalogue.HasAccount(transaction.Sender))
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
                _transactionPool.AddTransaction(transaction);
            }
        }

        /// <summary>
        /// Checks if given transaction is in the blockchain.
        /// Mainly used to prevent the same transaction getting
        /// added to the chain more than once.
        /// </summary>
        internal bool HasTransactionInChain(Transaction transaction)
        {
            return _blockChain.HasTransaction(transaction);
        }

        /// <summary>
        /// Checks if given transaction is in the transaction pool.
        /// </summary>
        internal bool HasTransactionInPool(Transaction transaction)
        {
            return _transactionPool.HasTransaction(transaction);
        }
    }
}
