namespace FunctionalLiving.Knx
{
    using System;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Commands;
    using Infrastructure;
    using Infrastructure.Toggles;
    using Microsoft.Extensions.Logging;
    using InfluxDB.Client;
    using InfluxDB.Client.Writes;
    using InfluxDB.Client.Api.Domain;
    using Parser;
    using static GroupAddresses;

    public sealed class KnxCommandHandlerModule : CommandHandlerModule
    {
        public KnxCommandHandlerModule(
            ILogger<KnxCommandHandlerModule> logger,
            ILogger<KnxCommand> knxCommandLogger,
            Func<WriteApi> influxWrite,
            SendToInflux sendToInflux)
        {
            For<KnxCommand>()
                .AddLogging(logger)
                .Handle(async (message, ct) =>
                {
                    var groupAddress = message.Command.Group.ToString(); // e.g. 1/0/1
                    var state = message.Command.State;

                    if (Switches.TryGetValue(groupAddress, out var description))
                    {
                        var functionalToggle = Category1_SingleBit.parseSingleBit(state[0]);
                        var value = functionalToggle.Exists()
                            ? functionalToggle.Value.Text
                            : "N/A";

                        Log(
                            knxCommandLogger,
                            groupAddress,
                            "ON/OFF",
                            description,
                            value);
                    }
                    else if (Toggles.TryGetValue(groupAddress, out description))
                    {
                        var functionalToggle = Category1_SingleBit.parseSingleBit(state[0]);
                        var value = functionalToggle.Exists()
                            ? functionalToggle.Value.IsOff
                                ? "False"
                                : functionalToggle.Value.IsOn
                                    ? "True"
                                    : "N/A"
                            : "N/A";

                        Log(
                            knxCommandLogger,
                            groupAddress,
                            "TRUE/FALSE",
                            description,
                            value);
                    }
                    else if (Percentages.TryGetValue(groupAddress, out description))
                    {
                        var functionalPercentage = Category5_Scaling.parseScaling(0, 100, state[0]);
                        var value = $"{functionalPercentage} %";

                        Log(
                            knxCommandLogger,
                            groupAddress,
                            "PERCENTAGE",
                            description,
                            value);
                    }
                    else if (GroupAddresses.Duration.TryGetValue(groupAddress, out description))
                    {
                        var functionalDuration = Category7_2ByteUnsignedValue.parseTwoByteUnsigned(1, state[0], state[1]);
                        var value = $"{functionalDuration} h";

                        Log(
                            knxCommandLogger,
                            groupAddress,
                            "DURATION",
                            description,
                            value);
                    }
                    else if (Current.TryGetValue(groupAddress, out description))
                    {
                        var functionalCurrent = Category7_2ByteUnsignedValue.parseTwoByteUnsigned(1, state[0], state[1]);
                        var value = $"{functionalCurrent} mA";

                        Log(
                            knxCommandLogger,
                            groupAddress,
                            "ENERGY",
                            description,
                            value);
                    }
                    else if (Temperatures.TryGetValue(groupAddress, out description))
                    {
                        var functionalTemp = Category9_2ByteFloatValue.parseTwoByteFloat(state[0], state[1]);
                        var value = $"{functionalTemp} °C";

                        Log(
                            knxCommandLogger,
                            groupAddress,
                            "TEMP",
                            description,
                            value);

                        if (sendToInflux.FeatureEnabled)
                            WriteTemperature(
                                influxWrite(),
                                description,
                                functionalTemp);
                    }
                    else if (LightStrength.TryGetValue(groupAddress, out description))
                    {
                        var functionalLightStrength = Category9_2ByteFloatValue.parseTwoByteFloat(state[0], state[1]);
                        var value = $"{functionalLightStrength} Lux";

                        Log(
                            knxCommandLogger,
                            groupAddress,
                            "LUX",
                            description,
                            value);
                    }
                    else if (Times.TryGetValue(groupAddress, out description))
                    {
                        var functionalTime = Category10_Time.parseTime(state[0], state[1], state[2]);
                        var value = $"{(functionalTime.Item1.Exists() ? functionalTime.Item1.Value.Text : string.Empty)}, {functionalTime.Item2:c}";

                        Log(
                            knxCommandLogger,
                            groupAddress,
                            "TIME",
                            description,
                            value);
                    }
                    else if (EnergyWattHour.TryGetValue(groupAddress, out description))
                    {
                        var functionalWattHour = Category13_4ByteSignedValue.parseFourByteSigned(state[0], state[1], state[2], state[3]);
                        var value = $"{functionalWattHour} Wh";

                        Log(
                            knxCommandLogger,
                            groupAddress,
                            "ENERGY",
                            description,
                            value);
                    }
                    else if (Dates.TryGetValue(groupAddress, out description))
                    {
                        var functionalDate = Category11_Date.parseDate(state[0], state[1], state[2]);
                        var value = $"{functionalDate:dd/MM/yyyy}";

                        Log(
                            knxCommandLogger,
                            groupAddress,
                            "DATE",
                            description,
                            value);
                    }
                    else if (Speed.TryGetValue(groupAddress, out description))
                    {
                        var functionalSpeed = Category9_2ByteFloatValue.parseTwoByteFloat(state[0], state[1]);
                        var value = $"{functionalSpeed} m/s";

                        Log(
                            knxCommandLogger,
                            groupAddress,
                            "SPEED",
                            description,
                            value);
                    }
                    else
                    {
                        knxCommandLogger.LogWarning(
                            "Unknown Address '{GroupAddress}'! Received '{State}'.",
                            groupAddress,
                            BitConverter.ToString(state));
                    }
                });
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

        private static void WriteTemperature(
            WriteApi writeApi,
            string location,
            double value)
        {
            var point = PointData.Measurement("temperature")
                .Tag("location", location)
                .Field("value", value)
                .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

            // TODO: Get bucket and org from config
            writeApi.WritePoint("functional-living", "cumps", point);
        }
    }
}
