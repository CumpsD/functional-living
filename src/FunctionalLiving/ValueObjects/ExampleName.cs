namespace FunctionalLiving
{
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.AggregateSource;

    public class ExampleName : ValueObject<ExampleName>
    {
        public string Name { get; }

        public Language Language { get; }

        public ExampleName(string name, Language language)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new NoNameException("Name of an example cannot be empty.");

            Name = name;
            Language = language;
        }

        protected override IEnumerable<object> Reflect()
        {
            yield return Name;
            yield return Language;
        }

        public override string ToString() => $"{Name} ({Language})";
    }
}
