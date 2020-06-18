namespace FunctionalLiving.Measurements
{
    using System;
    using InfluxDB.Client.Core;
    using Knx.Addressing;

    [Measurement("boolean")]
    public class Toggle
    {
        [Column("address", IsTag = true)]
        public string Address { get; }

        [Column("location", IsTag = true)]
        public string Location { get; }

        [Column("value")]
        public double Value { get; }

        [Column(IsTimestamp = true)]
        public DateTime Time { get; } = DateTime.UtcNow;

        public Toggle(KnxGroupAddress address, string location, bool? value)
        {
            Address = address.ToString()!;
            Location = location;
            Value = value.HasValue
                ? value.Value
                    ? 1
                    : 0
                : 0;
        }
    }
}
