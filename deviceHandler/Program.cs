using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace deviceHandler
{
    class TimerExampleState
    {
        public Timer tmr;
    }

    class Program
    {   
        bool networkBusy = false;
        string serverPath = "http://www.iemculturalfest.com/homeA/";
        //string serverPath = "http://localhost/homeA/";
        string fileToUpload = "C:\\Users\\rupa\\Desktop\\pic.jpg";
        string doorCheckString = "doorcheck";
        string portno = "COM26";
        SerialPort port;


        static void Main(string[] args)
        {
            Program main = new Program();
            main.run();
        }

        void run()
        {
            try
            {
                port = new SerialPort(portno, 9600);
                port.Open();
                port.DataReceived += new SerialDataReceivedEventHandler(checkStatusSerial);
            }
            catch (System.Exception)
            {
                Console.WriteLine("Error opening serial port '" + portno + "'. Check if it is already in use.");
            }

            TimerExampleState server = new TimerExampleState();
            TimerCallback timerDelegateServer = new TimerCallback(checkStatusServer);
            Timer timerServer = new Timer(timerDelegateServer, server, 1500, 3000);
            server.tmr = timerServer;

            //TimerExampleState serialPort = new TimerExampleState();
            //TimerCallback timerDelegateSerial = new TimerCallback(checkStatusSerial);
            //Timer timerSerial = new Timer(timerDelegateSerial, serialPort, 2000, 4000);
            //serialPort.tmr = timerSerial;

            while (server.tmr != null /*&& serialPort.tmr != null*/)
                Thread.Sleep(0);
        }


        async void checkStatusServer(Object state)
        {
            Console.WriteLine("Listening to server...");
            if (networkBusy)
                return;
            try
            {
                networkBusy = true;
                WebRequest request = WebRequest.Create(serverPath + "deviceHandler.php?checkDeviceTasksPending");
                request.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = await request.GetResponseAsync();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                response.Close();
                if (responseFromServer.Equals("1"))
                    getDeviceTasks();
                networkBusy = false;
            }
            catch (System.Exception)
            {
                networkBusy = false;
                Console.WriteLine("Network Failure!");
            }
        }

        async void getDeviceTasks()
        {

            try
            {
                networkBusy = true;
                WebRequest request = WebRequest.Create(serverPath + "deviceHandler.php?getDeviceQueue");
                request.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = await request.GetResponseAsync();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                response.Close();

                string[] tasks = responseFromServer.Split(new string[] { "<br>" }, StringSplitOptions.None);

                if(tasks.Length>0)
                    Console.WriteLine("Data received from server: ");

                for (int i = 0; i < tasks.Length; i++)
                {                   
                    Console.WriteLine(tasks[i]);
                    sendToSerial(port , tasks[i]);             
                }

                networkBusy = false;
            }
            catch (System.Exception)
            {
                networkBusy = false;
                Console.WriteLine("Network Failure!");
            }
        }

  
        async void checkStatusSerial(object sender , SerialDataReceivedEventArgs e)
        {
            string data = readFromSerial((SerialPort)sender);

            if (data.Equals(""))
                return;

            Console.WriteLine("Data received from serial:");
            Console.WriteLine(data);

            if (data.Equals(doorCheckString))
                uploadImage();

            try
            {
                WebRequest request = WebRequest.Create(serverPath + "deviceHandler.php?deviceToAppSerial=" + data);
                request.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = await request.GetResponseAsync();
                response.Close();
            }
            catch (System.Exception)
            {
                Console.WriteLine("Failed data upload to server.");
            }
            
        }

        void uploadImage()
        {
            try
            {
                Console.WriteLine("Image uploading.");
                WebClient up = new WebClient();
                up.UploadFile(serverPath + "upload.php", fileToUpload);
                up.Dispose();
                Console.WriteLine("Image uploaded.");
            }
            catch (System.Exception)
            {
                Console.WriteLine("Error uploading image to server.");
            }

        }

        void sendToSerial(SerialPort p, string serial)
        {
            try
            {
                p.Write(serial);
                Thread.Sleep(1);
            }
            catch (System.Exception)
            {
                Console.WriteLine("Error sending to serial port.");
            }
           
        }

        string readFromSerial(SerialPort p)
        {
            try
            {
                return p.ReadExisting(); ;
            }
            catch(System.Exception)
            {
                Console.WriteLine("Error reading bytes from port.");
                return "";
            }
            
        }


    }
}





