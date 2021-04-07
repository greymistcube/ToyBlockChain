using System;
using System.Text;

namespace ToyBlockChain.Core
{
    public class Block
    {
        private readonly Transaction _transaction;
        private readonly BlockHeader _blockHeader;

        public Block(BlockHeader blockHeader, Transaction transaction)
        {
            _blockHeader = blockHeader;
            _transaction = transaction;
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
            return String.Format(
                "BLOCK HEADER:\n{0}\n"
                + "TRANSACTION: \n{1}",
                BlockHeader.ToString(), Transaction.ToString());
        }

        public string ToSerializedString()
        {
            return String.Format(
                "{0},{1}",
                BlockHeader.ToSerializedString(),
                Transaction.ToSerializedString());
        }

        public byte[] ToSerializedBytes()
        {
            return Encoding.UTF8.GetBytes(ToSerializedString());
        }

    }
}
