namespace KNXLibTests.Unit.DataPoint
{
    using FunctionalLiving.Knx.DPT;
    using Xunit;
    using Xunit.Categories;

    public class DataPoint8BitNoSignScaledScaling
    {
        [Category("KNXLib.Unit.DataPoint.8BitNoSign"), Fact]
        public void DataPoint8BitNoSignScaledScalingTest()
        {
            var dptType = "5.001";

            var scale0 = 0;
            var scale0Bytes = new byte[] { 0x00, 0x00 };
            var scale20 = 20;
            var scale20Bytes = new byte[] { 0x00, 0x33 };
            var scale60 = 60;
            var scale60Bytes = new byte[] { 0x00, 0x99 };
            var scale80 = 80;
            var scale80Bytes = new byte[] { 0x00, 0xCC };
            var scale100 = 100;
            var scale100Bytes = new byte[] { 0x00, 0xFF };

            Assert.Equal(scale0, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, scale0Bytes)));
            Assert.Equal(scale20, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, scale20Bytes)));
            Assert.Equal(scale60, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, scale60Bytes)));
            Assert.Equal(scale80, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, scale80Bytes)));
            Assert.Equal(scale100, ((int) (decimal) DataPointTranslator.Instance.FromDataPoint(dptType, scale100Bytes)));

            Assert.Equal(scale0Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, scale0));
            Assert.Equal(scale20Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, scale20));
            Assert.Equal(scale60Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, scale60));
            Assert.Equal(scale80Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, scale80));
            Assert.Equal(scale100Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, scale100));
        }
    }
}
