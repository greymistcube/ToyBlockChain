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
                catch (TransactionInvalidForPoolException)
                {
                }
                catch (TransactionInvalidForCatalogueException)
                {
                }
                catch (TransactionInvalidForChainException)
                {
                }
                finally
                {
                    Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// Creates a random transaction.
        /// </summary>
        private Transaction CreateTransaction()
        {
            Dictionary<string, Account> accountCatalogue;
            lock (_node)
            {
                accountCatalogue = _node.GetAccountCatalogue();
            }

            // Create an unsigned transaction.
            Transaction transaction;
            if (!accountCatalogue.ContainsKey(_identity.Address))
            {
                transaction = CreateRegisterTransaction();
            }
            else
            {
                transaction = CreateMessageTransaction(accountCatalogue);
            }

            // Create a valid signature and sign the transaction.
            string signature = CryptoUtil.Sign(
                transaction.SignatureInputString(), _identity.RSAParameters);
            transaction.Sign(signature);

            return transaction;
        }

        private Transaction CreateRegisterTransaction()
        {
            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            Operation operation = Operation.OperationFactory(
                OperationOnUser.TARGET, OperationOnUserRegister.MOVE, "");
            Transaction transaction = new Transaction(
                _identity.Address, 1, operation, _identity.Address,
                timestamp, _identity.PublicKey, null);
            return transaction;
        }

        private Transaction CreateMessageTransaction(
            Dictionary<string, Account> accountCatalogue)
        {
            int count = accountCatalogue[_identity.Address].Count;
            string message = GetRandomMessage();
            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            string recipient = GetRandomUserRecipient(accountCatalogue);

            Operation operation = Operation.OperationFactory(
                OperationOnUser.TARGET, OperationOnUserMessage.MOVE, message);
            Transaction transaction = new Transaction(
                _identity.Address, count + 1, operation, recipient,
                timestamp, _identity.PublicKey, null);
            return transaction;
        }

        private Transaction CreateGameTransaction()
        {
            throw new NotImplementedException();
        }

        private string GetRandomUserRecipient(
            Dictionary<string, Account> accountCatalogue)
        {
            Random random = new Random();
            List<Account> accounts = accountCatalogue.Values.ToList().FindAll(
                account => account.Type == UserAccount.TYPE);
            string recipient = accounts[random.Next(accounts.Count)].Address;

            return recipient;
        }

        private string GetRandomMessage()
        {
            Random random = new Random();
            string message = random.Next().ToString();

            return message;
        }

        private string GetRandomContractRecipient(
            Dictionary<string, Account> accountCatalogue)
        {
            throw new NotImplementedException();
        }
    }
}
