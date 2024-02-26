using HidSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace LibreHardwareMonitor.Hardware.Controller.Corsair
{
    internal class CorsairGroup : IGroup
    {
        private const ushort VENDOR_ID = 0x1b1c;
        private const ushort COMMANDER_PRO_PRODUCT_ID = 0x0c10;
        private readonly List<IHardware> _hardware = new();
        private readonly StringBuilder _report;

        public CorsairGroup(ISettings settings)
        {
            _report = new StringBuilder();
            _report.AppendLine("Corsair Controllers");
            _report.AppendLine();

            foreach (HidDevice dev in DeviceList.Local.GetHidDevices(VENDOR_ID))
            {
                switch (dev.ProductID)
                {
                    case COMMANDER_PRO_PRODUCT_ID:
                    {
                        _report.AppendLine($"Device found: \"Commander Pro\"");
                        _report.AppendLine();
                        CommanderPro commanderPro = new CommanderPro(dev, settings);
                        _hardware.Add(commanderPro);

                        break;
                    }
                    default: break;
                }
            }
        }

        public IReadOnlyList<IHardware> Hardware => _hardware;

        public void Close()
        {
            foreach (IHardware iHardware in _hardware)
            {
                if (iHardware is Hardware hardware)
                    hardware.Close();
            }
        }

        public string GetReport()
        {
            return _report.ToString()
                + Environment.NewLine
                + string.Join(Environment.NewLine, _hardware.Select(x => x.GetReport()).ToArray());
        }
    }
}
