using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace SAMPQueryAPI.Connection
{
    class QueryConnection
    {
        public string ServerStringIP { get; }
        public ushort ServerPort { get; }

        private IPAddress serverIp;
        private IPEndPoint serverEndPoint;
        private Socket serverSocket;
        public int ServerPing { get; private set; }

        public const int SocketSendTimeout = 3000;
        public const int SocketReceiveTimeout = 3000;
        public const string PacketHeader = "SAMP";

        public QueryConnection(string ipAddress, ushort port)
        {
            ServerStringIP = ipAddress;
            ServerPort = port;

            serverIp = new IPAddress(IPAddress.Parse(ipAddress).GetAddressBytes());
            serverEndPoint = new IPEndPoint(serverIp, port);
        }

        public byte[] SendOpcode(char opcode, int responseSize = 1024)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

            using (memoryStream)
            {
                using (binaryWriter)
                {
                    string[] seperatedIp = ServerStringIP.Split('.');
                    binaryWriter.Write(PacketHeader.ToCharArray());
                    for (int i = 0; i < 4; i++)
                        binaryWriter.Write(Convert.ToByte(Convert.ToInt16(seperatedIp[i])));
                    binaryWriter.Write(ServerPort);
                    binaryWriter.Write(opcode);
                }
            }

            try
            {
                return Send(memoryStream.ToArray(), responseSize);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public int GetPing()
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

            using (memoryStream)
            {
                using (binaryWriter)
                {
                    string[] seperatedIp = ServerStringIP.Split('.');
                    binaryWriter.Write(PacketHeader.ToCharArray());
                    for (int i = 0; i < 4; i++)
                        binaryWriter.Write(Convert.ToByte(Convert.ToInt16(seperatedIp[i])));
                    binaryWriter.Write(ServerPort);
                    binaryWriter.Write('p');
                    byte[] randomValues = new byte[4];
                    Random randomizer = new Random();
                    randomizer.NextBytes(randomValues);
                    binaryWriter.Write(randomValues);
                }
            }

            try
            {
                Send(memoryStream.ToArray(), 15);
                return ServerPing;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public byte[] SendRconCommand(string rconCommand)
        {
            throw new NotImplementedException();
        }

        private byte[] Send(byte[] bytes, int responseSize)
        {
            char opcode = (char)bytes[10];

            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                serverSocket.SendTimeout = SocketSendTimeout;
                serverSocket.ReceiveTimeout = SocketReceiveTimeout;
            }
            catch (Exception e)
            {
                throw new QueryPacketException(opcode, "Failed to initiate connection or send the packet to the server.", e);
            }

            DateTime packetSent = DateTime.Now;
            int listenPort = serverSocket.SendTo(bytes, serverEndPoint);

            byte[] receivedBytes = new byte[responseSize];
            EndPoint local = serverSocket.LocalEndPoint;
            try
            {
                serverSocket.ReceiveFrom(receivedBytes, ref local);
            }
            catch (Exception e)
            {
                throw new QueryPacketException(opcode, "Failed to receive a packet from the server, perhaps it timed out.", e);
            }
            DateTime packetReceived = DateTime.Now;
            ServerPing = packetReceived.Subtract(packetSent).Milliseconds;

            return receivedBytes;
        }
    }
}