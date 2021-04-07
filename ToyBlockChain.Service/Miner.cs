using System;
using System.Collections.Generic;
using System.Threading;
using ToyBlockChain.Core;
using ToyBlockChain.Crypto;

namespace ToyBlockChain.Service
{
    public class Miner
    {
        public static int NONCE_LENGTH = 16;
        private readonly Node _node;

        public Miner(Node node)
        {
            _node = node;
        }

        public void Run()
        {
            // TODO: Temporary running script.
            Random rnd = new Random();

            List<Transaction> transactionPool;
            Transaction transaction;
            Block block;

            while (true)
            {
                transactionPool = _node.TransactionPool;

                if (transactionPool.Count > 0)
                {
                    transaction = transactionPool[
                        rnd.Next(transactionPool.Count)];

                    block = Mine(transaction);
                    if (block != null)
                    {
                        _node.AddBlock(block);
                    }
                }

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Continuously makes multiple attempts to mine a block
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
        /// Makes a single attempt to mine a block.
        /// </summary>
        private Block Pick(Transaction transaction)
        {
            Block lastBlock = _node.LastBlock();

            int index;
            string previousHashString;
            string transactionHashString = transaction.HashString;
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
                timestamp, nonce, difficulty);

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
