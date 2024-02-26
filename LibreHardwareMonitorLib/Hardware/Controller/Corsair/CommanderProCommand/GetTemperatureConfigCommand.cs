using HidSharp;
using System.Collections.Generic;
using System.Threading;

namespace LibreHardwareMonitor.Hardware.Controller.Corsair.CommanderProCommand
{
    internal class GetTemperatureConfigCommand : BaseCommand
    {
        public Dictionary<int, TempMode> TemperatureConfig { get; private set; }

        public GetTemperatureConfigCommand(ref HidStream stream, ref SemaphoreSlim semaphoreSlim) : base(ref stream, ref semaphoreSlim)
        {
            var result = SendAndReceive(GET_TEMPERATURE_CONFIG);

            for (int i = 0; i < 5; i++)
            {
                if (TemperatureConfig == null)
                {
                    TemperatureConfig = new Dictionary<int, TempMode>();
                    TemperatureConfig.Add(i, (TempMode)result[i + 2]);
                }
                else
                {
                    TemperatureConfig.Add(i, (TempMode)result[i + 2]);
                }
            }
        }
    }
}
