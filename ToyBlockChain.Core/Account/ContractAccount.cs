using System;
using ToyBlockChain.Crypto;

namespace ToyBlockChain.Core
{
    /// <summary>
    /// Represents an account of a contract.
    /// </summary>
    public abstract class ContractAccount : Account
    {
        public const string TYPE = "contract";

        public ContractAccount(string address, string type, string state)
            : base(address, type, state)
        {
        }

        public ContractAccount(string serializedString) : base(serializedString)
        {
        }

        public static Account ContractAccountFactory(
            string address, string type, string state)
        {
            // Note: If statement is used since ADDRESS is derived on runtime.
            if (address == RockPaperScissorsContractAccount.ADDRESS)
            {
                return new RockPaperScissorsContractAccount(
                    address, type, state);
            }
            else
            {
                throw new ArgumentException(
                    $"unkown contract address: {address}");
            }
        }
    }

    public class RockPaperScissorsContractAccount : ContractAccount
    {
        public static readonly string NAME = "RockPaperScissors";
        public static readonly string ADDRESS = CryptoUtil
            .ComputeHashString(NAME);

        public RockPaperScissorsContractAccount(
            string address, string type, string state)
            : base(address, type, state)
        {
        }

        public RockPaperScissorsContractAccount(string serializedString)
            : base(serializedString)
        {
        }

        internal override void ConsumeTransactionAsSender(
            Transaction transaction)
        {
            throw new MethodAccessException(
                "contract cannot consume transaction as a sender");
        }

        internal override void ConsumeTransactionAsRecipient(
            Transaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}