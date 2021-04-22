using System;
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

    public class Block
    {
        public const string SEPARATOR = "<B>";
        private readonly Transaction _transaction;
        private readonly BlockHeader _blockHeader;

        public Block(BlockHeader blockHeader, Transaction transaction)
        {
            _blockHeader = blockHeader;
            _transaction = transaction;
        }

        public Block(string serializedString)
        {
            string[] substrings = serializedString.Split(SEPARATOR);
            _blockHeader = new BlockHeader(substrings[0]);
            _transaction = new Transaction(substrings[1]);
        }

        public int Index
        {
            get
            {
                return BlockHeader.Index;
            }
        }

        public byte[] HashBytes
        {
            get
            {
                return BlockHeader.HashBytes;
            }
        }

        public string HashString
        {
            get
            {
                return BlockHeader.HashString;
            }
        }

        public string LogId
        {
            get
            {
                return BlockHeader.LogId;
            }
        }

        public string PreviousHashString
        {
            get
            {
                return BlockHeader.PreviousHashString;
            }
        }

        public BlockHeader BlockHeader
        {
            get
            {
                return _blockHeader;
            }
        }

        public Transaction Transaction
        {
            get
            {
                return _transaction;
            }
        }

        public bool IsValid()
        {
            return (
                _blockHeader.TransactionHashString == _transaction.HashString
                && BlockHeader.IsValid()
                && Transaction.IsValid());
        }

        public override string ToString()
        {
            return (
                $"BLOCK HEADER:\n{BlockHeader}".Replace("\n", "\n\t")
                + "\n"
                + $"TRANSACTION:\n{Transaction}".Replace("\n", "\n\t"));
        }

        public string ToSerializedString()
        {
            return String.Join(
                SEPARATOR,
                new string[] {
                    BlockHeader.ToSerializedString(),
                    Transaction.ToSerializedString() });
        }

        public byte[] ToSerializedBytes()
        {
            return Encoding.UTF8.GetBytes(ToSerializedString());
        }
    }
}
