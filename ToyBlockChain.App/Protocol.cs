using System;
using System.Text;

namespace ToyBlockChain.App
{
    public class Protocol
    {
        public const string REQUEST_ROUTING_TABLE = "QR";
        public const string REQUEST_BLOCKCHAIN = "QB";
        public const string RESPONSE_ROUTING_TABLE = "RR";
        public const string RESPONSE_BLOCKCHAIN = "RB";
        public const string ANNOUNCE_ADDRESS = "AA";
        public const string ANNOUNCE_TRANSACTION = "AT";
        public const string ANNOUNCE_BLOCK = "AB";
        public const int BUFFER_SIZE = 1024;
        public const int HEADER_SIZE = 2;
    }

    public class Payload
    {
        private string _header;
        private string _body;

        public Payload(string header, string body)
        {
            _header = header;
            _body = body;
        }

        public Payload(string serializedString)
        {
            string[] strings = serializedString.Split(',', 2);
            _header = strings[0];
            _body = strings[1];
        }

        public string Header
        {
            get
            {
                return _header;
            }
        }

        public string Body
        {
            get
            {
                return _body;
            }
        }

        public string ToSerializedString()
        {
            return $"{_header},{_body}";
        }

        public byte[] ToSerializedBytes()
        {
            return Encoding.UTF8.GetBytes(ToSerializedString());
        }
    }
}