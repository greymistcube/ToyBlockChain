using System;
using System.Collections.Generic;
using System.Linq;
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
            Random rnd = new Random();

            double value;
            string recipient;
            Transaction transaction = null;
            List<string> addresses;

            while(true)
            {
                lock (_node)
                {
                    addresses = _node.AccountCatalogue.Addresses;
                }

                value = rnd.NextDouble();
                recipient = addresses[rnd.Next(addresses.Count)];
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
                _identity.Address, value, recipient, timestamp,
                _identity.PublicKey);

            // Create a valid signature and sign the transaction.
            string signature = CryptoUtil.Sign(
                transaction.SignatureInputString(), _identity.RSAParameters);
            transaction.Sign(signature);

            return transaction;
        }
    }
}
