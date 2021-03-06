using System;
using System.Collections.Generic;
using System.Linq;
using ToyBlockChain.Util;

namespace ToyBlockChain.Core
{
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

        /// <summary>
        /// Dumps everything.
        /// </summary>
        internal void Dump()
        {
            _poolByHash = new Dictionary<string, Transaction>();
            _poolBySender = new Dictionary<string, Transaction>();
        }

        internal void ValidateBlock(Block block)
        {
            if (!_poolByHash.ContainsKey(block.Transaction.HashString))
            {
                throw new BlockInvalidException(
                    "block transaction not found in pool: "
                    + $"{block.Transaction.HashString}");
            }
        }

        internal void ValidateTransaction(Transaction transaction)
        {
            if (HasTransaction(transaction))
            {
                throw new TransactionInvalidForPoolException(
                    "transaction already exists in pool: "
                    + $"{transaction.HashString}");
            }
            else if (HasSender(transaction))
            {
                throw new TransactionInvalidForPoolException(
                    "transaction with the same sender already exists in pool: "
                    + $"{transaction.HashString}");
            }
        }

        public void AddTransaction(Transaction transaction)
        {
            _poolByHash.Add(transaction.HashString, transaction);
            _poolBySender.Add(transaction.Sender, transaction);
            Logger.Log(
                $"[Info] Pool: Transaction {transaction.LogId} "
                + "added to the pool",
                Logger.INFO, ConsoleColor.Green);
            Logger.Log(
                "[Debug] Pool: transaction detail:\n"
                + $"{transaction.ToString()}",
                Logger.DEBUG, ConsoleColor.Red);
        }

        public void RemoveTransaction(Transaction transaction)
        {
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