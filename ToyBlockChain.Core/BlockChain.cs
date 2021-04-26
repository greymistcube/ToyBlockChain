using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToyBlockChain.Util;

namespace ToyBlockChain.Core
{
    public class BlockChain
    {
        public const string SEPARATOR = "<BC>";
        public const int INIT_DIFFICULTY = MIN_DIFFICULTY;
        public const int MIN_DIFFICULTY = 8;
        public const int MAX_DIFFICULTY = 256;
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

        internal void Sync(string serializedString)
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

        /// <summary>
        /// Dumps everything.
        /// </summary>
        internal void Dump()
        {
            _chain = new List<Block>();
        }

        internal void ValidateBlock(Block block)
        {
            Block lastBlock = GetLastBlock();

            if (_chain.Count > block.Index)
            {
                throw new BlockInvalidIgnorableException(
                    "given block index is too low");
            }
            else if (_chain.Count < block.Index)
            {
                throw new BlockInvalidCriticalException(
                    "given block index is too high");
            }
            else if (
                (GetLastBlock() != null)
                && (GetLastBlock().HashString != block.PreviousHashString))
            {
                throw new BlockInvalidIgnorableException(
                    "previous hash for given block does not match " +
                    "hash of the last block in the chain");
            }
            else if (
                (GetLastBlock() != null)
                && !(GetLastBlock().BlockHeader.Timestamp
                    <= block.BlockHeader.Timestamp))
            {
                throw new BlockInvalidIgnorableException(
                    "timestamp for given block is earlier than "
                    + "timestamp for the last block in the chain");
            }
        }

        internal void ValidateTransaction(Transaction transaction)
        {
            return;
        }

        internal void AddBlock(Block block)
        {
            _chain.Add(block);
            AdjustDifficulty();
            Logger.Log(
                $"[Info] Chain: Block {block.LogId} "
                + "added to the chain",
                Logger.INFO, ConsoleColor.Green);
            Logger.Log(
                "[Debug] Chain: block detail:\n"
                + $"{block.ToString()}",
                Logger.DEBUG, ConsoleColor.Red);
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
        /// <see cref="MIN_DIFFICULTY"/> or raised above
        /// <see cref="MAX_DIFFICULTY"/>.
        /// </summary>
        private void AdjustDifficulty()
        {
            if (_chain.Count < MOVING_AVERAGE_LENGTH)
            {
                _difficulty = INIT_DIFFICULTY;
            }
            else
            {
                int startIndex = (
                    ((_chain.Count / MOVING_AVERAGE_LENGTH) - 1)
                    * MOVING_AVERAGE_LENGTH);
                int endIndex = startIndex + (MOVING_AVERAGE_LENGTH - 1);
                long startTimestamp = _chain[startIndex].BlockHeader.Timestamp;
                long endTimestamp = _chain[endIndex].BlockHeader.Timestamp;
                double simpleMovingAverage = ((
                    endTimestamp - startTimestamp)
                    / (MOVING_AVERAGE_LENGTH - 1));
                if (simpleMovingAverage < MINING_INTERVAL_LOWER_LIMIT)
                {
                    _difficulty = Math.Min(MAX_DIFFICULTY, _difficulty + 1);
                }
                else if (MINING_INTERVAL_UPPER_LIMIT < simpleMovingAverage)
                {
                    _difficulty = Math.Max(MIN_DIFFICULTY, _difficulty - 1);
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
