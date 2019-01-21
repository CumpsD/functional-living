namespace KNXLibTests.Unit.Addressing
{
    using FunctionalLiving.Knx.Addressing;
    using FunctionalLiving.Knx.Exceptions;
    using Xunit;
    using Xunit.Categories;

    public class KnxFreeStyleGroupAddressTest
    {
        [Category("KNXLib.Unit.Address.FreeStyle"), Theory]
        [InlineData(0)]        // 0/0/0 is not allowed
        [InlineData(70565)]    // Sub too high
        public void InvalidTest(int subGroup)
        {
            var ga = new KnxFreeStyleGroupAddress(subGroup);
            Assert.False(ga.IsValid());

            // Test if exception is thrown when using an invalid GA
            Assert.Throws<InvalidKnxAddressException>(() => ga.GetAddress());
        }

        [Category("KNXLib.Unit.Address.FreeStyle"), Theory]
        [InlineData(1)]        // Min
        [InlineData(65535)]    // Max
        [InlineData(18)]
        [InlineData(12045)]
        public void ValidTest(int subGroup)
        {
            var ga = new KnxFreeStyleGroupAddress(subGroup);

            Assert.True(ga.IsValid());
            Assert.Equal(subGroup, ga.SubGroup);
        }

        [Category("KNXLib.Unit.Address.FreeStyle"), Theory]
        [InlineData("1", 1)]
        [InlineData("65535", 65535)]
        [InlineData("18", 18)]
        [InlineData("12045", 12045)]
        public void ValidParserTest(string groupAddress, int subGroup)
        {
            var ga = new KnxFreeStyleGroupAddress(groupAddress);
            Assert.True(ga.IsValid());
            Assert.Equal(subGroup, ga.SubGroup);
        }

        [Category("KNXLib.Unit.Address.FreeStyle"), Theory]
        [InlineData("0/0/0")]
        [InlineData("0/0")]
        [InlineData("0")]
        [InlineData("70235")]
        [InlineData("5,6")]
        [InlineData("")]
        public void InvalidParserTest(string groupAddress)
        {
            var ga = new KnxFreeStyleGroupAddress(groupAddress);
            Assert.False(ga.IsValid());

            // Test if exception is thrown when using an invalid GA
            Assert.Throws<InvalidKnxAddressException>(() => ga.GetAddress());
        }

        [Category("KNXLib.Unit.Address.FreeStyle"), Theory]
        [InlineData(41140, new byte[] { 0xa0, 0xb4 })]
        [InlineData(20992, new byte[] { 0x52, 0x00 })]
        public void ConversionTest(int subGroup, byte[] expected)
        {
            var ga = new KnxFreeStyleGroupAddress(subGroup);
            var address = ga.GetAddress();
            var gaNew = new KnxFreeStyleGroupAddress(address);

            Assert.Equal(expected, address);
            Assert.Equal(ga.MainGroup, gaNew.MainGroup);
            Assert.Equal(ga.MiddleGroup, gaNew.MiddleGroup);
            Assert.Equal(ga.SubGroup, gaNew.SubGroup);
        }

        [Category("KNXLib.Unit.Address.FreeStyle"), Fact]
        public void EqualTest()
        {
            var ga1 = new KnxFreeStyleGroupAddress(555);
            var ga2 = new KnxFreeStyleGroupAddress(555);
            var ga3 = new KnxFreeStyleGroupAddress(666);

            Assert.True(ga1.Equals(ga2));
            Assert.False(ga1.Equals(ga3));

            var pa1 = new KnxIndividualAddress(1, 0, 3);
            var pa2 = new KnxIndividualAddress(ga1.GetAddress());

            Assert.False(ga1.Equals(pa1));
            Assert.False(ga1.Equals(pa2));
        }
    }
}
