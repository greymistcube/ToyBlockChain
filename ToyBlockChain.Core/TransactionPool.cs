using System;
using System.Collections.Generic;
using System.Linq;

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

    public class TransactionNotInPoolException : Exception
    {
        public TransactionNotInPoolException()
        {
        }

        public TransactionNotInPoolException(string message) : base(message)
        {
        }
    }

    public class TransactionPool
    {
        public const string SEPARATOR = "<TP>";
        private Dictionary<string, Transaction> _pool;

        public TransactionPool()
        {
            _pool = new Dictionary<string, Transaction>();
        }

        public void Sync(string serializedString)
        {
            _pool = new Dictionary<string, Transaction>();
            string[] transactionStrings = serializedString.Split(SEPARATOR);
            foreach (string transactionString in transactionStrings)
            {
                Transaction transaction = new Transaction(transactionString);
                _pool.Add(transaction.HashString, transaction);
            }

        }

        public void AddTransaction(Transaction transaction)
        {
            if (_pool.ContainsKey(transaction.HashString))
            {
                throw new TransactionInPoolException(
                    "transaction already exists in pool: "
                    + $"{transaction.HashString}");
            }
            _pool.Add(transaction.HashString, transaction);
        }

        public void RemoveTransaction(Transaction transaction)
        {
            if (!_pool.ContainsKey(transaction.HashString))
            {
                throw new TransactionNotInPoolException(
                    "transaction not found in pool: "
                    + $"{transaction.HashString}");
            }
            _pool.Remove(transaction.HashString);
        }

        public bool HasTransaction(Transaction transaction)
        {
            return _pool.ContainsKey(transaction.HashString);
        }

        public Dictionary<string, Transaction> Pool
        {
            get
            {
                return _pool;
            }
        }

        public string ToSerializedString()
        {
            return String.Join(
                SEPARATOR,
                _pool.Values.Select(
                    account => account.ToSerializedString()));
        }
    }
}