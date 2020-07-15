using System;
using System.Threading;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Displays.Lcd;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Peripherals.Sensors.Atmospheric;

namespace EnviroMeadow
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        Bme280 bme280;
        CharacterDisplay display;

        public MeadowApp()
        {
            Initialize(); 
            
            
        }


        void Initialize()
        {
            SetupDisplay();

            SetupBME280();

        }

        private void SetupBME280()
        {

            Console.WriteLine("Initializing BME280");

            // configure our BME280 on the I2C Bus
            var i2c = Device.CreateI2cBus();
            bme280 = new Bme280(
                i2c,
                Bme280.I2cAddress.Adddress0x77 //default
            );

            // classical .NET events can also be used:
            bme280.Updated += (object sender, AtmosphericConditionChangeResult e) =>
            {
                UpdateDisplay(Convert.ToDecimal(e.New.Temperature), Convert.ToDecimal(e.New.Pressure), Convert.ToDecimal(e.New.Humidity));
            };

            // start updating continuously
            bme280.StartUpdating();
            Console.WriteLine("BME280 ready");

            UpdateDisplay(Convert.ToDecimal(bme280.Temperature), Convert.ToDecimal(bme280.Pressure), Convert.ToDecimal(bme280.Humidity));
        }

        private void UpdateDisplay(Decimal temperature, Decimal pressure, Decimal humidity)
        {
            string temperatureString = temperature.ToString("    00");
            string pressureString =       pressure.ToString("   000000");
            string humidityString =       humidity.ToString("       00");

            //display.WriteLine("(T" + initialTemperature.ToString("00") + ", P" + initialPressure.ToString("000000") + ", H" + initialHumidity.ToString("00") + ")", 0);
            display.WriteLine("Enviro-Meadow", 0);
            display.WriteLine("Temperature" + temperatureString + $"C", 1);
            display.WriteLine("Pressure" + pressureString + $"hPa", 2);
            display.WriteLine("Humidity" + humidityString + $"%", 3);
        }

        private void SetupDisplay()
        {
            Console.WriteLine("Initializing Display");

            display = new CharacterDisplay(
                Device,
                pinRS: Device.Pins.D05,
                pinE: Device.Pins.D11,
                pinD4: Device.Pins.D12,
                pinD5: Device.Pins.D13,
                pinD6: Device.Pins.D14,
                pinD7: Device.Pins.D15,
                rows: 4, columns: 20    // Adjust dimensions to fit your display
                );
            display.SetBrightness(0.5f);
            display.WriteLine("Hello!", 0);
            display.WriteLine("Just getting ready...", 1);

            Console.WriteLine("Display ready");
        }

    }
}
