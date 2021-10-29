using System;

namespace PegasusApi.Library.Exceptions
{
    public abstract class EmailException : Exception
    {
        protected EmailException()
        {
        }

        protected EmailException(string message)
            : base(message)
        {
        }

        protected EmailException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class EmailApiKeyNotFoundException : EmailException
    {
        public EmailApiKeyNotFoundException()
        {
        }

        public EmailApiKeyNotFoundException(string message)
            : base(message)
        {
        }

        public EmailApiKeyNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}