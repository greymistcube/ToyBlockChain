using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ToyBlockChain.Core
{
    public class AccountTable
    {
        public const string SEPARATOR = "<AT>";
        private Dictionary<string, Account> _accounts;

        public AccountTable()
        {
            _accounts = new Dictionary<string, Account>();
        }

        public void Sync(string serializedString)
        {
            _accounts = new Dictionary<string, Account>();
            string[] accountStrings = serializedString.Split(SEPARATOR);
            foreach (string accountString in accountStrings)
            {
                Account account = new Account(accountString);
                _accounts.Add(account.Address, account);
            }
        }

        public void AddAccount(Account account)
        {
            _accounts.Add(account.Address, account);
        }

        public string ToSerializedString()
        {
            return String.Join(
                SEPARATOR,
                _accounts.Values.Select(
                    account => account.ToSerializedString()));
        }

        public byte[] ToSerializedBytes()
        {
            return Encoding.UTF8.GetBytes(ToSerializedString());
        }
    }
}