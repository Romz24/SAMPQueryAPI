namespace SAMPQueryAPI
{
    public class ServerRule
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public ServerRule(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
