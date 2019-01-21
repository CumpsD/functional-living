namespace KNXLibTests.Unit
{
    using FunctionalLiving.Knx;
    using FunctionalLiving.Knx.Enums;
    using Xunit;
    using Xunit.Categories;

    public class KnxControlField1Test
    {
        [Category("KNXLib.Unit.ControlField1"), Fact]
        public void ConversionTest()
        {
            var cf = new KnxControlField1(KnxTelegramType.StandardFrame, KnxTelegramRepetitionStatus.Original, KnxTelegramPriority.Low);
            Assert.Equal(0xbc, cf.GetValue());

            var cfNew = new KnxControlField1(cf.GetValue());
            Assert.Equal(KnxTelegramType.StandardFrame, cfNew.TelegramType);
            Assert.Equal(KnxTelegramRepetitionStatus.Original, cfNew.TelegramRepetitionStatus);
            Assert.Equal(KnxTelegramPriority.Low, cfNew.TelegramPriority);
        }
    }
}
