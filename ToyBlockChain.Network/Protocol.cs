using System;
using System.Text;

namespace ToyBlockChain.Network
{
    public class Protocol
    {
        public const string REQUEST_ROUTING_TABLE = "<RQRT>";
        public const string REQUEST_BLOCKCHAIN = "<RQBC>";
        public const string REQUEST_TRANSACTION_POOL = "<RQTP>";
        public const string RESPONSE_ROUTING_TABLE = "<RSRT>";
        public const string RESPONSE_BLOCKCHAIN = "<RSBC>";
        public const string RESPONSE_TRANSACTION_POOL = "<RSTP>";
        public const string ANNOUNCE_ADDRESS = "<ANAD>";
        public const string ANNOUNCE_TRANSACTION = "<ANTX>";
        public const string ANNOUNCE_BLOCK = "<ANBK>";
        public const int BUFFER_SIZE = 65536;
        public const int HEADER_SIZE = 6;
        public static readonly string[] REQUEST = new string[] {
            REQUEST_ROUTING_TABLE,
            REQUEST_BLOCKCHAIN, REQUEST_TRANSACTION_POOL };
        public static readonly string[] RESPONSE = new string[] {
            RESPONSE_ROUTING_TABLE,
            RESPONSE_BLOCKCHAIN, RESPONSE_TRANSACTION_POOL };
        public static readonly string[] ANNOUNCE = new string[] {
            ANNOUNCE_ADDRESS,
            ANNOUNCE_TRANSACTION, ANNOUNCE_BLOCK };
    }
}