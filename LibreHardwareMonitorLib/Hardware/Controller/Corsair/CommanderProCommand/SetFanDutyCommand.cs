using HidSharp;
using System.Threading;

namespace LibreHardwareMonitor.Hardware.Controller.Corsair.CommanderProCommand
{
    internal class SetFanDutyCommand : BaseCommand
    {
        public SetFanDutyCommand(int fanSensorIndex, int percentage, ref HidStream stream, ref SemaphoreSlim semaphoreSlim) : base(ref stream, ref semaphoreSlim)
        {
            var data = new byte[] { (byte)fanSensorIndex, (byte)percentage };
            SendAndReceive(SET_FAN_DUTY, data);
        }
    }
}
