using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ToyBlockChain.Core
{
    /// <summary>
    /// Thrown if the account is already in the catalogue
    /// when trying to add an account.
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
    /// Thrown if at least one of the accounts associated with a transaction
    /// is not found in the catalogue when processing a transaction.
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
                    Account account = new Account(accountString);
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

        public bool HasAccount(Account account)
        {
            return _catalogue.ContainsKey(account.Address);
        }

        public bool HasAccount(string address)
        {
            return _catalogue.ContainsKey(address);
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

        public Dictionary<string, Account> Catalogue
        {
            get
            {
                return _catalogue;
            }
        }

        public List<string> Addresses
        {
            get
            {
                return new List<string>(_catalogue.Keys.ToList());
            }
        }
    }
}