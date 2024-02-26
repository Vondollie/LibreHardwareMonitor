using HidSharp;
using System.Threading;

namespace LibreHardwareMonitor.Hardware.Controller.Corsair.CommanderProCommand
{
    internal class GetBootloaderVersionCommand : BaseCommand
    {
        public string BootloaderVersion { get; private set; }

        public GetBootloaderVersionCommand(ref HidStream stream, ref SemaphoreSlim semaphoreSlim) : base(ref stream, ref semaphoreSlim)
        {
            var result = SendAndReceive(GET_BOOTLOADER);

            BootloaderVersion = $"{result[2]}.{result[3]}";
        }
    }
}
