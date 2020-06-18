namespace FunctionalLiving.Knx.Domain
{
    using Addressing;

    public class KnxObject
    {
        public KnxControlType KnxControlType { get; }

        public KnxDataType KnxDataType { get; }

        public KnxGroupAddress Address { get; }

        public string Description { get; }

        public KnxObject(
            KnxControlType controlType,
            KnxDataType dataType,
            KnxGroupAddress address,
            string description)
        {
            KnxControlType = controlType;
            KnxDataType = dataType;
            Address = address;
            Description = description;
        }
    }
}
