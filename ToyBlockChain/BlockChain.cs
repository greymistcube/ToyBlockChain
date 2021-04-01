using System;
using System.Collections.Generic;

namespace ToyBlockChain
{
    public class BlockChain
    {
        private List<Block> _chain;

        public BlockChain()
        {
            _chain = new List<Block>();
            return;
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

        public bool AddBlock(Block block)
        {
            if (ValidateBlock(block))
            {
                _chain.Add(block);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ValidateBlock(Block block)
        {
            return (
                ValidateBlockHeader(block.BlockHeader)
                && ValidateTransaction(block.Transaction));
        }

        private bool ValidateBlockHeader(BlockHeader blockHeader)
        {
            return blockHeader.IsValid();
        }

        private bool ValidateTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public int TargetDifficulty()
        {
            return _chain.Count;
        }
    }
}
