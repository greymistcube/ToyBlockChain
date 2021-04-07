using System.Collections.Generic;

namespace ToyBlockChain.Core
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
                block.IsValid()
                && block.Index == _chain.Count
            ) && (
                LastBlock() == null
                || LastBlock().HashString == block.HashString);
        }

        public int TargetDifficulty()
        {
            return _chain.Count;
        }
    }
}
