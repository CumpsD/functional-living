namespace KNXLibTests.Unit.Addressing
{
    using FunctionalLiving.Knx.Addressing;
    using FunctionalLiving.Knx.Exceptions;
    using Xunit;
    using Xunit.Categories;

    public class KnxThreeLevelGroupAddressTest
    {
        [Category("KNXLib.Unit.Address.ThreeLevel"), Theory]
        [InlineData(0, 0, 0)]    // 0/0/0 is not allowed
        [InlineData(41, 2, 5)]   // Main too high
        [InlineData(15, 8, 5)]   // Middle too high
        [InlineData(15, 2, 321)] // Sub too high
        public void InvalidTest(int mainGroup, int middleGroup, int subGroup)
        {
            var ga = new KnxThreeLevelGroupAddress(mainGroup, middleGroup, subGroup);
            Assert.False(ga.IsValid());

            // Test if exception is thrown when using an invalid GA
            Assert.Throws<InvalidKnxAddressException>(() => ga.GetAddress());
        }

        [Category("KNXLib.Unit.Address.ThreeLevel"), Theory]
        [InlineData(0, 0, 1)]     // Min
        [InlineData(31, 7, 255)]  // Max
        [InlineData(18, 5, 230)]
        [InlineData(21, 7, 255)]
        public void ValidTest(int mainGroup, int middleGroup, int subGroup)
        {
            var ga = new KnxThreeLevelGroupAddress(mainGroup, middleGroup, subGroup);

            Assert.True(ga.IsValid());
            Assert.Equal(mainGroup, ga.MainGroup);
            Assert.Equal(middleGroup, ga.MiddleGroup);
            Assert.Equal(subGroup, ga.SubGroup);
        }

        [Category("KNXLib.Unit.Address.ThreeLevel"), Theory]
        [InlineData("0/0/1", 0, 0, 1)]
        [InlineData("31/7/255", 31, 7, 255)]
        [InlineData("18/5/230", 18, 5, 230)]
        [InlineData("21/7/255", 21, 7, 255)]
        public void ValidParserTest(string groupAddress, int mainGroup, int middleGroup, int subGroup)
        {
            var ga = new KnxThreeLevelGroupAddress(groupAddress);
            Assert.True(ga.IsValid());
            Assert.Equal(mainGroup, ga.MainGroup);
            Assert.Equal(middleGroup, ga.MiddleGroup);
            Assert.Equal(subGroup, ga.SubGroup);
        }

        [Category("KNXLib.Unit.Address.ThreeLevel"), Theory]
        [InlineData("0/0/0")]
        [InlineData("35/45/65")]
        [InlineData("5,6,4")]
        [InlineData("")]
        public void InvalidParserTest(string groupAddress)
        {
            var ga = new KnxThreeLevelGroupAddress(groupAddress);
            Assert.False(ga.IsValid());

            // Test if exception is thrown when using an invalid GA
            Assert.Throws<InvalidKnxAddressException>(() => ga.GetAddress());
        }

        [Category("KNXLib.Unit.Address.ThreeLevel"), Theory]
        [InlineData(20, 0, 180, new byte[] { 0xa0, 0xb4 })]
        [InlineData(10, 2, 0, new byte[] { 0x52, 0x00 })]
        public void ConversionTest(int mainGroup, int middleGroup, int subGroup, byte[] expected)
        {
            var ga = new KnxThreeLevelGroupAddress(mainGroup, middleGroup, subGroup);
            var address = ga.GetAddress();
            var gaNew = new KnxThreeLevelGroupAddress(address);

            Assert.Equal(expected, address);
            Assert.Equal(ga.MainGroup, gaNew.MainGroup);
            Assert.Equal(ga.MiddleGroup, gaNew.MiddleGroup);
            Assert.Equal(ga.SubGroup, gaNew.SubGroup);
        }

        [Category("KNXLib.Unit.Address.ThreeLevel"), Fact]
        public void EqualTest()
        {
            var ga1 = new KnxThreeLevelGroupAddress(1, 2, 3);
            var ga2 = new KnxThreeLevelGroupAddress(1, 2, 3);
            var ga3 = new KnxThreeLevelGroupAddress(4, 5, 6);

            Assert.True(ga1.Equals(ga2));
            Assert.False(ga1.Equals(ga3));

            var gaTwoLevel = new KnxTwoLevelGroupAddress(ga1.GetAddress());

            Assert.False(ga1.Equals(gaTwoLevel));

            var pa1 = new KnxIndividualAddress(1, 2, 3);
            var pa2 = new KnxIndividualAddress(ga1.GetAddress());

            Assert.False(ga1.Equals(pa1));
            Assert.False(ga1.Equals(pa2));
        }
    }
}
