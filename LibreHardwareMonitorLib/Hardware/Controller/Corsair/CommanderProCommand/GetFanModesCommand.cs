using HidSharp;
using System.Collections.Generic;
using System.Threading;

namespace LibreHardwareMonitor.Hardware.Controller.Corsair.CommanderProCommand
{
    internal class GetFanModesCommand : BaseCommand
    {
        public Dictionary<int, FanMode> FanModes { get; private set; }

        public GetFanModesCommand(ref HidStream stream, ref SemaphoreSlim semaphoreSlim) : base(ref stream, ref semaphoreSlim)
        {
            var result = SendAndReceive(GET_FAN_MODES);

            for (int i = 0; i < 6; i++)
            {
                if (FanModes == null)
                {
                    FanModes = new Dictionary<int, FanMode>();
                    FanModes.Add(i, (FanMode)result[i + 2]);
                }
                else
                {
                    FanModes.Add(i, (FanMode)result[i + 2]);
                }
            }
        }
    }
}
