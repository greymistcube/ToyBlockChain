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
            Logger.Log(
                $"[Info] Node: Block {block.BlockHeader.HashString[0..16]} "
                + "added to the chain",
                Logger.INFO, ConsoleColor.Yellow);
        }

        /// <summary>
        /// Adds given account to the catalogue.
        /// </summary>
        internal void AddAccountToCatalogue(Account account)
        {
            _accountCatalogue.AddAccount(account);
            Logger.Log(
                $"[Info] Node: Account {account.Address[0..16]} "
                + "added to the catalogue",
                Logger.INFO, ConsoleColor.Yellow);
        }

        /// <summary>
        /// Adds given transaction to the pool.
        /// </summary>
        internal void AddTransactionToPool(Transaction transaction)
        {
            // Validate against the chain and the catalogue.
            _blockChain.ValidateTransaction(transaction);
            _accountCatalogue.ValidateTransaction(transaction);

            _transactionPool.AddTransaction(transaction);
            Logger.Log(
                $"[Info] Node: Transaction {transaction.HashString[0..16]} "
                + "added to the pool",
                Logger.INFO, ConsoleColor.Yellow);
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
