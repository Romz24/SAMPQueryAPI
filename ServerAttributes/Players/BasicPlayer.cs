namespace SAMPQueryAPI
{
    /// <summary>
    /// Holds basic information about a player (Name and score)
    /// </summary>
    public class BasicPlayer
    {
        public string Name { get; }
        public int Score { get; }

        public BasicPlayer(string name, int score)
        {
            Name = name;
            Score = score;
        }
    }
}
