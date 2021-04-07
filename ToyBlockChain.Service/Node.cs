using System;
using System.Collections.Generic;
using ToyBlockChain.Core;

namespace ToyBlockChain.Service
{
    public class Node
    {
        private BlockChain _chain;
        private HashSet<string> _addresses;
        private TransactionPool _pool;

        public Node()
        {
            _chain = new BlockChain();
            _addresses = new HashSet<string>();
            _pool = new TransactionPool();
        }

        public void RegisterAddress(string address)
        {
            if (_addresses.Contains(address))
            {
                throw new ArgumentException();
            }
            else
            {
                _addresses.Add(address);
            }
            return;
        }
    }
}
