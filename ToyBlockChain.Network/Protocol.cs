using System;
using System.Text;

namespace ToyBlockChain.Network
{
    public class Protocol
    {
        public const string REQUEST_ROUTING_TABLE = "<QR>";
        public const string REQUEST_BLOCKCHAIN = "<QB>";
        public const string REQUEST_ACCOUNT_CATALOGUE = "<QA>";
        public const string REQUEST_TRANSACTION_POOL = "<QT>";
        public const string RESPONSE_ROUTING_TABLE = "<RR>";
        public const string RESPONSE_BLOCKCHAIN = "<RB>";
        public const string RESPONSE_ACCOUNT_CATALOGUE = "<RA>";
        public const string RESPONSE_TRANSACTION_POOL = "<RT>";
        public const string ANNOUNCE_ADDRESS = "<AA>";
        public const string ANNOUNCE_ACCOUNT = "<AC>";
        public const string ANNOUNCE_TRANSACTION = "<AT>";
        public const string ANNOUNCE_BLOCK = "<AB>";
        public const int BUFFER_SIZE = 1024;
        public const int HEADER_SIZE = 4;
        public static readonly string[] REQUEST = new string[] {
            REQUEST_ROUTING_TABLE, REQUEST_BLOCKCHAIN,
            REQUEST_ACCOUNT_CATALOGUE, REQUEST_TRANSACTION_POOL };
        public static readonly string[] RESPONSE = new string[] {
            RESPONSE_ROUTING_TABLE, RESPONSE_BLOCKCHAIN,
            RESPONSE_ACCOUNT_CATALOGUE, REQUEST_TRANSACTION_POOL };
        public static readonly string[] ANNOUNCE = new string[] {
            ANNOUNCE_ADDRESS, ANNOUNCE_ACCOUNT,
            ANNOUNCE_TRANSACTION, ANNOUNCE_BLOCK };
    }
}