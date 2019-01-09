namespace FunctionalLiving.Tests.AutoFixture
{
    using System;
    using global::AutoFixture;
    using global::AutoFixture.Xunit2;

    public class DefaultDataAttribute : AutoDataAttribute
    {
        public DefaultDataAttribute() : this(() => new Fixture()) { }

        protected DefaultDataAttribute(Func<IFixture> fixtureFactory)
            : base(() => fixtureFactory().Customize(new WithDefaults())) { }
    }

    public class WithDefaults : CompositeCustomization
    {
    }
}
