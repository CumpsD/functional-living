﻿namespace FunctionalLiving.Knx
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using Addressing;
    using Log;

    internal class KnxSenderTunneling : KnxSender
    {
        private static readonly string ClassName = typeof(KnxSenderTunneling).ToString();

        private UdpClient _udpClient;
        private readonly IPEndPoint _remoteEndpoint;

        private KnxConnectionTunneling KnxConnectionTunneling => (KnxConnectionTunneling) KnxConnection;

        internal KnxSenderTunneling(KnxConnection connection, UdpClient udpClient, IPEndPoint remoteEndpoint)
            : base(connection)
        {
            _udpClient = udpClient;
            _remoteEndpoint = remoteEndpoint;
        }

        public void SetClient(UdpClient client)
            => _udpClient = client;

        public void SendDataSingle(byte[] datagram)
        {
            Logger.Debug(
                ClassName,
                "Sending '{0}'.",
                BitConverter.ToString(datagram));

            _udpClient.Send(datagram, datagram.Length, _remoteEndpoint);
        }

        public override void SendData(byte[] datagram)
        {
            // TODO: Why does this need to send 4 times?
            Logger.Debug(
                ClassName,
                "Sending 4x '{0}'.",
                BitConverter.ToString(datagram));

            _udpClient.Send(datagram, datagram.Length, _remoteEndpoint);
            _udpClient.Send(datagram, datagram.Length, _remoteEndpoint);
            _udpClient.Send(datagram, datagram.Length, _remoteEndpoint);
            _udpClient.Send(datagram, datagram.Length, _remoteEndpoint);
        }

        public void SendTunnelingAck(byte sequenceNumber)
        {
            // HEADER
            var datagram = new byte[10];
            datagram[00] = 0x06;
            datagram[01] = 0x10;
            datagram[02] = 0x04;
            datagram[03] = 0x21;
            datagram[04] = 0x00;
            datagram[05] = 0x0A;

            datagram[06] = 0x04;
            datagram[07] = KnxConnectionTunneling.ChannelId;
            datagram[08] = sequenceNumber;
            datagram[09] = 0x00;

            _udpClient.Send(datagram, datagram.Length, _remoteEndpoint);
        }

        protected override byte[] CreateActionDatagram(KnxAddress destinationAddress, byte[] data)
        {
            lock (KnxConnectionTunneling.SequenceNumberLock)
            {
                try
                {
                    var dataLength = KnxHelper.GetDataLength(data);

                    // HEADER
                    var datagram = new byte[10];
                    datagram[00] = 0x06;
                    datagram[01] = 0x10;
                    datagram[02] = 0x04;
                    datagram[03] = 0x20;

                    var totalLength = BitConverter.GetBytes(dataLength + 20);
                    datagram[04] = totalLength[1];
                    datagram[05] = totalLength[0];

                    datagram[06] = 0x04;
                    datagram[07] = KnxConnectionTunneling.ChannelId;
                    datagram[08] = KnxConnectionTunneling.GenerateSequenceNumber();
                    datagram[09] = 0x00;

                    return CreateActionDatagramCommon(destinationAddress, data, datagram);
                }
                catch
                {
                    KnxConnectionTunneling.RevertSingleSequenceNumber();

                    return null;
                }
            }
        }

        protected override byte[] CreateRequestStatusDatagram(KnxAddress destinationAddress)
        {
            lock (KnxConnectionTunneling.SequenceNumberLock)
            {
                try
                {
                    // HEADER
                    var datagram = new byte[21];
                    datagram[00] = 0x06;
                    datagram[01] = 0x10;
                    datagram[02] = 0x04;
                    datagram[03] = 0x20;
                    datagram[04] = 0x00;
                    datagram[05] = 0x15;

                    datagram[06] = 0x04;
                    datagram[07] = KnxConnectionTunneling.ChannelId;
                    datagram[08] = KnxConnectionTunneling.GenerateSequenceNumber();
                    datagram[09] = 0x00;

                    return CreateRequestStatusDatagramCommon(destinationAddress, datagram, 10);
                }
                catch
                {
                    KnxConnectionTunneling.RevertSingleSequenceNumber();

                    return null;
                }
            }
        }
    }
}
