using System.Net;
using System.IO;
using System;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace HomeAutomation
{
    [Activity(Label = "Confirm User Entry", Icon = "@drawable/icon")]
    public class photoCheckActivity : Activity
    {
        
        bool networkBusy = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.PhotoCheck);
           
            var btnYes = FindViewById<Button>(Resource.Id.btnYes);
            var btnNo = FindViewById<Button>(Resource.Id.btnNo);
            ProgressBar pbar = FindViewById<ProgressBar>(Resource.Id.pbLoad);
            ImageView image = FindViewById<ImageView>(Resource.Id.imgUserGrant);

            downloadPhoto(pbar, image);

            btnYes.Click += delegate { addToDeviceQueue(Resources.GetString(Resource.String.doorOpenSerial)); };
            btnNo.Click += delegate { this.Finish(); };

        }

        void downloadPhoto(ProgressBar pbar, ImageView image)
        {
            AsyncProgressReporting.Common.DownloadHelper picDownload = new AsyncProgressReporting.Common.DownloadHelper(pbar, image);
        }



        async void addToDeviceQueue(string serialCode)
        {
            if (networkBusy)
                return;
            try
            {
                networkBusy = true;
                WebRequest request = WebRequest.Create(Resources.GetString(Resource.String.serverURL) + "appHandler.php?appToDeviceSerial=" + serialCode);
                request.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = await request.GetResponseAsync();
                response.Close();
                networkBusy = false;
                Toast.MakeText(this, "Door opened!", ToastLength.Short).Show();
                this.Finish();
            }
            catch (System.Exception)
            {
                Toast.MakeText(this, "Network Failure!", ToastLength.Short).Show();
                networkBusy = false;
            }

        }


    }

}


namespace AsyncProgressReporting.Common
{
    public class DownloadHelper : Activity
    {
        public static readonly string ImageToDownload = "http://www.iemculturalfest.com/homeA/pic.jpg";
        public static readonly int BufferSize = 4096;

        public DownloadHelper(ProgressBar pbar, ImageView image)
        {
            StartDownloadHandler(pbar,image);
        }

        public static async Task<int> CreateDownloadTask(string urlToDownload)
        {
            int receivedBytes = 0;
            int totalBytes = 0;
            WebClient client = new WebClient();
            string filePath = Android.OS.Environment.ExternalStorageDirectory.ToString();
            string filename = filePath + "/HomeAutomation/pic.jpg";
            FileStream fileStream = new FileStream(filename, FileMode.Create);
            using (var stream = await client.OpenReadTaskAsync(urlToDownload))
            {
                byte[] buffer = new byte[BufferSize];
                totalBytes = Int32.Parse(client.ResponseHeaders[HttpResponseHeader.ContentLength]);
                for (;;)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        await Task.Yield();
                        break;
                    }
                    fileStream.Write(buffer, 0, buffer.Length);

                    receivedBytes += bytesRead;
                }
                fileStream.Close();

            }

            return receivedBytes;
        }

        async void StartDownloadHandler(ProgressBar pbar, ImageView image)
        {
            Task<int> downloadTask = DownloadHelper.CreateDownloadTask(DownloadHelper.ImageToDownload);
            int bytesDownloaded = await downloadTask;
            RunOnUiThread(() => {
                pbar.Visibility = ViewStates.Gone;
                Bitmap bmp = BitmapFactory.DecodeFile(Android.OS.Environment.ExternalStorageDirectory.ToString() + "/HomeAutomation/pic.jpg");
                image.SetImageBitmap(bmp);
            });

        }
    }

}