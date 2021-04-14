using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToyBlockChain.App
{
    public class Address
    {
        private readonly string _ipAddress;
        private readonly int _portNumber;

        public Address(string ipAddress, int portNumber)
        {
            _ipAddress = ipAddress;
            _portNumber = portNumber;
        }

        public Address(string serializedString)
        {
            string[] strings = serializedString.Split(':');
            _ipAddress = strings[0];
            _portNumber = Int32.Parse(strings[1]);
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
            return $"{_ipAddress}:{_portNumber}";
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

    public class RoutingTable
    {
        private List<Address> _routes;

        public RoutingTable()
        {
            _routes = new List<Address>();
        }

        public void Sync(string serializedString)
        {
            _routes = new List<Address>();
            string[] addressStrings = serializedString.Split(',');
            foreach (string addressString in addressStrings)
            {
                _routes.Add(new Address(addressString));
            }
        }

        public void AddAddress(Address address)
        {
            _routes.Add(address);
            return;
        }

        public string ToSerializedString()
        {
            return String.Join(',', _routes.Select(address => address.ToSerializedString()));
        }

        public byte[] ToSerializedBytes()
        {
            return Encoding.UTF8.GetBytes(ToSerializedString());
        }

        public List<Address> Routes
        {
            get
            {
                return _routes;
            }
        }
    }
}