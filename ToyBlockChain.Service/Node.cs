using System;

namespace ToyBlockChain.Service
{
    public class Node
    {
        private BlockChain _chain;
        private TransactionPool _pool;

        public Node()
        {
            _chain = new BlockChain();
            _pool = new TransactionPool();
        }
    }
}
