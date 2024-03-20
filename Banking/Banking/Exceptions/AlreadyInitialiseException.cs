using System;

namespace Banking.Exceptions
{
    public class AlreadyInitialiseException : Exception
    {
        public AlreadyInitialiseException() { }

        public AlreadyInitialiseException(string message)
            : base(message) { }

        public AlreadyInitialiseException(string message, Exception inner)
            : base(message, inner) { }
    }
}
