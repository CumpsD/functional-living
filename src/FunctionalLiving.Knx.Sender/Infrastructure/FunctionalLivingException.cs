namespace FunctionalLiving
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;

    public abstract class FunctionalLivingKnxException : Exception
    {
        protected FunctionalLivingKnxException() { }

        protected FunctionalLivingKnxException(string message) : base(message) { }

        protected FunctionalLivingKnxException(string message, Exception inner) : base(message, inner) { }
    }
}
