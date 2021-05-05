using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Security.Cryptography;
using ToyBlockChain.Core;
using ToyBlockChain.Crypto;
using ToyBlockChain.Network;

namespace ToyBlockChain.Service
{
    public class MiningFailException : Exception
    {
        public MiningFailException()
        {
        }

        public MiningFailException(string message) : base(message)
        {
        }
    }

    public class Miner
    {
        public static int NONCE_LENGTH = 16;
        private readonly INodeMiner _node;
        private Identity _identity;
        private Action<Payload> Announce;

        /// <summary>
        /// The class representing a miner in a blockchain ecosystem.
        /// Its primary objective is to search and retrieve a
        /// <see cref="Transaction"/> that hasn't been added to the blockchain
        /// and "mine" a valid <see cref="Block"/> containing such
        /// <see cref="Transaction"/>.
        /// </summary>
        public Miner(INodeMiner node, Identity identity, Action<Payload> Func)
        {
            _node = node;
            _identity = identity;
            Announce = Func;
        }

        public void Run()
        {
            Random rnd = new Random();
            Block block = null;

            while (true)
            {
                try
                {
                    block = Mine();
                    lock (_node)
                    {
                        _node.AddBlockToBlockChain(block);
                    }
                    Announce(new Payload(
                        Protocol.ANNOUNCE_BLOCK,
                        block.ToSerializedString()));
                }
                catch (MiningFailException)
                {
                }
                finally
                {
                    Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// Continuously makes multiple attempts to mine a <see cref="Block"/>
        /// until transaction is added to the blockchain.
        /// </summary>
        private Block Mine()
        {
            Transaction transaction = GetTransactionToMine();

            while (true)
            {
                // Check if transaction is still in the pool.
                if (_node.HasTransactionInPool(transaction))
                {
                    try
                    {
                        Block block = Pick(transaction);
                        block.CheckSoundness();
                        return block;
                    }
                    catch (BlockUnsoundException)
                    {
                        continue;
                    }
                }
                else
                {
                    throw new MiningFailException(
                        "transaction can no longer be found in the pool: "
                        + $"{transaction.HashString}");
                }
            }
        }

        private Transaction GetTransactionToMine()
        {
            Dictionary<string, Transaction> transactionPool;
            lock (_node)
            {
                transactionPool = _node.GetTransactionPool();
            }

            if (!(transactionPool.Keys.Count > 0))
            {
                throw new MiningFailException(
                    "transaction pool is empty");
            }
            else
            {
                Random rnd = new Random();
                string key =
                    transactionPool.Keys.ToList()[
                        rnd.Next(transactionPool.Keys.Count)];
                return transactionPool[key];
            }
        }

        /// <summary>
        /// Makes a single attempt to mine a valid <see cref="Block"/>.
        /// </summary>
        private Block Pick(Transaction transaction)
        {
            Block lastBlock = _node.GetLastBlock();

            int index;
            string previousHashString;
            string transactionHashString = transaction.HashString;
            string miner = _identity.Address;
            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            string nonce = CryptoUtil.GenerateNonce();
            int difficulty = _node.GetTargetDifficulty();

            if (lastBlock == null)
            {
                index = 0;
                previousHashString = null;
            }
            else
            {
                index = lastBlock.Index + 1;
                previousHashString = lastBlock.HashString;
            }

            BlockHeader blockHeader = new BlockHeader(
                index, previousHashString, transaction.HashString,
                miner, timestamp, nonce, difficulty);

            return new Block(blockHeader, transaction);
        }
    }
}
