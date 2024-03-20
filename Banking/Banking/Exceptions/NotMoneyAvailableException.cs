using System;

namespace Banking.Exceptions
{
    public  class NotMoneyAvailableException : Exception
    {
        public NotMoneyAvailableException() { }

        public NotMoneyAvailableException(string message)
            : base(message) { }

        public NotMoneyAvailableException(string message, Exception inner)
            : base(message, inner) { }
    }
}
