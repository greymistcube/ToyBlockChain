using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ToyBlockChain.Core
{
    public class AccountCatalogue
    {
        public const string SEPARATOR = "<AT>";
        private Dictionary<string, Account> _catalogue;

        public AccountCatalogue()
        {
            _catalogue = new Dictionary<string, Account>();
        }

        public void Sync(string serializedString)
        {
            _catalogue = new Dictionary<string, Account>();
            string[] accountStrings = serializedString.Split(SEPARATOR);
            foreach (string accountString in accountStrings)
            {
                Account account = new Account(accountString);
                _catalogue.Add(account.Address, account);
            }
        }

        public void AddAccount(Account account)
        {
            if (_catalogue.ContainsKey(account.Address))
            {
                throw new ArgumentException(
                    $"account already exists: {account.Address}");
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

        public Dictionary<string, Account> Table
        {
            get
            {
                return _catalogue;
            }
        }
    }
}