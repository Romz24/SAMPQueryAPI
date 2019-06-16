namespace SAMPQueryAPI
{
    /// <summary>
    /// Holds more detailed information about a player (Nickname, score and ping)
    /// </summary>
    public class DetailedPlayer : BasicPlayer
    {
        public int Id { get; }
        public int Ping { get; }

        public DetailedPlayer(int id, string name, int score, int ping) : base(name, score)
        {
            Id = id;
            Ping = ping;
        }
    }
}
