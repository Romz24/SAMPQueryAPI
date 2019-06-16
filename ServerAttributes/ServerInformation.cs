namespace SAMPQueryAPI
{
    public class ServerInformation
    {
        public bool Passworded { get; }
        public int Players { get; }
        public int MaxPlayers { get; }
        public string Hostname { get; }
        public string Gamemode { get; }
        public string Language { get; }
        public int OpenSlots => MaxPlayers - Players;

        public ServerInformation(bool passworded, int players, int maxPlayers, string hostname, string gamemode, string language)
        {
            Passworded = passworded;
            Players = players;
            MaxPlayers = maxPlayers;
            Hostname = hostname;
            Gamemode = gamemode;
            Language = language;
        }
    }
}
