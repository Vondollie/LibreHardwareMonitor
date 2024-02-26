using HidSharp;
using System;
using System.Threading;

namespace LibreHardwareMonitor.Hardware.Controller.Corsair.CommanderProCommand
{
    internal class SetFanRpmCommand : BaseCommand
    {
        public SetFanRpmCommand(int fanSensorIndex, ushort rpm, ref HidStream stream, ref SemaphoreSlim semaphoreSlim) : base(ref stream, ref semaphoreSlim)
        {
            byte[] rpmInBytes = new byte[2];
            Array.Copy(BitConverter.GetBytes(rpm), rpmInBytes, 2);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(rpmInBytes);

            var data = new byte[] { (byte)fanSensorIndex, rpmInBytes[0], rpmInBytes[1] };
            SendAndReceive(SET_FAN_RPM, data);
        }
    }
}
