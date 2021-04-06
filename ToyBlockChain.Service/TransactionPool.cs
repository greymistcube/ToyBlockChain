using System;
using System.Collections.Generic;

namespace ToyBlockChain.Service
{
    public class TransactionPool
    {
        private Dictionary<string, Transaction> _pool;

        public TransactionPool()
        {
            _pool = new Dictionary<string, Transaction>();
        }

        public void AddTransaction(Transaction transaction)
        {
            _pool.Add(transaction.HashString, transaction);
        }

        public void RemoveTransaction(Transaction transaction)
        {
            _pool.Remove(transaction.HashString);
        }

        public bool Contains(Transaction transaction)
        {
            return _pool.ContainsKey(transaction.HashString);
        }
    }
}