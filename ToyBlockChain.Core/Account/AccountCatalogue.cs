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

        /// <summary>
        /// Dumps everything.
        /// </summary>
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
            Logger.Log(
                $"[Info] Catalogue: Account {account.LogId} "
                + "added to the catalogue",
                Logger.INFO, ConsoleColor.Green);
            Logger.Log(
                "[Debug] Catalogue: account detail:\n "
                + $"{account.ToString()}",
                Logger.DEBUG, ConsoleColor.Red);
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

        internal void ValidateBlock(Block block)
        {
            return;
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
                if (transaction.Operation.Type
                    != UserTargetedOperation.REGISTER)
                {
                    throw new TransactionInvalidForCatalogueException(
                        "transaction for a non-existant sender account "
                        + "must be a registration transaction");
                }
            }
        }

        /// <summary>
        /// Consumes given transaction.
        /// </summary>
        internal void ConsumeTransaction(Transaction transaction)
        {
            if (!HasAccount(transaction.Sender))
            {
                AddAccount(Account.AccountFactory(
                    transaction.Sender, UserAccount.TYPE));
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