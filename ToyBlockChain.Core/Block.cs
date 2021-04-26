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

    public class BlockUnsoundException : Exception
    {
        public BlockUnsoundException()
        {
        }

        public BlockUnsoundException(string message) : base(message)
        {
        }
    }

    public class BlockInvalidIgnorableException : BlockInvalidException
    {
        public BlockInvalidIgnorableException()
        {
        }

        public BlockInvalidIgnorableException(string message)
            : base(message)
        {
        }
    }

    public class BlockInvalidCriticalException : BlockInvalidException
    {
        public BlockInvalidCriticalException()
        {
        }

        public BlockInvalidCriticalException(string message)
            : base(message)
        {
        }
    }

    public class Block
    {
        public const string SEPARATOR = "<BK>";
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

        public void CheckSoundness()
        {
            if (_blockHeader.TransactionHashString != _transaction.HashString)
            {
                throw new BlockUnsoundException(
                    "block header transaction hash does not match "
                    + "the hash of the transaction");
            }
            try
            {
                _blockHeader.CheckSoundness();
                _transaction.CheckSoundness();
            }
            catch (Exception ex) when (
                ex is BlockHeaderUnsoundException
                || ex is TransactionUnsoundException)
            {
                throw new BlockUnsoundException(ex.Message);
            }
        }

        public override string ToString()
        {
            return (
                $"BLOCKHEADER:\n{BlockHeader}\n"
                + $"TRANSACTION:\n{Transaction}");
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
