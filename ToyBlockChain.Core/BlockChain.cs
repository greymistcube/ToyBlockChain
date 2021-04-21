using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    /// Thrown when the previoush hash value for given block does not match
    /// the hash value of the last block in the chain.
    /// </summary>
    public class BlockPreviousHashMismatchException
        : BlockInvalidForChainException
    {
        public BlockPreviousHashMismatchException()
        {
        }

        public BlockPreviousHashMismatchException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Thrown when the timestamp of given block is earlier than the timestamp
    /// of the last block in the chain.
    /// </summary>
    public class BlockInvalidTimestampException : BlockInvalidForChainException
    {
        public BlockInvalidTimestampException()
        {
        }

        public BlockInvalidTimestampException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Thrown when the index of a block to add is less than or equal
    /// to the index of the last block in the chain.
    /// </summary>
    public class BlockIndexLowForChainException
        : BlockInvalidForChainException
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
    public class BlockIndexHighForChainException
        : BlockInvalidForChainException
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

        internal BlockChain()
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

        internal void AddBlock(Block block)
        {
            Block lastBlock = GetLastBlock();

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
            else if (
                (GetLastBlock() != null)
                && (GetLastBlock().HashString != block.PreviousHashString))
            {
                throw new BlockPreviousHashMismatchException(
                    "previous hash for given block does not match " +
                    "hash of the last block in the chain");
            }
            else if (
                (GetLastBlock() != null)
                && !(GetLastBlock().BlockHeader.Timestamp
                    <= block.BlockHeader.Timestamp))
            {
                throw new BlockInvalidTimestampException(
                    "timestamp for given block is earlier than "
                    + "timestamp for the last block in the chain");
            }
            else
            {
                _chain.Add(block);
                AdjustDifficulty();
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
        /// <see cref="DIFFICULTY_MIN"/> or raised above
        /// <see cref="DIFFICULTY_MAX"/>.
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

        internal bool HasTransaction(Transaction transaction)
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

        internal int GetTargetDifficulty()
        {
            return _difficulty;
        }

        internal Block GetLastBlock()
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

        public string ToSerializedString()
        {
            return String.Join(
                SEPARATOR,
                _chain.Select(
                    block => block.ToSerializedString()));
        }

        public byte[] ToSerializedBytes()
        {
            return Encoding.UTF8.GetBytes(ToSerializedString());
        }
    }
}
