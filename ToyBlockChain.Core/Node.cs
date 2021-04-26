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
            ValidateBlock(block);

            _transactionPool.RemoveTransaction(block.Transaction);
            _accountCatalogue.ConsumeTransaction(block.Transaction);
            _blockChain.AddBlock(block);
        }

        /// <summary>
        /// Adds given transaction to the pool.
        /// </summary>
        internal void AddTransactionToPool(Transaction transaction)
        {
            // Check soundness and validity for safe operation.
            transaction.CheckSoundness();
            ValidateTransaction(transaction);

            _transactionPool.AddTransaction(transaction);
        }

        /// <summary>
        /// Checks if given block can be safely accepted to this node.
        /// </summary>
        private void ValidateBlock(Block block)
        {
            _blockChain.ValidateBlock(block);
            _accountCatalogue.ValidateBlock(block);
            // In case the transaction of given block is not found in the pool,
            // try to add it to the pool on the fly.
            try
            {
                _transactionPool.ValidateBlock(block);
            }
            catch (BlockInvalidException)
            {
                AddTransactionToPool(block.Transaction);
            }
        }

        /// <summary>
        /// Checks if given transaction can be safely accepted to this node.
        /// </summary>
        private void ValidateTransaction(Transaction transaction)
        {
            _blockChain.ValidateTransaction(transaction);
            _accountCatalogue.ValidateTransaction(transaction);
            _transactionPool.ValidateTransaction(transaction);
        }
    }
}
