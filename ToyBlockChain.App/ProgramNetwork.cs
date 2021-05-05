using System;
using System.Net.Sockets;
using System.Linq;
using ToyBlockChain.Core;
using ToyBlockChain.Network;
using ToyBlockChain.Util;

namespace ToyBlockChain.App
{
    public partial class Program
    {
        /// <summary>
        /// Processes an incoming payload with a request header.
        /// </summary>
        private static void ProcessRequestPayload(
            NetworkStream stream, Payload inboundPayload)
        {
            string header = inboundPayload.Header;
            if (header == Protocol.REQUEST_ROUTING_TABLE)
            {
                Payload outboundPayload = new Payload(
                    Protocol.RESPONSE_ROUTING_TABLE,
                    _routingTable.ToSerializedString());
                StreamHandler.WritePayload(stream, outboundPayload);
            }
            else if (header == Protocol.REQUEST_BLOCKCHAIN)
            {
                lock (_node)
                {
                    Payload outboundPayload = new Payload(
                        Protocol.RESPONSE_BLOCKCHAIN,
                        _node.GetBlockChainSerializedString());
                    StreamHandler.WritePayload(stream, outboundPayload);
                }
            }
            else if (header == Protocol.REQUEST_TRANSACTION_POOL)
            {
                lock (_node)
                {
                    Payload outboundPayload = new Payload(
                        Protocol.RESPONSE_TRANSACTION_POOL,
                        _node.GetTransactionPoolSerializedString());
                    StreamHandler.WritePayload(stream, outboundPayload);
                }
            }
            else
            {
                throw new ArgumentException(
                    $"invalid protocol header: {header}");
            }
        }

        /// <summary>
        /// Processes an incoming payload with an announce header.
        /// </summary>
        private static void ProcessAnnouncePayload(Payload inboundPayload)
        {
            string header = inboundPayload.Header;
            if (header == Protocol.ANNOUNCE_ADDRESS)
            {
                Address address = new Address(inboundPayload.Body);
                _routingTable.AddAddress(address);
                Logger.Log(
                    $"[Info] App: Address {address.PortNumber} "
                    + "added to routing table",
                    Logger.INFO, ConsoleColor.Blue);
            }
            else if (header == Protocol.ANNOUNCE_TRANSACTION)
            {
                Transaction transaction = new Transaction(
                    inboundPayload.Body);
                try
                {
                    lock (_node)
                    {
                        _node.AddTransactionToPool(transaction);
                    }
                    Announce(inboundPayload);
                }
                catch (TransactionInvalidException ex)
                {
                    Logger.Log(
                        $"[Info] App: Transaction {transaction.LogId} ignored",
                        Logger.INFO, ConsoleColor.Blue);
                    Logger.Log(
                        $"[Debug] App: {ex.Message}",
                        Logger.DEBUG, ConsoleColor.Red);
                }
            }
            else if (header == Protocol.ANNOUNCE_BLOCK)
            {
                Block block = new Block(inboundPayload.Body);
                try
                {
                    lock (_node)
                    {
                        _node.AddBlockToChain(block);
                    }
                    Announce(inboundPayload);
                }
                catch (TransactionInvalidException ex)
                {
                    Logger.Log(
                        $"[Info] App: Block {block.LogId} ignored",
                        Logger.INFO, ConsoleColor.Blue);
                    Logger.Log(
                        $"[Debug] App: {ex.Message}",
                        Logger.DEBUG, ConsoleColor.Red);
                }
                catch (BlockInvalidIgnorableException ex)
                {
                    Logger.Log(
                        $"[Info] App: Block {block.LogId} ignored",
                        Logger.INFO, ConsoleColor.Blue);
                    Logger.Log(
                        $"[Debug] App: {ex.Message}",
                        Logger.DEBUG, ConsoleColor.Red);
                }
                catch (BlockInvalidCriticalException)
                {
                    Logger.Log(
                        $"[Info] App: Chain falling behind, "
                        + "attempting to resync.",
                        Logger.INFO, ConsoleColor.White);
                    Address address = GetRandomAddress();
                    SyncNode(address);
                    Logger.Log(
                        $"[Info] App: Chain resync complete.",
                        Logger.INFO, ConsoleColor.White);
                }
            }
            else
            {
                throw new ArgumentException(
                    $"invalid protocol header: {header}");
            }
        }

        /// <summary>
        /// Processes an incoming payload with a response header.
        /// </summary>
        private static void ProcessResponsePayload(Payload inboundPayload)
        {
            string header = inboundPayload.Header;
            if (header == Protocol.RESPONSE_ROUTING_TABLE)
            {
                _routingTable.Sync(inboundPayload.Body);
                Logger.Log(
                    "[Info] App: Routing table synced.",
                    Logger.INFO, ConsoleColor.Blue);
            }
            else if (header == Protocol.RESPONSE_BLOCKCHAIN)
            {
                lock (_node)
                {
                    _node.SyncBlockChain(inboundPayload.Body);
                }
                Logger.Log(
                    "[Info] App: Blockchain synced.",
                    Logger.INFO, ConsoleColor.Blue);
            }
            else if (header == Protocol.RESPONSE_TRANSACTION_POOL)
            {
                lock (_node)
                {
                    _node.SyncTransactionPool(inboundPayload.Body);
                }
                Logger.Log(
                    "[Info] App: Transaction pool synced.",
                    Logger.INFO, ConsoleColor.Blue);
            }
            else
            {
                throw new ArgumentException(
                    $"invalid protocol header: {header}");
            }
        }
    }
}