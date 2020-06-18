namespace FunctionalLiving.Measurements
{
    using System;
    using InfluxDB.Client.Core;
    using Knx.Addressing;

    [Measurement("watt_hour")]
    public class WattHour
    {
        [Column("address", IsTag = true)]
        public string Address { get; }

        [Column("location", IsTag = true)]
        public string Location { get; }

        [Column("value")]
        public double Value { get; }

        [Column(IsTimestamp = true)]
        public DateTime Time { get; } = DateTime.UtcNow;

        public WattHour(KnxGroupAddress address, string location, long value)
        {
            Address = address.ToString()!;
            Location = location;
            Value = value;
        }
    }
}
