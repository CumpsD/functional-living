namespace FunctionalLiving.Knx.DPT
{
    using System.Globalization;
    using Log;

    internal sealed class DataPoint8BitNoSignScaledAngle : DataPoint
    {
        public override string[] Ids => new[] { "5.003" };

        public override object FromDataPoint(string data)
        {
            var dataConverted = new byte[data.Length];
            for (var i = 0; i < data.Length; i++)
                dataConverted[i] = (byte) data[i];

            return FromDataPoint(dataConverted);
        }

        public override object FromDataPoint(byte[] data)
        {
            if (data == null || data.Length != 1)
                return 0;

            var value = (int) data[0];

            decimal result = value * 360;
            result = result / 255;

            return result;
        }

        public override byte[] ToDataPoint(string value) => ToDataPoint(float.Parse(value, CultureInfo.InvariantCulture));

        public override byte[] ToDataPoint(object val)
        {
            var dataPoint = new byte[1];
            dataPoint[0] = 0x00;

            decimal input;
            switch (val)
            {
                case int i:
                    input = i;
                    break;

                case float f:
                    input = (decimal) f;
                    break;

                case long l:
                    input = l;
                    break;

                case double d:
                    input = (decimal) d;
                    break;

                case decimal dec:
                    input = dec;
                    break;

                default:
                    Logger.Error("5.003", "input value received is not a valid type");
                    return dataPoint;
            }

            if (input < 0 || input > 360)
            {
                Logger.Error("5.003", "input value received is not in a valid range");
                return dataPoint;
            }

            input = input * 255;
            input = input / 360;

            dataPoint[0] = (byte) (int) input;

            return dataPoint;
        }
    }
}
