using System;
using ToyBlockChain.Core;

namespace ToyBlockChain.Service
{
    public class Miner
    {
        private readonly Node _node;

        public Miner(Node node)
        {
            _node = node;
        }

        public void Run()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Continuously makes multiple attempts to mine a block
        /// until transaction is added to the blockchain.
        /// </summary>
        private Block Mine(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Makes a single attempt to mine a block.
        /// </summary>
        private Block Pick(Transaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
