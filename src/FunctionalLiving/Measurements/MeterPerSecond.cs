namespace FunctionalLiving.Measurements
{
    using System;
    using InfluxDB.Client.Core;
    using Knx.Addressing;

    [Measurement("speed")]
    public class MeterPerSecond
    {
        [Column("address", IsTag = true)]
        public string Address { get; }

        [Column("location", IsTag = true)]
        public string Location { get; }

        [Column("value")]
        public double Value { get; }

        [Column(IsTimestamp = true)]
        public DateTime Time { get; } = DateTime.UtcNow;

        public MeterPerSecond(KnxGroupAddress address, string location, double value)
        {
            Address = address.ToString()!;
            Location = location;
            Value = value;
        }
    }
}
