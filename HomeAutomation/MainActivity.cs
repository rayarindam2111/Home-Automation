using System;
using System.Net;
using System.IO;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace HomeAutomation
{
    [Activity(Label = "HomeAutomation", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        WebRequest request;
        WebResponse response;
        Stream dataStream;
        StreamReader reader;
        string responseFromServer;

        bool networkBusy = false;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            
            SetContentView(Resource.Layout.Main);

            Button button1 = FindViewById<Button>(Resource.Id.btnShowDevicesActivity);
            Button button2 = FindViewById<Button>(Resource.Id.btnphotocheckActivity);

            button1.Click += delegate { StartActivity(typeof(DevicesActivity)); };
            button2.Click += delegate { StartActivity(typeof(photoCheckActivity)); };


            TimerExampleState eg = new TimerExampleState();
            TimerCallback timerDelegate = new TimerCallback(CheckStatusTimer);
            Timer timer = new Timer(timerDelegate, eg, 2000 , 4000);
            eg.tmr = timer;
        }

        void CheckStatusTimer(Object state)
        {
           checkStatus();
        }

        async void checkStatus()
        {
            if (networkBusy)
                return;
            try
            {
                networkBusy = true;
                request = WebRequest.Create(Resources.GetString(Resource.String.serverURL) + "appHandler.php?checkAppTasksPending");
                request.Credentials = CredentialCache.DefaultCredentials;
                response = await request.GetResponseAsync();
                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
                reader.Close();
                response.Close();
                if (responseFromServer.Equals("1"))
                    getAppTasks();
                networkBusy = false;
            }
            catch (System.Exception)
            {
                networkBusy = false;
            }
        }

        async void getAppTasks()
        {
            
            try
            {
                networkBusy = true;
                request = WebRequest.Create(Resources.GetString(Resource.String.serverURL) + "appHandler.php?getAppQueue");
                request.Credentials = CredentialCache.DefaultCredentials;
                response = await request.GetResponseAsync();
                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
                reader.Close();
                response.Close();

                string[] tasks = responseFromServer.Split(new string[] { "<br>" },StringSplitOptions.None);

                for (int i=0; i<tasks.Length; i++)
                {
                    string s= Resources.GetString(Resource.String.doorInputSerial);
                    if (tasks[i].Equals(s))
                        RunOnUiThread(() => { StartActivity(typeof(photoCheckActivity)); });
                }

                networkBusy = false;
            }
            catch (System.Exception)
            {
                networkBusy = false;
           
            }
        }

    }

    class TimerExampleState
    {
        public Timer tmr;
    }
}

