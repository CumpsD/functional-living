namespace KNXLibTests.Unit.DataPoint
{
    using FunctionalLiving.Knx.DPT;
    using Xunit;
    using Xunit.Categories;

    public class DataPoint8BitNoSignScaledPercentU8
    {
        [Category("KNXLib.Unit.DataPoint.8BitNoSign"), Fact]
        public void DataPoint8BitNoSignScaledPercentU8Test()
        {
            var dptType = "5.004";

            var perc0 = 0;
            var perc0Bytes = new byte[] { 0x00 };
            var perc97 = 97;
            var perc97Bytes = new byte[] { 0x61 };
            var perc128 = 128;
            var perc128Bytes = new byte[] { 0x80 };
            var perc199 = 199;
            var perc199Bytes = new byte[] { 0xC7 };
            var perc255 = 255;
            var perc255Bytes = new byte[] { 0xFF };

            Assert.Equal(perc0, DataPointTranslator.Instance.FromDataPoint(dptType, perc0Bytes));
            Assert.Equal(perc97, DataPointTranslator.Instance.FromDataPoint(dptType, perc97Bytes));
            Assert.Equal(perc128, DataPointTranslator.Instance.FromDataPoint(dptType, perc128Bytes));
            Assert.Equal(perc199, DataPointTranslator.Instance.FromDataPoint(dptType, perc199Bytes));
            Assert.Equal(perc255, DataPointTranslator.Instance.FromDataPoint(dptType, perc255Bytes));

            Assert.Equal(perc0Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc0));
            Assert.Equal(perc97Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc97));
            Assert.Equal(perc128Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc128));
            Assert.Equal(perc199Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc199));
            Assert.Equal(perc255Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc255));
        }
    }
}
