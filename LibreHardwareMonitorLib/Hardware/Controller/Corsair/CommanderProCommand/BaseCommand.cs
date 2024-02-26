using HidSharp;
using System;
using System.Threading;

namespace LibreHardwareMonitor.Hardware.Controller.Corsair.CommanderProCommand
{
    internal class BaseCommand
    {
        private const int REPORT_LENGTH = 64;
        private const int RESPONSE_LENGTH = 16;

        protected const byte GET_FIRMWARE = 0x02;
        protected const byte GET_BOOTLOADER = 0x06;

        protected const byte GET_TEMPERATURE_CONFIG = 0x10;
        protected const byte GET_TEMPERATURE = 0x11;

        protected const byte GET_FAN_MODES = 0x20;
        protected const byte GET_FAN_RPM = 0x21;
        protected const byte GET_FAN_DUTY = 0x22;
        protected const byte SET_FAN_DUTY = 0x23;
        protected const byte SET_FAN_RPM = 0x24;

        public enum FanMode
        {
            FAN_MODE_DISCONNECTED = 0x00,
            FAN_MODE_DC = 0x01,
            FAN_MODE_PWM = 0x02
        }

        public enum TempMode
        {
            FAN_MODE_DISCONNECTED = 0x00,
            FAN_MODE_CONNECTED = 0x01
        }

        private readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(3);

        private HidStream _stream;
        private SemaphoreSlim _semaphoreSlim;

        public BaseCommand(ref HidStream stream, ref SemaphoreSlim semaphoreSlim)
        {
            _stream = stream;
            _semaphoreSlim = semaphoreSlim;
        }


        protected byte[] SendAndReceive(byte command, byte[] data = null)
        {
            _semaphoreSlim.Wait(_defaultTimeout);

            byte[] writeBuffer = new byte[REPORT_LENGTH];
            writeBuffer[1] = command;
            int startAt = 2;

            if (data != null)
            {
                Array.Resize(ref data, REPORT_LENGTH - 2);
                Array.Copy(data, 0, writeBuffer, startAt, data.Length);
            }

            _stream.Write(writeBuffer);
            byte[] readBuffer = new byte[RESPONSE_LENGTH];
            _stream.Read(readBuffer, 0, readBuffer.Length);

            _semaphoreSlim.Release();

            return readBuffer;
        }
    }
}
