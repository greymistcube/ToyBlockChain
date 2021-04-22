using System;
using System.Text;

namespace ToyBlockChain.Core
{
    public class BlockInvalidException : Exception
    {
        public BlockInvalidException()
        {
        }

        public BlockInvalidException(string message) : base(message)
        {
        }
    }

    public class BlockInvalidInternalException : BlockInvalidException
    {
        public BlockInvalidInternalException()
        {
        }

        public BlockInvalidInternalException(string message) : base(message)
        {
        }
    }

    public class BlockInvalidExternalException : BlockInvalidException
    {
        public BlockInvalidExternalException()
        {
        }

        public BlockInvalidExternalException(string message) : base(message)
        {
        }
    }

    public class BlockInvalidForChainException : BlockInvalidExternalException
    {
        public BlockInvalidForChainException()
        {
        }

        public BlockInvalidForChainException(string message) : base(message)
        {
        }
    }

    public class BlockInvalidForChainIgnorableException
        : BlockInvalidForChainException
    {
        public BlockInvalidForChainIgnorableException()
        {
        }

        public BlockInvalidForChainIgnorableException(string message)
            : base(message)
        {
        }
    }

    public class BlockInvalidForChainCriticalException
        : BlockInvalidForChainException
    {
        public BlockInvalidForChainCriticalException()
        {
        }

        public BlockInvalidForChainCriticalException(string message)
            : base(message)
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
