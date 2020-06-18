namespace FunctionalLiving.Knx
{
    using System;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Commands;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Parser;
    using static GroupAddresses;

    public sealed class KnxCommandHandlerModule : CommandHandlerModule
    {
        public KnxCommandHandlerModule(
            ILogger<KnxCommandHandlerModule> logger,
            ILogger<KnxCommand> knxCommandLogger)
        {
            For<KnxCommand>()
                .AddLogging(logger)
                .Handle(async (message, ct) =>
                {
                    var groupAddress = message.Command.Group.ToString(); // e.g. 1/0/1
                    var state = message.Command.State;

                    Log(knxCommandLogger, groupAddress!, state);
                });
        }

        private static void Log(
            ILogger logger,
            string groupAddress,
            byte[] state)
        {
            if (Switches.TryGetValue(groupAddress, out var description))
            {
                var functionalToggle = Category1_SingleBit.parseSingleBit(state[0]);
                Log(logger, groupAddress, "ON/OFF", description, functionalToggle.Exists() ? functionalToggle.Value.Text : "N/A");
            }
            else if (Toggles.TryGetValue(groupAddress, out description))
            {
                var functionalToggle = Category1_SingleBit.parseSingleBit(state[0]);
                Log(
                    logger,
                    groupAddress,
                    "TRUE/FALSE",
                    description,
                    functionalToggle.Exists()
                        ? functionalToggle.Value.IsOff
                            ? "False"
                            : functionalToggle.Value.IsOn
                                ? "True"
                                : "N/A"
                        : "N/A");
            }
            else if (Percentages.TryGetValue(groupAddress, out description))
            {
                var functionalPercentage = Category5_Scaling.parseScaling(0, 100, state[0]);
                Log(logger, groupAddress, "PERCENTAGE", description, $"{functionalPercentage} %");
            }
            else if (Duration.TryGetValue(groupAddress, out description))
            {
                var functionalDuration = Category7_2ByteUnsignedValue.parseTwoByteUnsigned(1, state[0], state[1]);
                Log(logger, groupAddress, "DURATION", description, $"{functionalDuration} h");
            }
            else if (Current.TryGetValue(groupAddress, out description))
            {
                var functionalCurrent = Category7_2ByteUnsignedValue.parseTwoByteUnsigned(1, state[0], state[1]);
                Log(logger, groupAddress, "ENERGY", description, $"{functionalCurrent} mA");
            }
            else if (Temperatures.TryGetValue(groupAddress, out description))
            {
                var functionalTemp = Category9_2ByteFloatValue.parseTwoByteFloat(state[0], state[1]);
                Log(logger, groupAddress, "TEMP", description, $"{functionalTemp} °C");
            }
            else if (LightStrength.TryGetValue(groupAddress, out description))
            {
                var functionalLightStrength = Category9_2ByteFloatValue.parseTwoByteFloat(state[0], state[1]);
                Log(logger, groupAddress, "LUX", description, $"{functionalLightStrength} Lux");
            }
            else if (Times.TryGetValue(groupAddress, out description))
            {
                var functionalTime = Category10_Time.parseTime(state[0], state[1], state[2]);
                Log(logger, groupAddress, "TIME", description, $"{(functionalTime.Item1.Exists() ? functionalTime.Item1.Value.Text : string.Empty)}, {functionalTime.Item2:c}");
            }
            else if (EnergyWattHour.TryGetValue(groupAddress, out description))
            {
                var functionalWattHour = Category13_4ByteSignedValue.parseFourByteSigned(state[0], state[1], state[2], state[3]);
                Log(logger, groupAddress, "ENERGY", description, $"{functionalWattHour} Wh");
            }
            else if (Dates.TryGetValue(groupAddress, out description))
            {
                var functionalDate = Category11_Date.parseDate(state[0], state[1], state[2]);
                Log(logger, groupAddress, "DATE", description, $"{functionalDate:dd/MM/yyyy}");
            }
            else if (Speed.TryGetValue(groupAddress, out description))
            {
                var functionalSpeed = Category9_2ByteFloatValue.parseTwoByteFloat(state[0], state[1]);
                Log(logger, groupAddress, "SPEED", description, $"{functionalSpeed} m/s");
            }
            else
            {
                logger.LogWarning(
                    "Unknown Address '{GroupAddress}'! Received '{State}'.",
                    groupAddress,
                    BitConverter.ToString(state));
            }
        }

        private static void Log(
            ILogger logger,
            string groupAddress,
            string dataType,
            string description,
            string value)
        {
            var feedbackAddress = groupAddress.StartsWith("0/") || groupAddress.StartsWith("1/");

            logger.LogInformation(
                "[{Type}] [{DataType}] {Description} ({Value})",
                feedbackAddress ? "FDBCK" : "CNTRL",
                dataType,
                description,
                value);
        }
    }
}
