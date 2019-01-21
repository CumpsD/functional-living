namespace KNXLibTests.Unit.Addressing
{
    using FunctionalLiving.Knx.Addressing;
    using FunctionalLiving.Knx.Enums;
    using Xunit;
    using Xunit.Categories;

    public class KnxGroupAddressTest
    {
        [Category("KNXLib.Unit.Address"), Fact]
        public void StringParserTestThreeLevel()
        {
            var ga = KnxGroupAddress.Parse("18/5/230");
            Assert.IsAssignableFrom<KnxThreeLevelGroupAddress>(ga);

            var threeLevelGa = (KnxThreeLevelGroupAddress)ga;

            Assert.Equal(18, threeLevelGa.MainGroup);
            Assert.Equal(5, threeLevelGa.MiddleGroup);
            Assert.Equal(230, threeLevelGa.SubGroup);

            Assert.True(threeLevelGa.Equals(18, 5, 230));
            Assert.True(threeLevelGa.Equals("18/5/230"));
            Assert.True(ga.Equals("18/5/230"));
        }

        [Category("KNXLib.Unit.Address"), Fact]
        public void StringParserTestTwoLevel()
        {
            var ga = KnxGroupAddress.Parse("18/230");
            Assert.IsAssignableFrom<KnxTwoLevelGroupAddress>(ga);

            var threeLevelGa = (KnxTwoLevelGroupAddress)ga;

            Assert.Equal(18, threeLevelGa.MainGroup);
            Assert.Equal(230, threeLevelGa.SubGroup);

            Assert.True(threeLevelGa.Equals(18, 230));
            Assert.True(threeLevelGa.Equals("18/230"));
        }

        [Category("KNXLib.Unit.Address"), Fact]
        public void StringParserTestFreeStyle()
        {
            var ga = KnxGroupAddress.Parse("230");
            Assert.IsAssignableFrom<KnxFreeStyleGroupAddress>(ga);

            var threeLevelGa = (KnxFreeStyleGroupAddress)ga;

            Assert.Equal(230, threeLevelGa.SubGroup);

            Assert.True(threeLevelGa.Equals(230));
            Assert.True(threeLevelGa.Equals("230"));
        }

        [Category("KNXLib.Unit.Address"), Fact]
        public void ByteParserTest()
        {
            var gaThreeLevel = KnxGroupAddress.Parse(new byte[] { 0xa0, 0xb4 }, KnxGroupAddressStyle.ThreeLevel);
            var gaTwoLevel = KnxGroupAddress.Parse(new byte[] { 0xa0, 0xb4 }, KnxGroupAddressStyle.TwoLevel);
            var gaFreeStyle = KnxGroupAddress.Parse(new byte[] { 0xa0, 0xb4 }, KnxGroupAddressStyle.Free);

            Assert.IsAssignableFrom<KnxThreeLevelGroupAddress>(gaThreeLevel);
            Assert.IsAssignableFrom<KnxTwoLevelGroupAddress>(gaTwoLevel);
            Assert.IsAssignableFrom<KnxFreeStyleGroupAddress>(gaFreeStyle);

            Assert.Equal("20/0/180", gaThreeLevel.ToString());
            Assert.Equal("20/180", gaTwoLevel.ToString());
            Assert.Equal("41140", gaFreeStyle.ToString());
        }
    }
}
