namespace KNXLibTests.Unit
{
    using FunctionalLiving.Knx;
    using FunctionalLiving.Knx.Addressing;
    using FunctionalLiving.Knx.Enums;
    using Xunit;
    using Xunit.Categories;

    public class KnxControlField2Test
    {
        [Category("KNXLib.Unit.ControlField2"), Fact]
        public void ConversionTest()
        {
            var cf = new KnxControlField2(KnxDestinationAddressType.Group, 5);
            Assert.Equal(0xd0, cf.GetValue());

            var cfNew = new KnxControlField2(cf.GetValue());
            Assert.Equal(KnxDestinationAddressType.Group, cfNew.DestinationAddressType);
            Assert.Equal(5, cfNew.HopCount);
        }

        [Category("KNXLib.Unit.ControlField2"), Fact]
        public void PassAddress()
        {
            var cf = new KnxControlField2(new KnxThreeLevelGroupAddress(12, 3, 4));
            Assert.Equal(0xE0, cf.GetValue());

            var cfNew = new KnxControlField2(new KnxIndividualAddress(1, 2, 3));
            Assert.Equal(0x70, cfNew.GetValue());
        }
    }
}
