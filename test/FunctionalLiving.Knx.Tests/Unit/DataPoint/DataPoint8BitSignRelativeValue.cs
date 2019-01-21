namespace KNXLibTests.Unit.DataPoint
{
    using FunctionalLiving.Knx.DPT;
    using Xunit;
    using Xunit.Categories;

    public class DataPoint8BitSignRelativeValue
    {
        [Category("KNXLib.Unit.DataPoint.8BitSign"), Fact]
        public void DataPoint8BitSignRelativeValuePercentTest()
        {
            var dptType = "6.001";

            var perc128N = -128;
            var perc128NBytes = new byte[] { 0x80 };
            var perc1N = -1;
            var perc1NBytes = new byte[] { 0xFF };
            var perc0 = 0;
            var perc0Bytes = new byte[] { 0x00 };
            var perc55 = 55;
            var perc55Bytes = new byte[] { 0x37 };
            var perc127 = 127;
            var perc127Bytes = new byte[] { 0x7F };

            Assert.Equal(perc128N, DataPointTranslator.Instance.FromDataPoint(dptType, perc128NBytes));
            Assert.Equal(perc1N, DataPointTranslator.Instance.FromDataPoint(dptType, perc1NBytes));
            Assert.Equal(perc0, DataPointTranslator.Instance.FromDataPoint(dptType, perc0Bytes));
            Assert.Equal(perc55, DataPointTranslator.Instance.FromDataPoint(dptType, perc55Bytes));
            Assert.Equal(perc127, DataPointTranslator.Instance.FromDataPoint(dptType, perc127Bytes));

            Assert.Equal(perc128NBytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc128N));
            Assert.Equal(perc1NBytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc1N));
            Assert.Equal(perc0Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc0));
            Assert.Equal(perc55Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc55));
            Assert.Equal(perc127Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, perc127));
        }

        [Category("KNXLib.Unit.DataPoint.8BitSign"), Fact]
        public void DataPoint8BitSignRelativeValueCountTest()
        {
            var dptType = "6.010";

            var count128N = -128;
            var count128NBytes = new byte[] { 0x80 };
            var count1N = -1;
            var count1NBytes = new byte[] { 0xFF };
            var count0 = 0;
            var count0Bytes = new byte[] { 0x00 };
            var count55 = 55;
            var count55Bytes = new byte[] { 0x37 };
            var count127 = 127;
            var count127Bytes = new byte[] { 0x7F };

            Assert.Equal(count128N, DataPointTranslator.Instance.FromDataPoint(dptType, count128NBytes));
            Assert.Equal(count1N, DataPointTranslator.Instance.FromDataPoint(dptType, count1NBytes));
            Assert.Equal(count0, DataPointTranslator.Instance.FromDataPoint(dptType, count0Bytes));
            Assert.Equal(count55, DataPointTranslator.Instance.FromDataPoint(dptType, count55Bytes));
            Assert.Equal(count127, DataPointTranslator.Instance.FromDataPoint(dptType, count127Bytes));

            Assert.Equal(count128NBytes, DataPointTranslator.Instance.ToDataPoint(dptType, count128N));
            Assert.Equal(count1NBytes, DataPointTranslator.Instance.ToDataPoint(dptType, count1N));
            Assert.Equal(count0Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count0));
            Assert.Equal(count55Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count55));
            Assert.Equal(count127Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, count127));
        }
    }
}
