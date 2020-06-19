namespace FunctionalLiving.Domain.Knx
{
    using System.Collections.Generic;
    using System.Linq;
    using FunctionalLiving.Knx.Addressing;

    public static class ControlGroupAddresses
    {
        public static readonly IDictionary<KnxGroupAddress, KnxObject> All =
            CustomerData.KnxObjects
                .Where(x => x.KnxControlType == KnxControlType.Control)
                .ToDictionary(x => x.Address, x => x);

        // 1.001 Switches
        public static readonly IDictionary<KnxGroupAddress, string> Switches =
            CustomerData.KnxObjects
                .Where(x => x.KnxDataType == KnxDataType.Switch)
                .Where(x => x.KnxControlType == KnxControlType.Control)
                .ToDictionary(x => x.Address, x => x.Description);

        // 1.002 Toggles (boolean)
        public static readonly IDictionary<KnxGroupAddress, string> Toggles =
            CustomerData.KnxObjects
                .Where(x => x.KnxDataType == KnxDataType.Toggle)
                .Where(x => x.KnxControlType == KnxControlType.Control)
                .ToDictionary(x => x.Address, x => x.Description);

        // 5.001 Percentages (0..100%)
        public static readonly IDictionary<KnxGroupAddress, string> Percentages =
            CustomerData.KnxObjects
                .Where(x => x.KnxDataType == KnxDataType.Percentage)
                .Where(x => x.KnxControlType == KnxControlType.Control)
                .ToDictionary(x => x.Address, x => x.Description);

        // 7.007 Time (h)
        public static readonly IDictionary<KnxGroupAddress, string> Duration =
            CustomerData.KnxObjects
                .Where(x => x.KnxDataType == KnxDataType.Duration)
                .Where(x => x.KnxControlType == KnxControlType.Control)
                .ToDictionary(x => x.Address, x => x.Description);

        // 7.012 Current (mA)
        public static readonly IDictionary<KnxGroupAddress, string> Current =
            CustomerData.KnxObjects
                .Where(x => x.KnxDataType == KnxDataType.Current)
                .Where(x => x.KnxControlType == KnxControlType.Control)
                .ToDictionary(x => x.Address, x => x.Description);

        // 9.001 Temperate (degrees C)
        public static readonly IDictionary<KnxGroupAddress, string> Temperatures =
            CustomerData.KnxObjects
                .Where(x => x.KnxDataType == KnxDataType.Temperature)
                .Where(x => x.KnxControlType == KnxControlType.Control)
                .ToDictionary(x => x.Address, x => x.Description);

        // 9.004 Light (lux)
        public static readonly IDictionary<KnxGroupAddress, string> LightStrength =
            CustomerData.KnxObjects
                .Where(x => x.KnxDataType == KnxDataType.LightStrength)
                .Where(x => x.KnxControlType == KnxControlType.Control)
                .ToDictionary(x => x.Address, x => x.Description);

        // 9.005 Speed (m/s)
        public static readonly IDictionary<KnxGroupAddress, string> Speed =
            CustomerData.KnxObjects
                .Where(x => x.KnxDataType == KnxDataType.Speed)
                .Where(x => x.KnxControlType == KnxControlType.Control)
                .ToDictionary(x => x.Address, x => x.Description);

        // 10.001 Time of day
        public static readonly IDictionary<KnxGroupAddress, string> Times =
            CustomerData.KnxObjects
                .Where(x => x.KnxDataType == KnxDataType.Time)
                .Where(x => x.KnxControlType == KnxControlType.Control)
                .ToDictionary(x => x.Address, x => x.Description);

        // 11.001 Date
        public static readonly IDictionary<KnxGroupAddress, string> Dates =
            CustomerData.KnxObjects
                .Where(x => x.KnxDataType == KnxDataType.Date)
                .Where(x => x.KnxControlType == KnxControlType.Control)
                .ToDictionary(x => x.Address, x => x.Description);

        // 13.010 Energy (Wh)
        public static readonly IDictionary<KnxGroupAddress, string> EnergyWattHour =
            CustomerData.KnxObjects
                .Where(x => x.KnxDataType == KnxDataType.EnergyWattHour)
                .Where(x => x.KnxControlType == KnxControlType.Control)
                .ToDictionary(x => x.Address, x => x.Description);

        // 13.013 Energy (kWh)
        public static readonly IDictionary<KnxGroupAddress, string> EnergyKiloWattHour =
            CustomerData.KnxObjects
                .Where(x => x.KnxDataType == KnxDataType.EnergyKiloWattHour)
                .Where(x => x.KnxControlType == KnxControlType.Control)
                .ToDictionary(x => x.Address, x => x.Description);
    }
}
