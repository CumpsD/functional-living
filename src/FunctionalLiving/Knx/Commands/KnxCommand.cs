namespace FunctionalLiving.Knx.Commands
{
    using Addressing;

    public class KnxCommand
    {
        public KnxGroupAddress Group { get; }

        public byte[] State { get; }

        public KnxCommand(
            KnxGroupAddress group,
            byte[] state)
        {
            Group = group;
            State = state;
        }
    }
}
