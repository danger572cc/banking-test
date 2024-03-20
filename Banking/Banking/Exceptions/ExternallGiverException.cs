using System;

namespace Banking.Exceptions
{
    public class ExternallGiverException : Exception
    {
        public ExternallGiverException() { }

        public ExternallGiverException(string message)
            : base(message) { }

        public ExternallGiverException(string message, Exception inner)
            : base(message, inner) { }
    }
}
