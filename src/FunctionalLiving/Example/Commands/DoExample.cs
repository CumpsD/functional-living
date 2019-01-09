namespace FunctionalLiving.Example.Commands
{
    public class DoExample
    {
        public ExampleId ExampleId { get; }

        public ExampleName Name { get; }

        public DoExample(
            ExampleId exampleId,
            ExampleName name)
        {
            ExampleId = exampleId;
            Name = name;
        }
    }
}
