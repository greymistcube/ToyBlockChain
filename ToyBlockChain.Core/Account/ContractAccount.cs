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

        public ContractAccount(string address, string type)
            : base(address, type)
        {
        }

        public static Account ContractAccountFactory(
            string address, string type)
        {
            // Note: If statement is used since ADDRESS is derived on runtime.
            if (address == RockPaperScissorsContractAccount.ADDRESS)
            {
                return new RockPaperScissorsContractAccount(
                    address, type);
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
        public static readonly string INIT_STATE = "";

        public RockPaperScissorsContractAccount(string address, string type)
            : base(address, type)
        {
            _state = INIT_STATE;
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