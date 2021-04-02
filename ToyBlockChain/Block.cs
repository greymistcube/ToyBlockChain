using System;
using System.Text;

namespace ToyBlockChain
{
    public class Block
    {
        public Transaction Transaction;
        public BlockHeader BlockHeader;

        public Block(BlockHeader blockHeader, Transaction transaction)
        {
            BlockHeader = blockHeader;
            Transaction = transaction;
        }

        public int Index
        {
            get
            {
                return BlockHeader.Index;
            }
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
