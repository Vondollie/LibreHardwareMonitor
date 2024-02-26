using HidSharp;
using System.Threading;

namespace LibreHardwareMonitor.Hardware.Controller.Corsair.CommanderProCommand
{
    internal class GetFanRpmCommand : BaseCommand
    {
        public ushort Rpm { get; private set; }

        public GetFanRpmCommand(int fanSensorIndex, ref HidStream stream, ref SemaphoreSlim semaphoreSlim) : base(ref stream, ref semaphoreSlim)
        {
            var data = new byte[] { (byte)fanSensorIndex };
            var result = SendAndReceive(GET_FAN_RPM, data);

            Rpm = (ushort)((result[2] << 8) | result[3]);
        }
    }
}
