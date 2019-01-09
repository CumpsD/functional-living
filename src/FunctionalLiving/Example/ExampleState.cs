namespace FunctionalLiving.Example
{
    using System.Collections.Generic;
    using Events;

    public partial class Example
    {
        private ExampleId _exampleId;

        private readonly Dictionary<Language, ExampleName> _names
            = new Dictionary<Language, ExampleName>();

        private Example()
        {
            Register<ExampleWasBorn>(When);
            Register<ExampleHappened>(When);
        }

        private void When(ExampleWasBorn @event)
        {
            _exampleId = new ExampleId(@event.ExampleId);
        }

        private void When(ExampleHappened @event)
        {
            _names[@event.Language] = new ExampleName(@event.Name, @event.Language);
        }
    }
}
