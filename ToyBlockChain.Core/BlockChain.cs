using System;
using System.Collections.Generic;

namespace ToyBlockChain.Core
{
    /// <summary>
    /// Thrown when the index of a block is equal to the index of
    /// the last block in the chain plus one but does not pass the
    /// validation.
    /// </summary>
    public class BlockInvalidForChainException : Exception
    {
        public BlockInvalidForChainException()
        {
        }

        public BlockInvalidForChainException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Thrown when the index of a block to add is less than or equal
    /// to the index of the last block in the chain.
    /// </summary>
    public class BlockIndexLowForChainException : Exception
    {
        public BlockIndexLowForChainException()
        {
        }

        public BlockIndexLowForChainException(string message) : base (message)
        {
        }
    }

    /// <summary>
    /// Thrown when the index of a block to add is greater than
    /// the index of the last block in the chain plus one.
    /// </summary>
    public class BlockIndexHighForChainException : Exception
    {
        public BlockIndexHighForChainException()
        {
        }

        public BlockIndexHighForChainException(string message) : base(message)
        {
        }
    }

    public class BlockChain
    {
        public const string SEPARATOR = "<BC>";
        private List<Block> _chain;

        public BlockChain()
        {
            _chain = new List<Block>();
            return;
        }

        public void Sync(string serializedString)
        {
            _chain = new List<Block>();
            if (serializedString != null && serializedString.Length > 0)
            {
                string[] blockStrings = serializedString.Split(SEPARATOR);
                foreach (string blockString in blockStrings)
                {
                    Block block = new Block(blockString);
                    _chain.Add(block);
                }
            }
        }

        public Block LastBlock()
        {
            if (_chain.Count > 0)
            {
                return _chain[_chain.Count - 1];
            }
            else
            {
                return null;
            }
        }

        public void AddBlock(Block block)
        {
            Block lastBlock = LastBlock();

            if (_chain.Count > block.Index)
            {
                throw new BlockIndexLowForChainException(
                    "given block index is too low");
            }
            else if (_chain.Count < block.Index)
            {
                throw new BlockIndexHighForChainException(
                    "given block index is too high");
            }
            else
            {
                if (ValidateBlock(block))
                {
                    _chain.Add(block);
                }
                else
                {
                    throw new BlockInvalidForChainException(
                        "block is not valid for the chain");
                }
            }
        }

        private bool ValidateBlock(Block block)
        {
            return (
                block.IsValid()
            ) && (
                LastBlock() == null
                || LastBlock().HashString == block.PreviousHashString);
        }

        public bool HasTransaction(Transaction transaction)
        {
            foreach (Block block in _chain)
            {
                if (transaction.HashString == block.Transaction.HashString)
                {
                    return true;
                }
            }
            return false;
        }

        public List<Block> Chain
        {
            get
            {
                return _chain;
            }
        }
    }
}
