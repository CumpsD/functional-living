namespace KNXLibTests.Unit.DataPoint
{
    using FunctionalLiving.Knx.DPT;
    using Xunit;
    using Xunit.Categories;

    public class DataPoint3BitControl
    {
        [Category("KNXLib.Unit.DataPoint.3BitControl"), Fact]
        public void DataPoint3BitControlDimmingTest()
        {
            var dptType = "3.007";

            var incr4 = 4;
            var incr4Bytes = new byte[] { 0x0C };
            var incr1 = 1;
            var incr1Bytes = new byte[] { 0x0F };
            var stop = 0;
            var stopBytes = new byte[] { 0x00 };
            var decr3 = -3;
            var decr3Bytes = new byte[] { 0x05 };
            var decr7 = -7;
            var decr7Bytes = new byte[] { 0x01 };

            Assert.Equal(incr4, DataPointTranslator.Instance.FromDataPoint(dptType, incr4Bytes));
            Assert.Equal(incr1, DataPointTranslator.Instance.FromDataPoint(dptType, incr1Bytes));
            Assert.Equal(stop, DataPointTranslator.Instance.FromDataPoint(dptType, stopBytes));
            Assert.Equal(decr3, DataPointTranslator.Instance.FromDataPoint(dptType, decr3Bytes));
            Assert.Equal(decr7, DataPointTranslator.Instance.FromDataPoint(dptType, decr7Bytes));

            Assert.Equal(incr4Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, incr4));
            Assert.Equal(incr1Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, incr1));
            Assert.Equal(stopBytes, DataPointTranslator.Instance.ToDataPoint(dptType, stop));
            Assert.Equal(decr3Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, decr3));
            Assert.Equal(decr7Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, decr7));
        }

        [Category("KNXLib.Unit.DataPoint.3BitControl"), Fact]
        public void DataPoint3BitControlBlindsTest()
        {
            var dptType = "3.008";

            var incr7 = 7;
            var incr7Bytes = new byte[] { 0x09 };
            var incr2 = 2;
            var incr2Bytes = new byte[] { 0x0E };
            var stop = 0;
            var stopBytes = new byte[] { 0x00 };
            var decr5 = -5;
            var decr5Bytes = new byte[] { 0x03 };
            var decr6 = -6;
            var decr6Bytes = new byte[] { 0x02 };

            Assert.Equal(incr7, DataPointTranslator.Instance.FromDataPoint(dptType, incr7Bytes));
            Assert.Equal(incr2, DataPointTranslator.Instance.FromDataPoint(dptType, incr2Bytes));
            Assert.Equal(stop, DataPointTranslator.Instance.FromDataPoint(dptType, stopBytes));
            Assert.Equal(decr5, DataPointTranslator.Instance.FromDataPoint(dptType, decr5Bytes));
            Assert.Equal(decr6, DataPointTranslator.Instance.FromDataPoint(dptType, decr6Bytes));

            Assert.Equal(incr7Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, incr7));
            Assert.Equal(incr2Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, incr2));
            Assert.Equal(stopBytes, DataPointTranslator.Instance.ToDataPoint(dptType, stop));
            Assert.Equal(decr5Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, decr5));
            Assert.Equal(decr6Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, decr6));
        }
    }
}
