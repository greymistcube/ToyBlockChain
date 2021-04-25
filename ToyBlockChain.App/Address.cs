using System;
using System.Text;

namespace ToyBlockChain.App
{
    public class Address
    {
        public const string SEPARATOR = "<AD>";
        private readonly string _ipAddress;
        private readonly int _portNumber;

        public Address(string ipAddress, int portNumber)
        {
            _ipAddress = ipAddress;
            _portNumber = portNumber;
        }

        public Address(string serializedString)
        {
            string[] substrings = serializedString.Split(SEPARATOR);
            _ipAddress = substrings[0];
            _portNumber = Int32.Parse(substrings[1]);
        }

        public string IpAddress
        {
            get
            {
                return _ipAddress;
            }
        }

        public int PortNumber
        {
            get
            {
                return _portNumber;
            }
        }

        public string ToSerializedString()
        {
            return String.Join(
                SEPARATOR,
                new string[] {
                    IpAddress, PortNumber.ToString() });
        }

        public byte[] ToSerializedBytes()
        {
            return Encoding.UTF8.GetBytes(ToSerializedString());
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Address address = (Address)obj;
                return (
                    _ipAddress == address.IpAddress
                    && _portNumber == address.PortNumber);
            }
        }

        public override int GetHashCode()
        {
            return _ipAddress.GetHashCode() ^ _portNumber.GetHashCode();
        }
    }
}