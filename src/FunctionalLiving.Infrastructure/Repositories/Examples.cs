namespace FunctionalLiving.Infrastructure.Repositories
{
    using Example;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.SqlStreamStore;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using SqlStreamStore;

    public class Examples : Repository<Example, ExampleId>, IExamples
    {
        public Examples(ConcurrentUnitOfWork unitOfWork, IStreamStore eventStore, EventMapping eventMapping, EventDeserializer eventDeserializer)
            : base(Example.Factory, unitOfWork, eventStore, eventMapping, eventDeserializer) { }
    }
}
