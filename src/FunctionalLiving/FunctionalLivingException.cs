namespace FunctionalLiving
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;

    public abstract class FunctionalLivingException : DomainException
    {
        protected FunctionalLivingException() { }

        protected FunctionalLivingException(string message) : base(message) { }

        protected FunctionalLivingException(string message, Exception inner) : base(message, inner) { }
    }
}
