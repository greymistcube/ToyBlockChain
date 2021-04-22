using System;
using System.Text;

namespace ToyBlockChain.Network
{
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
            _header = serializedString[..Protocol.HEADER_SIZE];
            _body = serializedString[Protocol.HEADER_SIZE..];
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
            return $"{_header}{_body}";
        }

        public byte[] ToSerializedBytes()
        {
            return Encoding.UTF8.GetBytes(ToSerializedString());
        }
    }
}