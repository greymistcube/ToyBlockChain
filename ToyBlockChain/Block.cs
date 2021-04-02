using System;
using System.Text;

namespace ToyBlockChain
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

        public Transaction Transaction { get; }
        public BlockHeader BlockHeader { get; }

        public bool IsValid()
        {
            return _blockHeader.TransactionHashString == _transaction.HashString();
        }

        public byte[] Serialize()
        {
            return Encoding.UTF8.GetBytes(ToString());
        }

        public override string ToString()
        {
            return String.Format(
                "{0},{1}",
                BlockHeader.ToString(),
                Transaction.ToString());
        }
    }
}
