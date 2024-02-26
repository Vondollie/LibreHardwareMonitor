using HidSharp;
using System.Threading;

namespace LibreHardwareMonitor.Hardware.Controller.Corsair.CommanderProCommand
{
    internal class GetFirmwareVersionCommand : BaseCommand
    {
        public string FirmwareVersion { get; private set; }

        public GetFirmwareVersionCommand(ref HidStream stream, ref SemaphoreSlim semaphoreSlim) : base(ref stream, ref semaphoreSlim)
        {
            var result = SendAndReceive(GET_FIRMWARE);

            FirmwareVersion = $"{result[2]}.{result[3]}.{result[4]}";
        }
    }
}
