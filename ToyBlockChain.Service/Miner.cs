using System;
using System.Collections.Generic;
using System.Threading;
using System.Security.Cryptography;
using ToyBlockChain.Core;
using ToyBlockChain.Crypto;

namespace ToyBlockChain.Service
{
    public class Miner
    {
        public static int NONCE_LENGTH = 16;
        private readonly Node _node;
        private Identity _identity;

        /// <summary>
        /// The class representing a miner in a blockchain ecosystem.
        /// Its primary objective is to search and retrieve a
        /// <see cref="Transaction"/> that hasn't been added to the blockchain
        /// and "mine" a valid <see cref="Block"/> containing such
        /// <see cref="Transaction"/>.
        /// </summary>
        public Miner(Node node, Identity identity)
        {
            _node = node;
            _identity = identity;
        }

        public void Run()
        {
            // TODO: Temporary running script.
            Random rnd = new Random();

            TransactionPool transactionPool;
            Transaction transaction;
            Block block;

            while (true)
            {
                // I have no idea why this fixes the issue of getting
                // a null reference few lines below.
                lock (_node)
                {
                    transactionPool = _node.TransactionPool;
                }

                if (transactionPool.Count > 0)
                {
                    transaction = transactionPool[
                        rnd.Next(transactionPool.Count)];

                    block = Mine(transaction);
                    if (block != null)
                    {
                        lock (_node)
                        {
                            _node.AddBlock(block);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Continuously makes multiple attempts to mine a <see cref="Block"/>
        /// until transaction is added to the blockchain.
        /// </summary>
        private Block Mine(Transaction transaction)
        {
            while (true)
            {
                // Check if transaction is still in the pool.
                if (_node.HasTransactionInPool(transaction))
                {
                    Block block = Pick(transaction);
                    if (block != null)
                    {
                        return block;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Makes a single attempt to mine a valid <see cref="Block"/>.
        /// </summary>
        private Block Pick(Transaction transaction)
        {
            Block lastBlock = _node.LastBlock();

            int index;
            string previousHashString;
            string transactionHashString = transaction.HashString;
            string miner = _identity.Address;
            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            string nonce = CryptoUtil.GenerateNonce();
            int difficulty = _node.TargetDifficulty();

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

            if (blockHeader.IsValid())
            {
                return new Block(blockHeader, transaction);
            }
            else
            {
                return null;
            }
        }
    }
}
