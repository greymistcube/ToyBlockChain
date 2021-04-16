using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using ToyBlockChain.Core;
using ToyBlockChain.Crypto;
using ToyBlockChain.Network;

namespace ToyBlockChain.Service
{
    public class Client
    {
        private INodeClient _node;
        private Identity _identity;

        public delegate void AnnounceDelegate(Payload payload);
        private AnnounceDelegate Announce;

        public Client(
            INodeClient node, Identity identity, AnnounceDelegate Func)
        {
            _node = node;
            _identity = identity;
            Announce = Func;
        }

        public void Run()
        {
            while(true)
            {
                Random rnd = new Random();

                string value;
                string recipient;
                Dictionary<string, Account> addressCatalogue;
                Transaction transaction = null;

                lock (_node)
                {
                    addressCatalogue = _node.GetAccountCatalogue();
                }

                value = rnd.Next().ToString();
                recipient = addressCatalogue.Keys.ToList()[
                    rnd.Next(addressCatalogue.Count)];
                if (transaction == null
                    || !_node.HasTransactionInPool(transaction))
                {
                    transaction = CreateTransaction(value, recipient);
                    lock (_node)
                    {
                        _node.AddTransactionToPool(transaction);
                    }
                    Announce(new Payload(
                        Protocol.ANNOUNCE_TRANSACTION,
                        transaction.ToSerializedString()));
                }
            }
        }

        private Transaction CreateTransaction(string value, string recipient)
        {
            // TODO: remove dummy nonce.
            // Create an unsigned, invalid transaction.
            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            Transaction transaction = new Transaction(
                _identity.Address, 0, value, recipient, timestamp,
                _identity.PublicKey);

            // Create a valid signature and sign the transaction.
            string signature = CryptoUtil.Sign(
                transaction.SignatureInputString(), _identity.RSAParameters);
            transaction.Sign(signature);

            return transaction;
        }
    }
}
