using System;

namespace ToyBlockChain
{
    public class Block
    {
        public Transaction Transaction;
        public BlockHeader BlockHeader;

        public Block(Transaction transaction, BlockHeader blockHeader)
        {
            Transaction = transaction;
            BlockHeader = blockHeader;
        }

        public int Index
        {
            get
            {
                return BlockHeader.Index;
            }
        }
    }
}