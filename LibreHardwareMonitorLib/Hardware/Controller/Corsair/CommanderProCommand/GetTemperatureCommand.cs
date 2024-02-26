using HidSharp;
using System.Threading;

namespace LibreHardwareMonitor.Hardware.Controller.Corsair.CommanderProCommand
{
    internal class GetTemperatureCommand : BaseCommand
    {
        public float Temperature { get; private set; }

        public GetTemperatureCommand(int temperaturSensorIndex, ref HidStream stream, ref SemaphoreSlim semaphoreSlim) : base(ref stream, ref semaphoreSlim)
        {
            var data = new byte[] { (byte)temperaturSensorIndex };
            var result = SendAndReceive(GET_TEMPERATURE, data);

            Temperature = ((result[2] << 8) | result[3]) / 100f;
        }
    }
}
