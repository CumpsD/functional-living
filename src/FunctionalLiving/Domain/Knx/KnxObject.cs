namespace FunctionalLiving.Domain.Knx
{
    using FunctionalLiving.Knx.Addressing;

    public class KnxObject
    {
        public KnxControlType KnxControlType { get; }

        public KnxDataType KnxDataType { get; }

        public KnxGroupAddress Address { get; }

        public string Description { get; }

        public KnxGroupAddress? FeedbackAddress { get; }

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
        public KnxObject(
            KnxControlType controlType,
            KnxDataType dataType,
            KnxGroupAddress address,
            string description,
            KnxGroupAddress? feedbackAddress)
            : this(
                controlType,
                dataType,
                address,
                description)
        {
            FeedbackAddress = feedbackAddress;
        }
    }
}
