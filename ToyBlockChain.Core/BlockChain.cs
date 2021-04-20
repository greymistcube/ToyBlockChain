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
        public const int DIFFICULTY_INIT = DIFFICULTY_MIN;
        public const int DIFFICULTY_MIN = 8;
        public const int DIFFICULTY_MAX = 256;
        public const int MOVING_AVERAGE_LENGTH = 8;
        public const int MINING_INTERVAL_LOWER_LIMIT = 4;
        public const int MINING_INTERVAL_UPPER_LIMIT = 8;

        private List<Block> _chain;
        private int _difficulty;

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
            AdjustDifficulty();
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
                    AdjustDifficulty();
                }
                else
                {
                    throw new BlockInvalidForChainException(
                        "block is not valid for the chain");
                }
            }
        }

        /// <summary>
        /// Adjusts the target difficulty for the next prospective block.
        /// Uses a simple moving average of time spent to mine the last
        /// <c>MOVING_AVERAGE_LENGTH - 1</c> blocks in the last chunk.
        /// <br/>
        /// If the average is lower than
        /// <see cref="MINING_INTERVAL_LOWER_LIMIT"/>, the target
        /// difficulty is raised. If the average is higher than
        /// <see cref="MINING_INTERVAL_UPPER_LIMIT"/>, the target
        /// difficulty is lowerd.
        /// <br/>
        /// Note that the terget difficulty is never lowered below
        /// <see cref="DIFFICULTY_MIN"/>.
        /// </summary>
        private void AdjustDifficulty()
        {
            if (_chain.Count < MOVING_AVERAGE_LENGTH)
            {
                _difficulty = DIFFICULTY_INIT;
            }
            else
            {
                int startIndex = (
                    ((_chain.Count - 1) / MOVING_AVERAGE_LENGTH)
                    * MOVING_AVERAGE_LENGTH);
                int endIndex = startIndex + (MOVING_AVERAGE_LENGTH - 1);
                long startTimestamp = _chain[startIndex].BlockHeader.Timestamp;
                long endTimestamp = _chain[endIndex].BlockHeader.Timestamp;
                double simpleMovingAverage = ((
                    endTimestamp - startTimestamp)
                    / (MOVING_AVERAGE_LENGTH - 1));
                if (simpleMovingAverage < MINING_INTERVAL_LOWER_LIMIT)
                {
                    _difficulty = Math.Min(DIFFICULTY_MAX, _difficulty + 1);
                }
                else if (MINING_INTERVAL_UPPER_LIMIT < simpleMovingAverage)
                {
                    _difficulty = Math.Max(DIFFICULTY_MIN, _difficulty - 1);
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

        public int GetTargetDifficulty()
        {
            return _difficulty;
        }
    }
}
