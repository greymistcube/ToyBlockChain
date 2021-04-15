using System;
using System.Collections.Generic;

namespace ToyBlockChain.Core
{
    public class TransactionInPoolException : Exception
    {
        public TransactionInPoolException()
        {
        }

        public TransactionInPoolException(string message) : base(message)
        {
        }
    }

    public class TransactionPool
    {
        public const string SEPARATOR = "<TP>";
        private Dictionary<string, string> _pool;

        public TransactionPool()
        {

        }

        public TransactionPool(string serializedString)
        {

        }

        public Dictionary<string, string> Pool
        {
            get
            {
                return _pool;
            }
        }
    }
}