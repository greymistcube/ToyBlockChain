using System;

namespace ToyBlockChain.App
{
    public partial class Program
    {
        private static Address GetLocalAddress()
        {
            if (_seedFlag)
            {
                return _SEED_ADDRESS;
            }
            else
            {
                // Generate a new random address that is not already in
                // the routing table.
                Random rnd = new Random();
                while (true)
                {
                    Address address = new Address(
                        Const.IP_ADDRESS,
                        rnd.Next(Const.PORT_NUM_MIN, Const.PORT_NUM_MAX));
                    if (!_routingTable.Table.Contains(address))
                    {
                        return address;
                    }
                }
            }
        }

        /// <summary>
        /// Get a random address from the routing table excluding
        /// this node's address.
        /// </summary>
        private static Address GetRandomAddress()
        {
            if (_routingTable.Table.Count <= 1)
            {
                throw new MethodAccessException(
                    "invalid access; routing table size too small: "
                    + $"{_routingTable.Table.Count}");
            }
            else
            {
                Address address;
                Random rnd = new Random();
                int idx;

                idx = rnd.Next(_routingTable.Table.Count);
                address = _routingTable.Table[idx];
                if (_address.Equals(address))
                {
                    idx = (idx + 1) + rnd.Next(_routingTable.Table.Count - 1);
                    idx = idx % _routingTable.Table.Count;
                    address = _routingTable.Table[idx];
                }
                return address;
            }
        }
    }
}