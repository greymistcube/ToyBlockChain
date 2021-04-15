using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using ToyBlockChain.Core;
using ToyBlockChain.Crypto;

namespace ToyBlockChain.Service
{
    public class Client
    {
        private Node _node;
        private Identity _identity;

        public Client(Node node, Identity identity)
        {
            _node = node;
            _identity = identity;
        }

        public void Run()
        {
            // TODO: Temporary running script.
            lock (_node)
            {
                _node.RegisterAddress(_address);
            }

            Random rnd = new Random();

            double value;
            string recipient;
            Transaction transaction = null;

            while(true)
            {
                List<string> accounts = _node.Accounts;

                value = rnd.NextDouble();
                recipient = accounts[rnd.Next(accounts.Count)];
                if (transaction == null
                    || !_node.HasTransactionInPool(transaction))
                {
                    transaction = CreateTransaction(value, recipient);
                    lock (_node)
                    {
                        _node.RegisterTransaction(transaction);
                    }
                }
            }
        }

        public Transaction CreateTransaction(double value, string recipient)
        {
            // Create an unsigned, invalid transaction.
            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            Transaction transaction = new Transaction(
                _address, value, recipient, timestamp, PublicKey);

            // Create a valid signature and sign the transaction.
            string signature = CryptoUtil.Sign(
                transaction.SignatureInputString(), _rsaParameters);
            transaction.Sign(signature);

            return transaction;
        }

        public string Address
        {
            get
            {
                return _address;
            }
        }

        public string PublicKey
        {
            get
            {
                return _publicKey;
            }
        }

        public override string ToString()
        {
            return String.Format(
                "Address: {0}\n"
                + "Public Key: {1}",
                Address, PublicKey);
        }
    }
}
