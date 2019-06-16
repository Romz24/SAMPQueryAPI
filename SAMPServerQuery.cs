using System;
using System.Text;
using SAMPQueryAPI.Connection;
using System.IO;

namespace SAMPQueryAPI
{
    public class SAMPServerQuery
    {
        public string ServerIp { get; private set; }
        public int ServerPort { get; private set; }
        public string RCONPassword { get; private set; }
        public bool RCONPasswordSet => (!String.IsNullOrEmpty(RCONPassword));

        public const int MaxResponseBytesForPlayers = 4096; // approx, max player has 4 attributes, 1000 players max in samp

        public SAMPServerQuery(string serverIp, int serverPort = 7777)
        {
            ServerIp = serverIp;
            ServerPort = serverPort;
        }

        public SAMPServerQuery(string serverIp, string rconPassword, int serverPort = 7777)
        {
            ServerIp = serverIp;
            ServerPort = serverPort;
            RCONPassword = rconPassword;
        }

        /// <summary>
        /// Change the targeted server's IP address and port.
        /// </summary>
        public void ChangeServerIP(string serverIp, int serverPort = 7777)
        {
            ServerIp = serverIp;
            ServerPort = serverPort;
        }

        /// <summary>
        /// Queries a basic list of connected players, displaying their names and score.
        /// </summary>
        public BasicPlayer[] GetBasicPlayers()
        {
            byte[] returnedBytes = SendOpcode('c', MaxResponseBytesForPlayers);
            if (returnedBytes != null)
            {
                MemoryStream memoryStream = new MemoryStream(returnedBytes);
                BinaryReader binaryReader = new BinaryReader(memoryStream);
                binaryReader.ReadBytes(11);

                int amountOfPlayers = binaryReader.ReadInt16();
                BasicPlayer[] players = new BasicPlayer[amountOfPlayers];

                for (int i = 0; i < amountOfPlayers; i++)
                {
                    string playerName = new string(binaryReader.ReadChars(binaryReader.ReadByte()));
                    int playerScore = binaryReader.ReadInt32();
                    players[i] = new BasicPlayer(playerName, playerScore);
                }

                return players;
            }
            return null;
        }

        /// <summary>
        /// Queries a detailed list of connected players, displaying their ids, names, score and ping.
        /// </summary>
        public DetailedPlayer[] GetDetailedPlayers()
        {
            byte[] returnedBytes = SendOpcode('d', MaxResponseBytesForPlayers);
            if (returnedBytes != null)
            {
                MemoryStream memoryStream = new MemoryStream(returnedBytes);
                BinaryReader binaryReader = new BinaryReader(memoryStream);
                binaryReader.ReadBytes(11);

                int amountOfPlayers = binaryReader.ReadInt16();
                DetailedPlayer[] players = new DetailedPlayer[amountOfPlayers];

                for (int i = 0; i < amountOfPlayers; i++)
                {
                    int playerId = binaryReader.ReadByte();
                    string playerName = new string(binaryReader.ReadChars(binaryReader.ReadByte()));
                    int playerScore = binaryReader.ReadInt32();
                    int playerPing = binaryReader.ReadInt32();
                    players[i] = new DetailedPlayer(playerId, playerName, playerScore, playerPing);
                }

                return players;
            }
            return null;
        }

        /// <summary>
        /// Queries the server's rules (a set of configurations).
        /// </summary>
        public ServerRule[] GetServerRules()
        {
            byte[] returnedBytes = SendOpcode('r');
            if (returnedBytes != null)
            {
                MemoryStream memoryStream = new MemoryStream(returnedBytes);
                BinaryReader binaryReader = new BinaryReader(memoryStream);
                binaryReader.ReadBytes(11);

                int amountOfRules = binaryReader.ReadInt16();
                ServerRule[] rules = new ServerRule[amountOfRules];

                for (int i = 0; i < amountOfRules; i++)
                {
                    string rulename = new string(binaryReader.ReadChars(binaryReader.ReadByte()));
                    string value = new string(binaryReader.ReadChars(binaryReader.ReadByte()));
                    rules[i] = new ServerRule(rulename, value);
                }

                return rules;
            }
            return null;
        }

        /// <summary>
        /// Queries server information like it's hostname, amount of players and more.
        /// </summary>
        public ServerInformation GetServerInformation()
        {
            byte[] returnedBytes = SendOpcode('i');
            if (returnedBytes != null)
            {
                MemoryStream memoryStream = new MemoryStream(returnedBytes);
                BinaryReader binaryReader = new BinaryReader(memoryStream);
                binaryReader.ReadBytes(11);

                bool passworded = (binaryReader.ReadByte() == 1);
                int players = binaryReader.ReadInt16();
                int maxPlayers = binaryReader.ReadInt16();

                int hostname_len = binaryReader.ReadInt32();
                string hostname = Encoding.Default.GetString(binaryReader.ReadBytes(hostname_len));

                int gamemode_len = binaryReader.ReadInt32();
                string gamemode = Encoding.Default.GetString(binaryReader.ReadBytes(gamemode_len));

                int language_len = binaryReader.ReadInt32();
                string language = Encoding.Default.GetString(binaryReader.ReadBytes(language_len));

                ServerInformation serverInformation = new ServerInformation(passworded, players, maxPlayers, hostname, gamemode, language);
                return serverInformation;
            }
            return null;
        }

        /// <summary>
        /// Queries the server ping.
        /// </summary>
        public int GetServerPing()
        {
            QueryConnection query = new QueryConnection(ServerIp, (ushort)ServerPort);
            return query.GetPing();
        }

        /// <summary>
        /// Queries a RCON command to the server (if there's a RCON password set).
        /// </summary>
        public void SendRCONCommand(string command)
        {
            if (!RCONPasswordSet)
                throw new Exception("A RCON password is not set.");
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets/changes the RCON password to the server (if not set before).
        /// </summary>
        public void SetRCONPassword(string newPassword) => RCONPassword = newPassword;

        private byte[] SendOpcode(char opcode, int responseSize = 1024)
        {
            QueryConnection query = new QueryConnection(ServerIp, (ushort)ServerPort);
            return query.SendOpcode(opcode, responseSize);
        }
    }
}
