using System;
using ToyBlockChain.Crypto;

namespace ToyBlockChain.Core
{
    public abstract class AccountContract : Account
    {
        public const string TYPE = "contract";

        public AccountContract(string address, string type, string state)
            : base(address, type, state)
        {
        }

        public AccountContract(string serializedString) : base(serializedString)
        {
        }

        public static Account AccountContractFactory(
            string address, string type, string state)
        {
            // Note: If statement is used since ADDRESS is derived on runtime.
            if (address == AccountContractRockPaperScissors.ADDRESS)
            {
                return new AccountContractRockPaperScissors(
                    address, type, state);
            }
            else
            {
                throw new ArgumentException(
                    $"unkown contract address: {address}");
            }
        }
    }

    public class AccountContractRockPaperScissors : AccountContract
    {
        public static readonly string NAME = "RockPaperScissors";
        public static readonly string ADDRESS = CryptoUtil
            .ComputeHashString(NAME);

        public AccountContractRockPaperScissors(
            string address, string type, string state)
            : base(address, type, state)
        {
        }

        public AccountContractRockPaperScissors(string serializedString)
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