using System;
using System.Net.Sockets;
using System.Text;
using ToyBlockChain.Util;

namespace ToyBlockChain.Network
{
    public class Network
    {
        /// <summary>
        /// Read payload from given network stream.
        /// </summary>
        public static Payload ReadPayload(NetworkStream stream)
        {
            byte[] inboundBytes = new byte[Protocol.BUFFER_SIZE];
            string inboundString = null;
            int numBytesRead = stream.Read(
                inboundBytes, 0, inboundBytes.Length);
            inboundString = Encoding.UTF8.GetString(
                inboundBytes, 0, numBytesRead);
            Payload inboundPayload = new Payload(inboundString);
            Logger.Log($"Received: {inboundPayload.ToSerializedString()}",
                ConsoleColor.Green);
            return inboundPayload;
        }

        /// <summary>
        /// Write payload to given network stream.
        /// </summary>
        public static void WritePayload(
            NetworkStream stream, Payload outboundPayload)
        {
            stream.Write(
                outboundPayload.ToSerializedBytes(), 0,
                outboundPayload.ToSerializedBytes().Length);
            Logger.Log($"Sent: {outboundPayload.ToSerializedString()}",
                ConsoleColor.Red);
        }
    }
}
