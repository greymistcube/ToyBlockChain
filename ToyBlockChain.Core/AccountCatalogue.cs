using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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

        public void Sync(string serializedString)
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

        public void AddAccount(Account account)
        {
            if (_catalogue.ContainsKey(account.Address))
            {
                throw new AccountInCatalogueException(
                    $"account already exists in catalogue: {account.Address}");
            }
            _catalogue.Add(account.Address, account);
        }

        internal bool HasAccount(Account account)
        {
            return _catalogue.ContainsKey(account.Address);
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
            if (!HasAccount(transaction.Sender)
                || !(HasAccount(transaction.Recipient)))
            {
                throw new TransactionInvalidForCatalogueException(
                    "one of the accounts in transaction "
                    + "is not found in the catalogue");
            }
            else if ((_catalogue[transaction.Sender].Count + 1)
                != transaction.Count)
            {
                throw new TransactionInvalidForCatalogueException(
                    "transaction count is invalid");
            }
        }

        internal void ValidateBlock(Block block)
        {
            // Assuming block contains only transactions in the pool,
            // there is nothing to validate.
            return;
        }

        internal void ConsumeTransaction(Transaction transaction)
        {
            _catalogue[transaction.Sender].ConsumeTransactionAsSender(transaction);
            _catalogue[transaction.Recipient].ConsumeTransactionAsRecipient(transaction);
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