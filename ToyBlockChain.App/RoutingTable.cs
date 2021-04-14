using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToyBlockChain.App
{
    public class RoutingTable
    {
        public const string SEPARATOR = "<RT>";
        private List<Address> _routes;

        public RoutingTable()
        {
            _routes = new List<Address>();
        }

        public void Sync(string serializedString)
        {
            _routes = new List<Address>();
            string[] addressStrings = serializedString.Split(SEPARATOR);
            foreach (string addressString in addressStrings)
            {
                Address address = new Address(addressString);
                _routes.Add(address);
            }
        }

        public void AddAddress(Address address)
        {
            _routes.Add(address);
        }

        public string ToSerializedString()
        {
            return String.Join(
                SEPARATOR,
                _routes.Select(address => address.ToSerializedString()));
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