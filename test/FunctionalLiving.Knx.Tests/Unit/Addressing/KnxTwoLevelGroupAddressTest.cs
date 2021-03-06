namespace KNXLibTests.Unit.Addressing
{
    using FunctionalLiving.Knx.Addressing;
    using FunctionalLiving.Knx.Exceptions;
    using Xunit;
    using Xunit.Categories;

    public class KnxTwoLevelGroupAddressTest
    {
        [Category("KNXLib.Unit.Address.TwoLevel"), Theory]
        [InlineData(0, 0)]        // 0/0/0 is not allowed
        [InlineData(41, 5)]       // Main too high
        [InlineData(15, 3542)]    // Sub too high
        public void InvalidTest(int mainGroup, int subGroup)
        {
            var ga = new KnxTwoLevelGroupAddress(mainGroup, subGroup);
            Assert.False(ga.IsValid());

            // Test if exception is thrown when using an invalid GA
            Assert.Throws<InvalidKnxAddressException>(() => ga.GetAddress());
        }

        [Category("KNXLib.Unit.Address.TwoLevel"), Theory]
        [InlineData(0, 1)]        // Min
        [InlineData(31, 2047)]    // Max
        [InlineData(18, 230)]
        [InlineData(21, 1356)]
        public void ValidTest(int mainGroup, int subGroup)
        {
            var ga = new KnxTwoLevelGroupAddress(mainGroup, subGroup);

            Assert.True(ga.IsValid());
            Assert.Equal(mainGroup, ga.MainGroup);
            Assert.Equal(subGroup, ga.SubGroup);
        }

        [Category("KNXLib.Unit.Address.TwoLevel"), Theory]
        [InlineData("0/1", 0, 1)]
        [InlineData("31/2047", 31, 2047)]
        [InlineData("18/230", 18, 230)]
        [InlineData("21/1356", 21, 1356)]
        public void ValidParserTest(string groupAddress, int mainGroup, int subGroup)
        {
            var ga = new KnxTwoLevelGroupAddress(groupAddress);
            Assert.True(ga.IsValid());
            Assert.Equal(mainGroup, ga.MainGroup);
            Assert.Equal(subGroup, ga.SubGroup);
        }

        [Category("KNXLib.Unit.Address.TwoLevel"), Theory]
        [InlineData("0/0/0")]
        [InlineData("0/0")]
        [InlineData("35/45")]
        [InlineData("5,6")]
        [InlineData("")]
        public void InvalidParserTest(string groupAddress)
        {
            var ga = new KnxTwoLevelGroupAddress(groupAddress);
            Assert.False(ga.IsValid());

            // Test if exception is thrown when using an invalid GA
            Assert.Throws<InvalidKnxAddressException>(() => ga.GetAddress());
        }

        [Category("KNXLib.Unit.Address.TwoLevel"), Theory]
        [InlineData(20, 180, new byte[] { 0xa0, 0xb4 })]
        [InlineData(10, 512, new byte[] { 0x52, 0x00 })]
        public void ConversionTest(int mainGroup, int subGroup, byte[] expected)
        {
            var ga = new KnxTwoLevelGroupAddress(mainGroup, subGroup);
            var address = ga.GetAddress();
            var gaNew = new KnxTwoLevelGroupAddress(address);

            Assert.Equal(expected, address);
            Assert.Equal(ga.MainGroup, gaNew.MainGroup);
            Assert.Equal(ga.MiddleGroup, gaNew.MiddleGroup);
            Assert.Equal(ga.SubGroup, gaNew.SubGroup);
        }

        [Category("KNXLib.Unit.Address.TwoLevel"), Fact]
        public void EqualTest()
        {
            var ga1 = new KnxTwoLevelGroupAddress(1, 3);
            var ga2 = new KnxTwoLevelGroupAddress(1, 3);
            var ga3 = new KnxTwoLevelGroupAddress(4, 6);

            Assert.True(ga1.Equals(ga2));
            Assert.False(ga1.Equals(ga3));

            var pa1 = new KnxIndividualAddress(1, 0, 3);
            var pa2 = new KnxIndividualAddress(ga1.GetAddress());

            Assert.False(ga1.Equals(pa1));
            Assert.False(ga1.Equals(pa2));
        }
    }
}
