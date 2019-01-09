namespace FunctionalLiving
{
    using System;

    public class NoNameException : FunctionalLivingException
    {
        public NoNameException() { }

        public NoNameException(string message) : base(message) { }

        public NoNameException(string message, Exception inner) : base(message, inner) { }
    }
}
