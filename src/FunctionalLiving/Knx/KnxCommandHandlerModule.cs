namespace FunctionalLiving.Knx
{
    using System;
    using System.Threading.Tasks;
    using Addressing;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Commands;
    using FunctionalLiving.Domain.Knx;
    using InfluxDB.Client;
    using InfluxDB.Client.Api.Domain;
    using Infrastructure;
    using Infrastructure.Toggles;
    using Measurements;
    using Microsoft.Extensions.Logging;
    using static Domain.Knx.FeedbackGroupAddresses;

    public sealed class KnxCommandHandlerModule : CommandHandlerModule
    {
        private readonly ILogger<KnxCommand> _knxCommandLogger;
        private readonly Func<WriteApi> _influxWrite;
        private readonly IKnxHub _knxHub;

        private readonly SendToInflux _sendToInflux;
        private readonly SendToLog _sendToLog;
        private readonly SendToSignalR _sendToSignalR;

        public KnxCommandHandlerModule(
            ILogger<KnxCommandHandlerModule> logger,
            ILogger<KnxCommand> knxCommandLogger,
            Func<WriteApi> influxWrite,
            IKnxHub knxHub,
            SendToInflux sendToInflux,
            SendToLog sendToLog,
            SendToSignalR sendToSignalR)
        {
            _knxCommandLogger = knxCommandLogger;
            _influxWrite = influxWrite;
            _knxHub = knxHub;

            _sendToInflux = sendToInflux;
            _sendToLog = sendToLog;
            _sendToSignalR = sendToSignalR;

            For<KnxCommand>()
                .AddLogging(logger)
                .Handle(async (message, ct) =>
                {
                    var groupAddress = message.Command.Group; // e.g. 1/0/1
                    var state = message.Command.State;

                    await ProcessKnxMessageBasedOnDataType(groupAddress, state);
                });
        }

        private async Task ProcessKnxMessageBasedOnDataType(
            KnxGroupAddress groupAddress,
            byte[] state)
        {
            await Switches.ProcessKnxSingleBit(
                groupAddress,
                state,
                async (description, value) =>
                {
                    SendToInflux(new Toggle(groupAddress, description, value));
                    SendToLog(groupAddress, "ON/OFF", description, value);
                    await SendToSignalR(groupAddress, "ON/OFF", description, value);
                });

            await Toggles.ProcessKnxSingleBit(
                groupAddress,
                state,
                async (description, value) =>
                {
                    SendToInflux(new Toggle(groupAddress, description, value));
                    SendToLog(groupAddress, "TRUE/FALSE", description, value);
                    await SendToSignalR(groupAddress, "TRUE/FALSE", description, value);
                });

            await Percentages.ProcessKnxScaling(
                groupAddress,
                state,
                async (description, value) =>
                {
                    SendToLog(groupAddress, "PERCENTAGE", description, $"{value} %");
                    await SendToSignalR(groupAddress, "PERCENTAGE", description, $"{value} %");
                });

            await FeedbackGroupAddresses.Duration.ProcessKnx2ByteUnsignedValue(
                groupAddress,
                state,
                async (description, value) =>
                {
                    SendToLog(groupAddress, "DURATION", description, $"{value} h");
                    await SendToSignalR(groupAddress, "DURATION", description, $"{value} h");
                });

            await Current.ProcessKnx2ByteUnsignedValue(
                groupAddress,
                state,
                async (description, value) =>
                {
                    SendToInflux(new MilliAmpere(groupAddress, description, value));
                    SendToLog(groupAddress, "ENERGY", description, $"{value} mA");
                    await SendToSignalR(groupAddress, "ENERGY", description, $"{value} mA");
                });

            await Temperatures.ProcessKnx2ByteFloatValue(
                groupAddress,
                state,
                async (description, value) =>
                {
                    SendToInflux(new TemperatureCelcius(groupAddress, description, value));
                    SendToLog(groupAddress, "TEMP", description, $"{value} °C");
                    await SendToSignalR(groupAddress, "TEMP", description, $"{value} °C");
                });

            await LightStrength.ProcessKnx2ByteFloatValue(
                groupAddress,
                state,
                async (description, value) =>
                {
                    SendToInflux(new Lux(groupAddress, description, value));
                    SendToLog(groupAddress, "LUX", description, $"{value} Lux");
                    await SendToSignalR(groupAddress, "LUX", description, $"{value} Lux");
                });

            await Times.ProcessKnxTime(
                groupAddress,
                state,
                async (description, day, time) =>
                {
                    SendToLog(groupAddress, "TIME", description, $"{day?.Text}, {time:c}");
                    await SendToSignalR(groupAddress, "TIME", description, $"{day?.Text}, {time:c}");
                });

            await EnergyWattHour.ProcessKnx4ByteSignedValue(
                groupAddress,
                state,
                async (description, value) =>
                {
                    SendToInflux(new WattHour(groupAddress, description, value));
                    SendToLog(groupAddress, "ENERGY", description, $"{value} Wh");
                    await SendToSignalR(groupAddress, "ENERGY", description, $"{value} Wh");
                });

            await Dates.ProcessKnxDate(
                groupAddress,
                state,
                async (description, value) =>
                {
                    SendToLog(groupAddress, "DATE", description, $"{value:dd/MM/yyyy}");
                    await SendToSignalR(groupAddress, "DATE", description, $"{value:dd/MM/yyyy}");
                });

            await Speed.ProcessKnx2ByteFloatValue(
                groupAddress,
                state,
                async (description, value) =>
                {
                    SendToInflux(new MeterPerSecond(groupAddress, description, value));
                    SendToLog(groupAddress, "SPEED", description, $"{value} m/s");
                    await SendToSignalR(groupAddress, "SPEED", description, $"{value} m/s");
                });
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

        private async Task SendToSignalR<T>(
            KnxGroupAddress groupAddress,
            string dataType,
            string description,
            T value)
        {
            if (!_sendToSignalR.FeatureEnabled)
                return;

            var groupAddressString = groupAddress.ToString()!;
            var feedbackAddress = groupAddressString.StartsWith("0/") || groupAddressString.StartsWith("1/");

            await _knxHub.SendKnxMessage(
                $"[{(feedbackAddress ? "FDBCK" : "CNTRL")}] [{dataType}] {description} ({value})");
        }
    }
}
