using System;
using System.Diagnostics;
using System.Timers;
using UsbLibrary;

namespace PushTheButton.Console
{
    public class Program
    {
        private UsbHidPort _usb;
        private Timer _timer;

        protected bool ButtonIsDown { get; set; }
        protected bool CoverIsOpen { get; set; }

        public static void Main(string[] args)
        {
            var program = new Program();
            program.Begin();
        }

        private void Begin()
        {
            _usb = new UsbLibrary.UsbHidPort();
            _usb.OnSpecifiedDeviceRemoved += new EventHandler(USB_OnSpecifiedDeviceRemoved);
            _usb.OnDataRecieved += new DataRecievedEventHandler(USB_OnDataRecieved);
            _usb.OnSpecifiedDeviceArrived += new EventHandler(USB_OnSpecifiedDeviceArrived);
            _usb.VID_List[0] = 7476;
            _usb.PID_List[0] = 13; //18
            _usb.ID_List_Cnt = 1;
            _usb.RegisterHandle(Process.GetCurrentProcess().MainWindowHandle);
           
            _timer = new Timer(50);
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();

            for (; ;) //ever
            {
            }
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var numArray1 = new byte[9];
            numArray1[8] = (byte)2;
            SendUSBData(numArray1);
        }

        private void USB_OnSpecifiedDeviceArrived(object sender, EventArgs e)
        {
            var numArray3 = new byte[9];
            numArray3[1] = (byte)3;
            numArray3[2] = (byte)2;
            SendUSBData(numArray3);
        }

        private void USB_OnDataRecieved(object sender, DataRecievedEventArgs args)
        {
            UpdateHandle(args.data);
        }

        private void USB_OnSpecifiedDeviceRemoved(object sender, EventArgs e)
        {
            var numArray3 = new byte[9];
            numArray3[1] = (byte)3;
            numArray3[2] = (byte)2;
            SendUSBData(numArray3);
        }

        private void UpdateHandle(byte[] Data)
        {
            if (((int)Data[1] & 2) == 2 && !CoverIsOpen)
            {
                CoverIsOpen = true;
                System.Console.WriteLine("Cover is Opened");
                //Do something clever
            }
            else if (((int)Data[1] & 2) == 0 && CoverIsOpen)
            {
                CoverIsOpen = false;
                System.Console.WriteLine("Cover is Closed");
                //Do something clever
            }
            if (((int)Data[1] & 1) == 0 && !ButtonIsDown)
            {
                ButtonIsDown = true;
                System.Console.WriteLine(@"
       _______  | |  __  __           |  \/  |         \ \   / / | |
      |__   __| | | |  \/  |  ______  | \  / |     /\   \ \_/ /  | |
         | |    | | | \  / | |______| | |\/| |    /  \   \   /   |_|
         | |    | | | |\/| |          | |  | |   / /\ \   | |    (_)
         | |    |_| | |  | |          |_|  |_|  / ____ \  |_|
         |_|        |_|  |_|                   /_/    \_\
                            _______,.........._
                       _.::::::::::::::::::::::::._
                    _J::::::::::::::::::::::::::::::-.
                 _,J::::;::::::!:::::::::::!:::::::::::-.\_ ___
              ,-:::::/::::::::::::/''''''-:/   \::::::::::::::::L_
            ,;;;;;::!::/         V               -::::::::::::::::7
          ,J:::::::/ \/                              '-'`\:::::::.7
          |:::::::'                                       \::!:::/
         J::::::/                                          `.!:\ dp
         |:::::7                                             |/\:\
        J::::/                                               \/ \:|
        |:::/                                                    \:\
        |::/                                                     |:.Y
        |::7                                                      |:|
        |:/                              `OOO8ooo._               |:|
        |/               ,,oooo8OO'           ``Y8o,             |'
         |            ,odP'                      `8P            /
         |          ,8P'    _.__         .---.._                /
         |           '   .-'    `-.    .'       `-.            /
         `.            ,'          `. /            `.          L_
       .-.J.          /              Y               \        /_ \
      |    \         /               |                Y      // `|
       \ '\ \       Y          8B    |   8B           |     /Y   '
        \  \ \      |                |                ;    / |  /
         \  \ \     |               ,'.              /    /  L |
          \  J \     \           _.'   `-._       _.'    /  _.'
           `.___,\    `._     _,'          '-...-'     /'`'
                  \      '---'  _____________         /
                   `.           \|T T T T T|/       ,'
                     `.           \_|_|_|_/       .'
                       `.         `._.-..'      .'
                         `._         `-'     _,'
                            `--._________.--'"
                );
                PlayAudio();
            }
            else
            {
                if (((int)Data[1] & 1) != 1 || !ButtonIsDown)
                    return;
                ButtonIsDown = false;
            }
        }

        private void PlayAudio()
        {
            MediaPlayer.Play("the_sound.wav");
        }

        private void SendUSBData(byte[] Data)
        {
            if (_usb.SpecifiedDevice == null)
                return;
            try
            {
                _usb.SpecifiedDevice.SendData(Data);
            }
            catch (Exception)
            {
            }
        }
    }
}
