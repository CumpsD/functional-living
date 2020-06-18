namespace FunctionalLiving.Measurements
{
    using System;
    using InfluxDB.Client.Core;

    [Measurement("lux")]
    public class Lux
    {
        [Column("location", IsTag = true)]
        public string Location { get; set; }

        [Column("value")]
        public double Value { get; set; }

        [Column(IsTimestamp = true)]
        public DateTime Time { get; set; } = DateTime.UtcNow;
    }
}
