using HidSharp;
using LibreHardwareMonitor.Hardware.Controller.Corsair.CommanderProCommand;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LibreHardwareMonitor.Hardware.Controller.Corsair
{
    internal sealed class CommanderPro : Hardware
    {
        private HidStream _stream;
        private List<Sensor> _fanSensors = new();
        private List<Sensor> _fanControls = new();
        private List<Sensor> _tempSensors = new();

        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private StringBuilder _report = new();

        public string FirmwareVersion { get; private set; }
        public string BootloaderVersion { get; private set; }

        public CommanderPro(HidDevice dev, ISettings settings) : base("Commander Pro", new Identifier(dev.DevicePath), settings)
        {
            if (dev.TryOpen(out _stream))
            {
                _stream.ReadTimeout = 5000;

                FirmwareVersion = new GetFirmwareVersionCommand(ref _stream, ref _semaphoreSlim).FirmwareVersion;
                BootloaderVersion = new GetBootloaderVersionCommand(ref _stream, ref _semaphoreSlim).BootloaderVersion;

                _report.AppendLine($"Commander Pro device product Id: {dev.ProductID} FirmwareVersion: {FirmwareVersion} BootloaderVersion: {BootloaderVersion}");

                var fanModes = new GetFanModesCommand(ref _stream, ref _semaphoreSlim).FanModes;

                foreach (var fan in fanModes)
                {
                    if (fan.Value != BaseCommand.FanMode.FAN_MODE_DISCONNECTED)
                    {
                        Sensor fanControl = new($"Commander Pro Fan #{fan.Key}", fan.Key, SensorType.Control, this, settings);
                        Control control = new(fanControl, settings, 0, 100);
                        fanControl.Control = control;
                        control.ControlModeChanged += c => SoftwareControlValueChanged(c, fan.Key);
                        control.SoftwareControlValueChanged += c => SoftwareControlValueChanged(c, fan.Key);

                        Sensor fanSensor = new($"Commander Pro Fan #{fan.Key}", fan.Key, SensorType.Fan, this, settings);

                        _fanControls.Add(fanControl);
                        _fanSensors.Add(fanSensor);

                        ActivateSensor(fanControl);
                        ActivateSensor(fanSensor);

                        _report.AppendLine($"Fan index {fan.Key} found ");
                    }
                }

                var tempConfig = new GetTemperatureConfigCommand(ref _stream, ref _semaphoreSlim).TemperatureConfig;

                foreach (var temperaturSensor in tempConfig)
                {
                    if (temperaturSensor.Value == BaseCommand.TempMode.FAN_MODE_CONNECTED)
                    {
                        Sensor temperature = new($"Commander Pro Temp Sensor #{temperaturSensor.Key}", temperaturSensor.Key, SensorType.Temperature, this, settings);

                        _tempSensors.Add(temperature);

                        ActivateSensor(temperature);
                        temperature.Value = new GetTemperatureCommand(1, ref _stream, ref _semaphoreSlim).Temperature;

                        _report.AppendLine($"Temperature sensor index {temperaturSensor.Key} found ");
                    }
                }
            }
        }

        private void SoftwareControlValueChanged(Control control, int fanIndex)
        {
            if (control.ControlMode == ControlMode.Software)
            {
                int value = (int)control.SoftwareValue;
                new SetFanDutyCommand(fanIndex, (value > 100 ? 100 : (value < 0) ? 0 : value), ref _stream, ref _semaphoreSlim);
            }
            else if (control.ControlMode == ControlMode.Default)
            {
                // there is no default
            }
        }

        public override HardwareType HardwareType => HardwareType.EmbeddedController;

        public override void Update()
        {
            foreach (var fan in _fanSensors)
            {
                fan.Value = new GetFanRpmCommand(fan.Index, ref _stream, ref _semaphoreSlim).Rpm;
            }

            foreach (var fan in _fanControls)
            {
                fan.Value = new GetFanDutyCommand(fan.Index, ref _stream, ref _semaphoreSlim).Duty;
            }

            foreach (var temperatur in _tempSensors)
            {
                temperatur.Value = new GetTemperatureCommand(temperatur.Index, ref _stream, ref _semaphoreSlim).Temperature;
            }
        }

        public override void Close()
        {
            _fanSensors.Clear();
            _fanControls.Clear();
            _tempSensors.Clear();

            base.Close();
            _stream?.Close();
        }

        public override string GetReport()
        {
            return _report.ToString();
        }
    }
}
