namespace FunctionalLiving
{
    using System;

    public class NoExampleIdException : FunctionalLivingException
    {
        public NoExampleIdException() { }

        public NoExampleIdException(string message) : base(message) { }

        public NoExampleIdException(string message, Exception inner) : base(message, inner) { }
    }
}
