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
                Transaction transaction = CreateTransaction();
                try
                {
                    lock (_node)
                    {
                        _node.AddTransactionToPool(transaction);
                    }
                    Announce(new Payload(
                        Protocol.ANNOUNCE_TRANSACTION,
                        transaction.ToSerializedString()));
                }
                catch (TransactionSenderInPoolException)
                {
                    continue;
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Creates a random transaction.
        /// </summary>
        private Transaction CreateTransaction()
        {
            Random rnd = new Random();

            Dictionary<string, Account> addressCatalogue;
            lock (_node)
            {
                addressCatalogue = _node.GetAccountCatalogue();
            }

            Account account = addressCatalogue[_identity.Address];

            // TODO: Random value and recipient selection as a placeholder.
            string value = rnd.Next().ToString();
            string recipient = addressCatalogue.Keys.ToList()[
                    rnd.Next(addressCatalogue.Count)];

            // Create an unsigned transaction.
            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            Transaction transaction = new Transaction(
                _identity.Address, account.Count + 1, value, recipient,
                timestamp, _identity.PublicKey);

            // Create a valid signature and sign the transaction.
            string signature = CryptoUtil.Sign(
                transaction.SignatureInputString(), _identity.RSAParameters);
            transaction.Sign(signature);

            return transaction;
        }
    }
}
