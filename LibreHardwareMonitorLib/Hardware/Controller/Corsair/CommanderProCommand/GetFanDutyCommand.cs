using HidSharp;
using System.Threading;

namespace LibreHardwareMonitor.Hardware.Controller.Corsair.CommanderProCommand
{
    internal class GetFanDutyCommand : BaseCommand
    {
        public ushort Duty { get; private set; }

        public GetFanDutyCommand(int fanSensorIndex, ref HidStream stream, ref SemaphoreSlim semaphoreSlim) : base(ref stream, ref semaphoreSlim)
        {
            var data = new byte[] { (byte)fanSensorIndex };
            var result = SendAndReceive(GET_FAN_DUTY, data);

            Duty = (result[2]);
        }
    }
}
