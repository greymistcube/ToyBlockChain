using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ToyBlockChain.Util;

namespace ToyBlockChain.Core
{
    /// <summary>
    /// Thrown if given account is already in the catalogue.
    /// </summary>
    public class AccountInCatalogueException : Exception
    {
        public AccountInCatalogueException()
        {
        }

        public AccountInCatalogueException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Thrown if given account is not found in the catalogue.
    /// </summary>
    public class AccountNotInCatalogueException : Exception
    {
        public AccountNotInCatalogueException()
        {
        }

        public AccountNotInCatalogueException(string message) : base(message)
        {
        }
    }

    public class AccountCatalogue
    {
        public const string SEPARATOR = "<AC>";
        private Dictionary<string, Account> _catalogue;

        public AccountCatalogue()
        {
            _catalogue = new Dictionary<string, Account>();
        }

        internal void Sync(string serializedString)
        {
            _catalogue = new Dictionary<string, Account>();
            if (serializedString != null && serializedString.Length > 0)
            {
                string[] accountStrings = serializedString.Split(SEPARATOR);
                foreach (string accountString in accountStrings)
                {
                    // TODO: Placeholder implementation.
                    Account account = Account.AccountFactory(accountString);
                    _catalogue.Add(account.Address, account);
                }
            }
        }

        internal void Dump()
        {
            _catalogue = new Dictionary<string, Account>();
        }

        public void AddAccount(Account account)
        {
            if (_catalogue.ContainsKey(account.Address))
            {
                throw new ArgumentException(
                    $"account already exists in catalogue: {account.Address}");
            }
            _catalogue.Add(account.Address, account);
        }

        internal bool HasAccount(string address)
        {
            return _catalogue.ContainsKey(address);
        }

        internal Dictionary<string, Account> Catalogue
        {
            get
            {
                return _catalogue;
            }
        }

        internal List<string> Addresses
        {
            get
            {
                return new List<string>(_catalogue.Keys.ToList());
            }
        }

        /// <summary>
        /// Checks if given transaction is valid for consumption.
        /// </summary>
        internal void ValidateTransaction(Transaction transaction)
        {
            if (HasAccount(transaction.Sender))
            {
                if (transaction.Nonce != _catalogue[transaction.Sender].Nonce)
                {
                    throw new TransactionInvalidForCatalogueException(
                        "transaction nonce does not match account nonce");
                }
                else if (!HasAccount(transaction.Recipient))
                {
                    throw new TransactionInvalidForCatalogueException(
                        "recipient account not found in the catalogue");
                }
            }
            else
            {
                if (!(transaction.Operation is OperationOnUserRegister))
                {
                    throw new TransactionInvalidForCatalogueException(
                        "transaction for a non-existant sender account "
                        + "must be a registration transaction");
                }
            }
        }

        internal void ValidateBlock(Block block)
        {
            // Assuming block contains only transactions in the pool,
            // there is nothing to validate.
            return;
        }

        /// <summary>
        /// Consumes given transaction.
        /// </summary>
        internal void ConsumeTransaction(Transaction transaction)
        {
            if (!HasAccount(transaction.Sender))
            {
                AddAccount(Account.AccountFactory(
                    transaction.Sender, UserAccount.TYPE,
                    UserAccount.INIT_STATE));
            }
            Account senderAccount = _catalogue[transaction.Sender];
            Account recipientAccount = _catalogue[transaction.Recipient];
            senderAccount.ConsumeTransactionAsSender(transaction);
            recipientAccount.ConsumeTransactionAsRecipient(transaction);

            Logger.Log(
                $"[Info] Catalogue: Sender account {senderAccount.LogId} "
                + $"and recipient account {recipientAccount.LogId} "
                + $"consumed Transaction {transaction.LogId}",
                Logger.INFO, ConsoleColor.Green);
            Logger.Log(
                "[Debug] Catalogue: sender account detail:\n"
                + $"{senderAccount.ToString()}",
                Logger.DEBUG, ConsoleColor.Red);
            Logger.Log(
                "[Debug] Catalogue: recipient account detail:\n"
                + $"{recipientAccount.ToString()}",
                Logger.DEBUG, ConsoleColor.Red);
        }

        public string ToSerializedString()
        {
            return String.Join(
                SEPARATOR,
                _catalogue.Values.Select(
                    account => account.ToSerializedString()));
        }

        public byte[] ToSerializedBytes()
        {
            return Encoding.UTF8.GetBytes(ToSerializedString());
        }
    }
}