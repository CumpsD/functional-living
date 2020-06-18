namespace FunctionalLiving.Knx
{
    using System;
    using Addressing;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Commands;
    using Domain;
    using InfluxDB.Client;
    using InfluxDB.Client.Api.Domain;
    using Infrastructure;
    using Infrastructure.Toggles;
    using Measurements;
    using Microsoft.Extensions.Logging;
    using static Domain.GroupAddresses;

    public sealed class KnxCommandHandlerModule : CommandHandlerModule
    {
        private readonly ILogger<KnxCommand> _knxCommandLogger;
        private readonly Func<WriteApi> _influxWrite;
        private readonly SendToLog _sendToLog;
        private readonly SendToInflux _sendToInflux;

        public KnxCommandHandlerModule(
            ILogger<KnxCommandHandlerModule> logger,
            ILogger<KnxCommand> knxCommandLogger,
            Func<WriteApi> influxWrite,
            SendToLog sendToLog,
            SendToInflux sendToInflux)
        {
            _knxCommandLogger = knxCommandLogger;
            _influxWrite = influxWrite;
            _sendToLog = sendToLog;
            _sendToInflux = sendToInflux;

            For<KnxCommand>()
                .AddLogging(logger)
                .Handle(async (message, ct) =>
                {
                    var groupAddress = message.Command.Group; // e.g. 1/0/1
                    var state = message.Command.State;

                    Switches.ProcessKnxSingleBit(
                        groupAddress,
                        state,
                        (description, value) =>
                        {
                            SendToLog(groupAddress, "ON/OFF", description, value);
                            SendToInflux(new Toggle(groupAddress, description, value));
                        });

                    Toggles.ProcessKnxSingleBit(
                        groupAddress,
                        state,
                        (description, value) =>
                        {
                            SendToLog(groupAddress, "TRUE/FALSE", description, value);
                            SendToInflux(new Toggle(groupAddress, description, value));
                        });

                    Percentages.ProcessKnxScaling(
                        groupAddress,
                        state,
                        (description, value) => SendToLog(groupAddress, "PERCENTAGE", description, $"{value} %"));

                    GroupAddresses.Duration.ProcessKnx2ByteUnsignedValue(
                        groupAddress,
                        state,
                        (description, value) => SendToLog(groupAddress, "DURATION", description, $"{value} h"));

                    Current.ProcessKnx2ByteUnsignedValue(
                        groupAddress,
                        state,
                        (description, value) =>
                        {
                            SendToLog(groupAddress, "ENERGY", description, $"{value} mA");
                            SendToInflux(new MilliAmpere(groupAddress, description, value));
                        });

                    Temperatures.ProcessKnx2ByteFloatValue(
                        groupAddress,
                        state,
                        (description, value) =>
                        {
                            SendToLog(groupAddress, "TEMP", description, $"{value} °C");
                            SendToInflux(new TemperatureCelcius(groupAddress, description, value));
                        });

                    LightStrength.ProcessKnx2ByteFloatValue(
                        groupAddress,
                        state,
                        (description, value) =>
                        {
                            SendToLog(groupAddress, "LUX", description, $"{value} Lux");
                            SendToInflux(new Lux(groupAddress, description, value));
                        });

                    Times.ProcessKnxTime(
                        groupAddress,
                        state,
                        (description, day, time) => SendToLog(groupAddress, "TIME", description, $"{day?.Text}, {time:c}"));

                    EnergyWattHour.ProcessKnx4ByteSignedValue(
                        groupAddress,
                        state,
                        (description, value) =>
                        {
                            SendToLog(groupAddress, "ENERGY", description, $"{value} Wh");
                            SendToInflux(new WattHour(groupAddress, description, value));
                        });

                    Dates.ProcessKnxDate(
                        groupAddress,
                        state,
                        (description, value) => SendToLog(groupAddress, "DATE", description, $"{value:dd/MM/yyyy}"));

                    GroupAddresses.Speed.ProcessKnx2ByteFloatValue(
                        groupAddress,
                        state,
                        (description, value) =>
                        {
                            SendToLog(groupAddress, "SPEED", description, $"{value} m/s");
                            SendToInflux(new MeterPerSecond(groupAddress, description, value));
                        });
                });
        }

        private void SendToLog<T>(
            KnxGroupAddress groupAddress,
            string dataType,
            string description,
            T value)
        {
            if (!_sendToLog.FeatureEnabled)
                return;

            var groupAddressString = groupAddress.ToString()!;
            var feedbackAddress = groupAddressString.StartsWith("0/") || groupAddressString.StartsWith("1/");

            _knxCommandLogger.LogInformation(
                "[{Type}] [{DataType}] {Description} ({Value})",
                feedbackAddress ? "FDBCK" : "CNTRL",
                dataType,
                description,
                value);
        }

        private void SendToInflux<T>(T measurement)
        {
            if (!_sendToInflux.FeatureEnabled)
                return;

            using (var writeClient = _influxWrite())
            {
                writeClient.WriteMeasurement(
                    WritePrecision.Ns,
                    measurement);

                writeClient.Flush();
            }
        }
    }
}
