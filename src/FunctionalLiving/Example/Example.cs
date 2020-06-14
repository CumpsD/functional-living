namespace FunctionalLiving.Example
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using System;

    public partial class Example : AggregateRootEntity
    {
        public static readonly Func<Example> Factory = () => new Example();

        public static Example Register(ExampleId id)
        {
            var example = Factory();
            //example.ApplyChange(new ExampleWasBorn(id));
            return example;
        }

        public void DoExample(ExampleName name)
        {
            //ApplyChange(new ExampleHappened(_exampleId, name));
        }
    }
}
