using System;

namespace SAMPQueryAPI
{
    class QueryPacketException : Exception
    {
        public char Opcode;

        public QueryPacketException(char opcode) : base()
        {
            Opcode = opcode;
        }

        public QueryPacketException(char opcode, string message) : base(message)
        {
            Opcode = opcode;
        }

        public QueryPacketException(char opcode, string message, Exception innerException) : base(message, innerException)
        {
            Opcode = opcode;
        }
    }
}
