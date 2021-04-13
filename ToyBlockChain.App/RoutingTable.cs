using System;
using System.Collections.Generic;
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

        public override string ToString()
        {
            return $"{_ipAddress}:{_portNumber}";
        }

        public byte[] Serialize()
        {
            return Encoding.UTF8.GetBytes(ToString());
        }
    }
    public class RoutingTable
    {
        private readonly List<Address> _routingTable;

        public RoutingTable()
        {
            _routingTable = new List<Address>();
        }

        public void AddAddress(Address address)
        {
            _routingTable.Add(address);
            return;
        }

        public override string ToString()
        {
            return String.Join(',', _routingTable);
        }

        public byte[] Serialize()
        {
            return Encoding.UTF8.GetBytes(ToString());
        }
    }
}