namespace FunctionalLiving.Knx
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Addressing;
    using Parser;

    public static class ProcessKnxExtensions
    {
        public static async Task ProcessKnxMessageAsync<T>(
            this IDictionary<KnxGroupAddress, T> groupAddresses,
            KnxGroupAddress groupAddress,
            Func<T, Task> processAction)
        {
            if (!groupAddresses.TryGetValue(groupAddress, out var o))
                return;

            await processAction(o);
        }

        public static async Task ProcessKnxSingleBit<T>(
            this IDictionary<KnxGroupAddress, T> groupAddresses,
            KnxGroupAddress groupAddress,
            byte[] state,
            Func<T, bool?, Task> processAction)
        {
            await groupAddresses.ProcessKnxMessageAsync(
                groupAddress,
                async o =>
                {
                    var functionalToggle = Category1_SingleBit.parseSingleBit(state[0]);
                    var value = functionalToggle.Exists()
                        ? functionalToggle.Value.IsOn
                        : (bool?) null;

                    await processAction(o, value);
                });
        }

        public static async Task ProcessKnxScaling<T>(
            this IDictionary<KnxGroupAddress, T> groupAddresses,
            KnxGroupAddress groupAddress,
            byte[] state,
            Func<T, double, Task> processAction)
        {
            await groupAddresses.ProcessKnxMessageAsync(
                groupAddress,
                async o => await processAction(o, Category5_Scaling.parseScaling(0, 100, state[0])));
        }

        public static async Task ProcessKnx2ByteUnsignedValue<T>(
            this IDictionary<KnxGroupAddress, T> groupAddresses,
            KnxGroupAddress groupAddress,
            byte[] state,
            Func<T, double, Task> processAction)
        {
            await groupAddresses.ProcessKnxMessageAsync(
                groupAddress,
                async o => await processAction(o, Category7_2ByteUnsignedValue.parseTwoByteUnsigned(1, state[0], state[1])));
        }

        public static async Task ProcessKnx2ByteFloatValue<T>(
            this IDictionary<KnxGroupAddress, T> groupAddresses,
            KnxGroupAddress groupAddress,
            byte[] state,
            Func<T, double, Task> processAction)
        {
            await groupAddresses.ProcessKnxMessageAsync(
                groupAddress,
                async o => await processAction(o, Category9_2ByteFloatValue.parseTwoByteFloat(state[0], state[1])));
        }

        public static async Task ProcessKnxTime<T>(
            this IDictionary<KnxGroupAddress, T> groupAddresses,
            KnxGroupAddress groupAddress,
            byte[] state,
            Func<T, Domain.Day?, TimeSpan, Task> processAction)
        {
            await groupAddresses.ProcessKnxMessageAsync(
                groupAddress,
                async o =>
                {
                    var (day, timeSpan) = Category10_Time.parseTime(state[0], state[1], state[2]);
                    
                    await processAction(o, day.Exists() ? day.Value : null, timeSpan);
                });
        }

        public static async Task ProcessKnx4ByteSignedValue<T>(
            this IDictionary<KnxGroupAddress, T> groupAddresses,
            KnxGroupAddress groupAddress,
            byte[] state,
            Func<T, long, Task> processAction)
        {
            await groupAddresses.ProcessKnxMessageAsync(
                groupAddress,
                async o => await processAction(o, Category13_4ByteSignedValue.parseFourByteSigned(state[0], state[1], state[2], state[3])));
        }

        public static async Task ProcessKnxDate<T>(
            this IDictionary<KnxGroupAddress, T> groupAddresses,
            KnxGroupAddress groupAddress,
            byte[] state,
            Func<T, DateTime, Task> processAction)
        {
            await groupAddresses.ProcessKnxMessageAsync(
                groupAddress,
                async o => await processAction(o, Category11_Date.parseDate(state[0], state[1], state[2])));
        }
    }
}
