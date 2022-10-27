using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstUwp.Helpers
{
    public class VoiceHelper
    {

        //private static GpioController gpioController = new GpioController();
        private static int voicePin = 23;

        public static void voiceAction_1(GpioController gpioController)
        {
            Task.Run(() =>
            {
                gpioController.Write(voicePin, PinValue.High);
                Task.Delay(250).Wait();
                gpioController.Write(voicePin, PinValue.Low);
                Task.Delay(125).Wait();
                gpioController.Write(voicePin, PinValue.High);
                Task.Delay(250).Wait();
                gpioController.Write(voicePin, PinValue.Low);
            });
        }

        public static void voiceAction_2(GpioController gpioController)
        { 
            Task.Run(() =>
            {
                gpioController.Write(voicePin, PinValue.High);
                Task.Delay(625).Wait();
                gpioController.Write(voicePin, PinValue.Low);
            });
        }

        public static void voiceAction_3(GpioController gpioController)
        {
            Task.Run(() =>
            {
                gpioController.Write(voicePin, PinValue.High);
                Task.Delay(250).Wait();
                gpioController.Write(voicePin, PinValue.Low);
            });
        }
    }
}
