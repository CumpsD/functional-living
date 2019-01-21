namespace KNXLibTests.Unit.DataPoint
{
    using FunctionalLiving.Knx.DPT;
    using Xunit;
    using Xunit.Categories;

    public class DataPoint2ByteFloatTemperature
    {
        [Category("KNXLib.Unit.DataPoint.2ByteFloat"), Fact]
        public void DataPoint2ByteFloatTemperatureTest()
        {
            var dptType = "9.001";

            var temp30N = -30f;
            var temp30NBytes = new byte[] { 0x8A, 0x24 };
            var temp23Float = 23.0f;
            var temp23Bytes = new byte[] { 0x0C, 0x7E };
            var temp19Float = 19.5f;
            var temp19Bytes = new byte[] { 0x07, 0x9E };
            var temp5Float = 5.8f;
            var temp5Bytes = new byte[] { 0x02, 0x44 };
            var tempMinus6Float = -6.5f;
            var tempMinus6Bytes = new byte[] { 0x85, 0x76 };
            var temp36Float = 36.7f;
            var temp36Bytes = new byte[] { 0x0F, 0x2B };
            var temp0Float = 0f;
            var temp0Bytes = new byte[] { 0x00, 0x00 };

            Assert.Equal(temp30N, DataPointTranslator.Instance.FromDataPoint(dptType, temp30NBytes));
            Assert.Equal(temp23Float, DataPointTranslator.Instance.FromDataPoint(dptType, temp23Bytes));
            Assert.Equal(temp19Float, DataPointTranslator.Instance.FromDataPoint(dptType, temp19Bytes));
            Assert.Equal(temp5Float, DataPointTranslator.Instance.FromDataPoint(dptType, temp5Bytes));
            Assert.Equal(tempMinus6Float, DataPointTranslator.Instance.FromDataPoint(dptType, tempMinus6Bytes));
            Assert.Equal(temp36Float, DataPointTranslator.Instance.FromDataPoint(dptType, temp36Bytes));
            Assert.Equal(temp0Float, DataPointTranslator.Instance.FromDataPoint(dptType, temp0Bytes));

            temp30NBytes = new byte[] { 00, temp30NBytes[0], temp30NBytes[1] };
            temp23Bytes = new byte[] { 00, temp23Bytes[0], temp23Bytes[1] };
            temp19Bytes = new byte[] { 00, temp19Bytes[0], temp19Bytes[1] };
            temp5Bytes = new byte[] { 00, temp5Bytes[0], temp5Bytes[1] };
            tempMinus6Bytes = new byte[] { 00, tempMinus6Bytes[0], tempMinus6Bytes[1] };
            temp36Bytes = new byte[] { 00, temp36Bytes[0], temp36Bytes[1] };
            temp0Bytes = new byte[] { 00, temp0Bytes[0], temp0Bytes[1] };

            Assert.Equal(temp30NBytes, DataPointTranslator.Instance.ToDataPoint(dptType, temp30N));
            Assert.Equal(temp23Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, temp23Float));
            Assert.Equal(temp19Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, temp19Float));
            Assert.Equal(temp5Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, temp5Float));
            Assert.Equal(tempMinus6Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, tempMinus6Float));
            Assert.Equal(temp36Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, temp36Float));
            Assert.Equal(temp0Bytes, DataPointTranslator.Instance.ToDataPoint(dptType, temp0Float));
        }
    }
}
