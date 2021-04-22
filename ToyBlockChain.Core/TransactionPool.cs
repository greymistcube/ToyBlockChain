using System;
using System.Collections.Generic;
using System.Linq;

namespace ToyBlockChain.Core
{
    /// <summary>
    /// Thrown if the transaction is already in the pool
    /// when trying to add a transaction.
    /// </summary>
    public class TransactionInPoolException : Exception
    {
        public TransactionInPoolException()
        {
        }

        public TransactionInPoolException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Thrown if the transaction is not in the pool.
    /// </summary>
    public class TransactionNotInPoolException : Exception
    {
        public TransactionNotInPoolException()
        {
        }

        public TransactionNotInPoolException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Thrown if there is a transaction with the same sender in the pool
    /// when trying to add a transaction.
    /// </summary>
    public class TransactionSenderInPoolException : Exception
    {
        public TransactionSenderInPoolException()
        {
        }

        public TransactionSenderInPoolException(string message) : base(message)
        {
        }
    }

    public class TransactionPool
    {
        public const string SEPARATOR = "<TP>";
        private Dictionary<string, Transaction> _poolByHash;
        private Dictionary<string, Transaction> _poolBySender;

        public TransactionPool()
        {
            _poolByHash = new Dictionary<string, Transaction>();
            _poolBySender = new Dictionary<string, Transaction>();
        }

        public void Sync(string serializedString)
        {
            _poolByHash = new Dictionary<string, Transaction>();
            if (serializedString != null && serializedString.Length > 0)
            {
                string[] transactionStrings = serializedString.Split(SEPARATOR);
                foreach (string transactionString in transactionStrings)
                {
                    Transaction transaction = new Transaction(
                        transactionString);
                    _poolByHash.Add(transaction.HashString, transaction);
                    _poolBySender.Add(transaction.Sender, transaction);
                }
            }
        }

        public void AddTransaction(Transaction transaction)
        {
            if (HasTransaction(transaction))
            {
                throw new TransactionInPoolException(
                    "transaction already exists in pool: "
                    + $"{transaction.HashString}");
            }
            else if (HasSender(transaction))
            {
                throw new TransactionSenderInPoolException(
                    "transaction with the same sender already exists in pool: "
                    + $"{transaction.HashString}");
            }
            else
            {
                _poolByHash.Add(transaction.HashString, transaction);
                _poolBySender.Add(transaction.Sender, transaction);
            }
        }

        public void RemoveTransaction(Transaction transaction)
        {
            if (!_poolByHash.ContainsKey(transaction.HashString))
            {
                throw new TransactionNotInPoolException(
                    "transaction not found in pool: "
                    + $"{transaction.HashString}");
            }
            _poolByHash.Remove(transaction.HashString);
            _poolBySender.Remove(transaction.Sender);
        }

        internal bool HasTransaction(Transaction transaction)
        {
            return _poolByHash.ContainsKey(transaction.HashString);
        }

        internal bool HasTransaction(string transactionHashString)
        {
            return _poolByHash.ContainsKey(transactionHashString);
        }

        internal bool HasSender(Transaction transaction)
        {
            return _poolBySender.ContainsKey(transaction.Sender);
        }

        internal bool HasSender(string senderAddress)
        {
            return _poolBySender.ContainsKey(senderAddress);
        }

        internal Dictionary<string, Transaction> Pool
        {
            get
            {
                return _poolByHash;
            }
        }

        public string ToSerializedString()
        {
            return String.Join(
                SEPARATOR,
                _poolByHash.Values.Select(
                    account => account.ToSerializedString()));
        }
    }
}