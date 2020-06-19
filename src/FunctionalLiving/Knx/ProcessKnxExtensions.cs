namespace FunctionalLiving.Knx
{
    using System;
    using System.Collections.Generic;
    using Addressing;
    using Parser;

    public static class ProcessKnxExtensions
    {
        public static void ProcessKnxMessage<T>(
            this IDictionary<KnxGroupAddress, T> groupAddresses,
            KnxGroupAddress groupAddress,
            Action<T> processAction)
        {
            if (!groupAddresses.TryGetValue(groupAddress, out var o))
                return;

            processAction(o);
        }

        public static void ProcessKnxSingleBit<T>(
            this IDictionary<KnxGroupAddress, T> groupAddresses,
            KnxGroupAddress groupAddress,
            byte[] state,
            Action<T, bool?> processAction)
        {
            groupAddresses.ProcessKnxMessage(
                groupAddress,
                o =>
                {
                    var functionalToggle = Category1_SingleBit.parseSingleBit(state[0]);
                    var value = functionalToggle.Exists()
                        ? functionalToggle.Value.IsOn
                        : (bool?) null;

                    processAction(o, value);
                });
        }

        public static void ProcessKnxScaling<T>(
            this IDictionary<KnxGroupAddress, T> groupAddresses,
            KnxGroupAddress groupAddress,
            byte[] state,
            Action<T, double> processAction)
        {
            groupAddresses.ProcessKnxMessage(
                groupAddress,
                o => processAction(o, Category5_Scaling.parseScaling(0, 100, state[0])));
        }

        public static void ProcessKnx2ByteUnsignedValue<T>(
            this IDictionary<KnxGroupAddress, T> groupAddresses,
            KnxGroupAddress groupAddress,
            byte[] state,
            Action<T, double> processAction)
        {
            groupAddresses.ProcessKnxMessage(
                groupAddress,
                o => processAction(o, Category7_2ByteUnsignedValue.parseTwoByteUnsigned(1, state[0], state[1])));
        }

        public static void ProcessKnx2ByteFloatValue<T>(
            this IDictionary<KnxGroupAddress, T> groupAddresses,
            KnxGroupAddress groupAddress,
            byte[] state,
            Action<T, double> processAction)
        {
            groupAddresses.ProcessKnxMessage(
                groupAddress,
                o => processAction(o, Category9_2ByteFloatValue.parseTwoByteFloat(state[0], state[1])));
        }

        public static void ProcessKnxTime<T>(
            this IDictionary<KnxGroupAddress, T> groupAddresses,
            KnxGroupAddress groupAddress,
            byte[] state,
            Action<T, Domain.Day?, TimeSpan> processAction)
        {
            groupAddresses.ProcessKnxMessage(
                groupAddress,
                o =>
                {
                    var (day, timeSpan) = Category10_Time.parseTime(state[0], state[1], state[2]);
                    
                    processAction(o, day.Exists() ? day.Value : null, timeSpan);
                });
        }

        public static void ProcessKnx4ByteSignedValue<T>(
            this IDictionary<KnxGroupAddress, T> groupAddresses,
            KnxGroupAddress groupAddress,
            byte[] state,
            Action<T, long> processAction)
        {
            groupAddresses.ProcessKnxMessage(
                groupAddress,
                o => processAction(o, Category13_4ByteSignedValue.parseFourByteSigned(state[0], state[1], state[2], state[3])));
        }

        public static void ProcessKnxDate<T>(
            this IDictionary<KnxGroupAddress, T> groupAddresses,
            KnxGroupAddress groupAddress,
            byte[] state,
            Action<T, DateTime> processAction)
        {
            groupAddresses.ProcessKnxMessage(
                groupAddress,
                o => processAction(o, Category11_Date.parseDate(state[0], state[1], state[2])));
        }
    }
}
