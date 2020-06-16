namespace FunctionalLiving.Knx
{
    using System;
    using System.Net;
    using System.Text;
    using DPT;
    using Exceptions;
    using Enums;
    using Addressing;
    using Events;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     Base class that controls the KNX connection, implemented by KnxConnectionRouting and KnxConnetionTunneling
    /// </summary>
    public abstract class KnxConnection : IDisposable
    {
        private readonly ILogger<KnxConnection> _logger;

        /// <summary>
        ///     Delegate function for connection established trigger
        /// </summary>
        public delegate void KnxConnected();

        /// <summary>
        ///     Event triggered when connection is established
        /// </summary>
        public KnxConnected KnxConnectedDelegate;

        /// <summary>
        ///     Delegate function for disconnection trigger
        /// </summary>
        public delegate void KnxDisconnected();

        /// <summary>
        ///     Event triggered when connection drops
        /// </summary>
        public KnxDisconnected KnxDisconnectedDelegate;

        /// <summary>
        ///     Delegate function for KNX events
        /// </summary>
        public delegate void KnxEvent(object sender, KnxEventArgs args);

        /// <summary>
        ///     Event triggered when there is a new KNX event
        /// </summary>
        public KnxEvent KnxEventDelegate;

        /// <summary>
        ///     Delegate function for KNX status queries
        /// </summary>
        public delegate void KnxStatus(object sender, KnxStatusArgs args);

        /// <summary>
        ///     Event triggered when received a status after a query
        /// </summary>
        public KnxStatus KnxStatusDelegate;

        private readonly KnxLockManager _lockManager = new KnxLockManager();

        /// <summary>
        ///     Create a new KNX Connection to specified host and port
        /// </summary>
        /// <param name="host">Host to connect</param>
        /// <param name="port">Port to use</param>
        protected KnxConnection(
            ILoggerFactory loggerFactory,
            string host,
            int port)
        {
            _logger = loggerFactory.CreateLogger<KnxConnection>();
            ConnectionConfiguration = new KnxConnectionConfiguration(host, port);

            ActionMessageCode = 0x00;
            GroupAddressStyle = KnxGroupAddressStyle.ThreeLevel;
            Debug = false;
        }

        internal KnxConnectionConfiguration ConnectionConfiguration { get; }

        /// <summary>
        ///     Get the IPEndPoint instance representing the remote KNX gateway
        /// </summary>
        public IPEndPoint RemoteEndpoint => ConnectionConfiguration.EndPoint;

        internal KnxReceiver KnxReceiver { get; set; }

        internal KnxSender KnxSender { get; set; }

        /// <summary>
        ///     Configure this paramenter based on the KNX installation:
        ///     - ThreeLevel: 3-level group address: main/middle/sub (5/3/8 bits)
        ///     - TwoLevel: 2-level group address: main/sub (5/11 bits)
        ///     - Free: free style group address: sub (16 bits)
        ///     Default: ThreeLevel
        /// </summary>
        public KnxGroupAddressStyle GroupAddressStyle { get; set; }

        /// <summary>
        ///     Set to true to receive debug log messages
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        ///     Some KNX Routers/Interfaces might need this parameter defined, some need this to be 0x29.
        ///     Default: 0x00
        /// </summary>
        public byte ActionMessageCode { get; set; }

        /// <summary>
        ///     Start the connection
        /// </summary>
        public abstract void Connect();

        /// <summary>
        ///     Stop the connection
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        ///     Event triggered by implementing class to notify that the connection has been established
        /// </summary>
        internal virtual void Connected()
        {
            _lockManager.UnlockConnection();

            try
            {
                KnxConnectedDelegate?.Invoke();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Calling KnxConnectedDelegate failed.");
            }

            _logger.LogDebug(
                "KNX is connected. Unlocking send - {LockCount} free locks",
                _lockManager.LockCount);
        }

        /// <summary>
        ///     Event triggered by implementing class to notify that the connection has been established
        /// </summary>
        internal virtual void Disconnected()
        {
            _lockManager.LockConnection();

            try
            {
                KnxDisconnectedDelegate?.Invoke();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Calling KnxDisconnectedDelegate failed.");
            }

            _logger.LogDebug(
                "KNX is disconnected. Send locked - {LockCount} free locks",
                _lockManager.LockCount);
        }

        internal void Event(KnxEventArgs args)
        {
            try
            {
                KnxEventDelegate?.Invoke(this, args);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Calling KnxEventDelegate failed.");
            }

            _logger.LogDebug(
                "Device '{SourceAddress}' sent event '{State}' to '{DestinationAddress}'.",
                args.SourceAddress,
                args.StateHex,
                args.DestinationAddress);
        }

        internal void Status(KnxStatusArgs args)
        {
            try
            {
                KnxStatusDelegate?.Invoke(this, args);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Calling KnxStatusDelegate failed.");
            }

            _logger.LogDebug(
                "Device '{SourceAddress}' has status '{State}' for '{DestinationAddress}'.",
                args.SourceAddress,
                args.StateHex,
                args.DestinationAddress);
        }

        /// <summary>
        ///     Set the lock interval between requests sent to the network (in ms)
        /// </summary>
        /// <param name="interval">time in ms for the interval</param>
        public void SetLockIntervalMs(int interval)
            => _lockManager.IntervalMs = interval;

        /// <summary>
        ///     Send a bit value as data to specified address
        /// </summary>
        /// <param name="address">KNX Address</param>
        /// <param name="data">Bit value</param>
        /// <exception cref="InvalidKnxDataException"></exception>
        public void Action(KnxAddress address, bool data)
        {
            byte[] val;

            try
            {
                val = new[] { Convert.ToByte(data) };
            }
            catch
            {
                throw new InvalidKnxDataException(data.ToString());
            }

            if (val == null)
                throw new InvalidKnxDataException(data.ToString());

            Action(address, val, addTruncateByte: false);
        }

        /// <summary>
        ///     Send a string value as data to specified address
        /// </summary>
        /// <param name="address">KNX Address</param>
        /// <param name="data">String value</param>
        /// <exception cref="InvalidKnxDataException"></exception>
        public void Action(KnxAddress address, string data)
        {
            byte[] val;
            try
            {
                val = Encoding.ASCII.GetBytes(data);
            }
            catch
            {
                throw new InvalidKnxDataException(data);
            }

            if (val == null)
                throw new InvalidKnxDataException(data);

            Action(address, val);
        }

        /// <summary>
        ///     Send an int value as data to specified address
        /// </summary>
        /// <param name="address">KNX Address</param>
        /// <param name="data">Int value</param>
        /// <exception cref="InvalidKnxDataException"></exception>
        public void Action(KnxAddress address, int data)
        {
            var val = new byte[2];
            if (data <= 255)
            {
                val[0] = 0x00;
                val[1] = (byte) data;
            }
            else if (data <= 65535)
            {
                val[0] = (byte) data;
                val[1] = (byte) (data >> 8);
            }
            else
            {
                // allowing only positive integers less than 65535 (2 bytes), maybe it is incorrect...???
                throw new InvalidKnxDataException(data.ToString());
            }

            if (val == null)
                throw new InvalidKnxDataException(data.ToString());

            Action(address, val);
        }

        /// <summary>
        ///     Send a byte value as data to specified address
        /// </summary>
        /// <param name="address">KNX Address</param>
        /// <param name="data">byte value</param>
        public void Action(KnxAddress address, byte data)
            => Action(address, new byte[] {0x00, data});

        /// <summary>
        ///     Send a byte array value as data to specified address
        /// </summary>
        /// <param name="address">KNX Address</param>
        /// <param name="data">Byte array value</param>
        /// <param name="addTruncateByte">adds extra byte to chop off for payload</param>
        public void Action(KnxAddress address, byte[] data, bool addTruncateByte = true)
        {
            if (addTruncateByte)
            {
                // reverse bytes temporary to add byte in front
                Array.Reverse(data);
                Array.Resize(ref data, data.Length + 1);
                data[data.Length - 1] = 0x00;
                Array.Reverse(data);
            }

            _logger.LogDebug(
                "Sending '{State}' to '{DestinationAddress}'.",
                BitConverter.ToString(data),
                address);

            _lockManager.PerformLockedOperation(() => KnxSender.Action(address, data));

            _logger.LogDebug(
                "Sent '{State}' to '{DestinationAddress}'.",
                BitConverter.ToString(data),
                address);
        }

        /// <summary>
        ///     Send a request to KNX asking for specified address current status
        /// </summary>
        /// <param name="address"></param>
        public void RequestStatus(KnxAddress address)
        {
            _logger.LogDebug(
                "Sending request status to '{DestinationAddress}'.",
                address);

            _lockManager.PerformLockedOperation(() => KnxSender.RequestStatus(address));

            _logger.LogDebug(
                "Sent request status to '{DestinationAddress}'.",
                address);
        }

        /// <summary>
        ///     Convert a value received from KNX using datapoint translator, e.g.,
        ///     get a temperature value in Celsius
        /// </summary>
        /// <param name="type">Datapoint type, e.g.: 9.001</param>
        /// <param name="data">Data to convert</param>
        /// <returns></returns>
        public object FromDataPoint(string type, string data)
            => DataPointTranslator.Instance.FromDataPoint(type, data);

        /// <summary>
        ///     Convert a value received from KNX using datapoint translator, e.g.,
        ///     get a temperature value in Celsius
        /// </summary>
        /// <param name="type">Datapoint type, e.g.: 9.001</param>
        /// <param name="data">Data to convert</param>
        /// <returns></returns>
        public object FromDataPoint(string type, byte[] data)
            => DataPointTranslator.Instance.FromDataPoint(type, data);

        /// <summary>
        ///     Convert a value to send to KNX using datapoint translator, e.g.,
        ///     get a temperature value in Celsius in a byte representation
        /// </summary>
        /// <param name="type">Datapoint type, e.g.: 9.001</param>
        /// <param name="value">Value to convert</param>
        /// <returns></returns>
        public byte[] ToDataPoint(string type, string value)
            => DataPointTranslator.Instance.ToDataPoint(type, value);

        /// <summary>
        ///     Convert a value to send to KNX using datapoint translator, e.g.,
        ///     get a temperature value in Celsius in a byte representation
        /// </summary>
        /// <param name="type">Datapoint type, e.g.: 9.001</param>
        /// <param name="value">Value to convert</param>
        /// <returns></returns>
        public byte[] ToDataPoint(string type, object value)
            => DataPointTranslator.Instance.ToDataPoint(type, value);

        public abstract void Dispose();

        protected abstract void Dispose(bool disposing);
    }
}
