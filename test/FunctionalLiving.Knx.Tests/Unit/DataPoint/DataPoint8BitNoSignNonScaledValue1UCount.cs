namespace KNXLibTests.Unit.DataPoint
{
    using FunctionalLiving.Knx.DPT;
    using Xunit;
    using Xunit.Categories;

    public class DataPoint8BitNoSignNonScaledValue1UCount
    {
        [Category("KNXLib.Unit.DataPoint.8BitNoSign"), Fact]
        public void DataPoint8BitNoSignNonScaledValue1UCountTest()
        {
            var dptType = "5.010";

            var count0 = 0;
            var count0Bytes = new byte[] { 0x00 };
            var count97 = 97;
            var count97Bytes = new byte[] { 0x61 };
            var count128 = 128;
            var count128Bytes = new byte[] { 0x80 };
            var count199 = 199;
            var count199Bytes = new byte[] { 0xC7 };
            var count255 = 255;
            var count255Bytes = new byte[] { 0xFF };

            Assert.Equal(count0, DataPointTranslator.Instance.FromDataPoint(dptType, count0Bytes));
            Assert.Equal(count97, DataPointTranslator.Instance.FromDataPoint(dptType, count97Bytes));
            Assert.Equal(count128, DataPointTranslator.Instance.FromDataPoint(dptType, count128Bytes));
            Assert.Equal(count199, DataPointTranslator.Instance.FromDataPoint(dptType, count199Bytes));
            Assert.Equal(count255, DataPointTranslator.Instance.FromDataPoint(dptType, count255Bytes));

            Assert.Equal(count0Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count0));
            Assert.Equal(count97Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count97));
            Assert.Equal(count128Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count128));
            Assert.Equal(count199Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count199));
            Assert.Equal(count255Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count255));
        }
    }
}
