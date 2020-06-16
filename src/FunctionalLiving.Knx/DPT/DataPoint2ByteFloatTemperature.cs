namespace FunctionalLiving.Knx.DPT
{
    using System;
    using System.Globalization;
    using Microsoft.Extensions.Logging;

    internal sealed class DataPoint2ByteFloatTemperature : DataPoint
    {
        public override string[] Ids => new[] { "9.001", "9.005" };

        public override object FromDataPoint(string data)
        {
            var dataConverted = new byte[data.Length];
            for (var i = 0; i < data.Length; i++)
                dataConverted[i] = (byte) data[i];

            return FromDataPoint(dataConverted);
        }

        public override object FromDataPoint(byte[] data)
        {
            // DPT bits high byte: MEEEEMMM, low byte: MMMMMMMM
            // first M is signed state from two's complement notation

            int val;
            var m = (uint) ((data[0] & 0x07) << 8) | data[1];
            var signed = (data[0] & 0x80) >> 7 == 1;

            if (signed)
            {
                // change for two's complement notation and use only mantissa bytes
                m = m - 1;
                m = ~m;
                m = m & (0 | 0x07FF);
                val = (int) (m * -1);
            }
            else
            {
                val = (int) m;
            }

            var power = (data[0] & 0x78) >> 3;

            var calc = 0.01d * val;

            return (float) Math.Round(calc * Math.Pow(2, power), 2);
        }

        public override byte[] ToDataPoint(string value) => ToDataPoint(float.Parse(value, CultureInfo.InvariantCulture));

        public override byte[] ToDataPoint(object val)
        {
            var dataPoint = new byte[] { 0x00, 0x00, 0x00 };

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
                    // Logger.Error("9.001", "input value received is not a valid type");
                    return dataPoint;
            }

            if (input < -273 || input > +670760)
            {
                // Logger.Error("9.001", "input value received is not in a valid range");
                return dataPoint;
            }

            // value will be multiplied by 0.01
            var v = Math.Round(input * 100m);

            // mantissa only holds 11 bits for value, so, check if exponet is required
            var e = 0;
            while (v < -2048m)
            {
                v = v / 2;
                e++;
            }
            while (v > 2047m)
            {
                v = v / 2;
                e++;
            }

            int mantissa;
            bool signed;
            if (v < 0)
            {
                // negative value > two's complement
                signed = true;
                mantissa = (int) v * -1;
                mantissa = ~mantissa;
                mantissa = mantissa + 1;
            }
            else
            {
                signed = false;
                mantissa = (int) v;
            }

            // signed value > enable first bit
            if (signed)
                dataPoint[1] = 0x80;

            dataPoint[1] = (byte) (dataPoint[1] | ((e & 0x0F) << 3));
            dataPoint[1] = (byte) (dataPoint[1] | ((mantissa >> 8) & 0x07));
            dataPoint[2] = (byte) mantissa;

            return dataPoint;
        }
    }
}
