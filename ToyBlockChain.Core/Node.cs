using System;

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
            _transactionPool = new TransactionPool();
            _accountCatalogue = new AccountCatalogue();
        }

        /// <summary>
        /// Adds given block to the chain.
        /// </summary>
        internal void AddBlockToBlockChain(Block block)
        {
            // Check soundness and validity for safe operation.
            block.CheckSoundness();
            _transactionPool.ValidateBlock(block);
            _accountCatalogue.ValidateBlock(block);
            _blockChain.ValidateBlock(block);

            _transactionPool.RemoveTransaction(block.Transaction);
            _accountCatalogue.ConsumeTransaction(block.Transaction);
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
            // Check soundness and validity for safe operation.
            transaction.CheckSoundness();
            _blockChain.ValidateTransaction(transaction);
            _accountCatalogue.ValidateTransaction(transaction);
            _transactionPool.ValidateTransaction(transaction);

            _transactionPool.AddTransaction(transaction);
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
