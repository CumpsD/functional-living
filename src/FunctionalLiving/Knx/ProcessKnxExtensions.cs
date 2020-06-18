namespace FunctionalLiving.Knx
{
    using System;
    using System.Collections.Generic;
    using Addressing;
    using Parser;

    public static class ProcessKnxExtensions
    {
        public static void ProcessKnxMessage(
            this IDictionary<KnxGroupAddress, string> groupAddresses,
            KnxGroupAddress groupAddress,
            Action<string> processAction)
        {
            if (!groupAddresses.TryGetValue(groupAddress, out var description))
                return;

            processAction(description);
        }

        public static void ProcessKnxSingleBit(
            this IDictionary<KnxGroupAddress, string> groupAddresses,
            KnxGroupAddress groupAddress,
            byte[] state,
            Action<string, bool?> processAction)
        {
            groupAddresses.ProcessKnxMessage(
                groupAddress,
                description =>
                {
                    var functionalToggle = Category1_SingleBit.parseSingleBit(state[0]);
                    var value = functionalToggle.Exists()
                        ? functionalToggle.Value.IsOn
                        : (bool?) null;

                    processAction(description, value);
                });
        }

        public static void ProcessKnxScaling(
            this IDictionary<KnxGroupAddress, string> groupAddresses,
            KnxGroupAddress groupAddress,
            byte[] state,
            Action<string, double> processAction)
        {
            groupAddresses.ProcessKnxMessage(
                groupAddress,
                description => processAction(description, Category5_Scaling.parseScaling(0, 100, state[0])));
        }

        public static void ProcessKnx2ByteUnsignedValue(
            this IDictionary<KnxGroupAddress, string> groupAddresses,
            KnxGroupAddress groupAddress,
            byte[] state,
            Action<string, double> processAction)
        {
            groupAddresses.ProcessKnxMessage(
                groupAddress,
                description => processAction(description, Category7_2ByteUnsignedValue.parseTwoByteUnsigned(1, state[0], state[1])));
        }

        public static void ProcessKnx2ByteFloatValue(
            this IDictionary<KnxGroupAddress, string> groupAddresses,
            KnxGroupAddress groupAddress,
            byte[] state,
            Action<string, double> processAction)
        {
            groupAddresses.ProcessKnxMessage(
                groupAddress,
                description => processAction(description, Category9_2ByteFloatValue.parseTwoByteFloat(state[0], state[1])));
        }

        public static void ProcessKnxTime(
            this IDictionary<KnxGroupAddress, string> groupAddresses,
            KnxGroupAddress groupAddress,
            byte[] state,
            Action<string, Parser.Domain.Day?, TimeSpan> processAction)
        {
            groupAddresses.ProcessKnxMessage(
                groupAddress,
                description =>
                {
                    var (day, timeSpan) = Category10_Time.parseTime(state[0], state[1], state[2]);
                    
                    processAction(description, day.Exists() ? day.Value : null, timeSpan);
                });
        }

        public static void ProcessKnx4ByteSignedValue(
            this IDictionary<KnxGroupAddress, string> groupAddresses,
            KnxGroupAddress groupAddress,
            byte[] state,
            Action<string, long> processAction)
        {
            groupAddresses.ProcessKnxMessage(
                groupAddress,
                description => processAction(description, Category13_4ByteSignedValue.parseFourByteSigned(state[0], state[1], state[2], state[3])));
        }

        public static void ProcessKnxDate(
            this IDictionary<KnxGroupAddress, string> groupAddresses,
            KnxGroupAddress groupAddress,
            byte[] state,
            Action<string, DateTime> processAction)
        {
            groupAddresses.ProcessKnxMessage(
                groupAddress,
                description => processAction(description, Category11_Date.parseDate(state[0], state[1], state[2])));
        }
    }
}
