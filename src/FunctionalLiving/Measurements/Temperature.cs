namespace FunctionalLiving.Measurements
{
    using System;
    using InfluxDB.Client.Core;

    [Measurement("temperature")]
    public class Temperature
    {
        [Column("location", IsTag = true)]
        public string Location { get; set; }

        [Column("value")]
        public double Value { get; set; }

        [Column(IsTimestamp = true)]
        public DateTime Time;
    }
}
