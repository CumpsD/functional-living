namespace FunctionalLiving.Tests.DoExample
{
    using Xunit;
    using Xunit.Abstractions;
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using Example.Commands;
    using Example.Events;

    public class given_one_example : FunctionalLivingTest
    {
        public given_one_example(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Theory, DefaultData]
        public void then_example_should_have_been_born_and_had_happened(
            DoExample doExampleCommand)
        {
            Assert(new Scenario()
                .GivenNone()
                .When(doExampleCommand)
                .Then(doExampleCommand.ExampleId,
                    new ExampleWasBorn(doExampleCommand.ExampleId),
                    new ExampleHappened(doExampleCommand.ExampleId, doExampleCommand.Name)));
        }
    }
}
