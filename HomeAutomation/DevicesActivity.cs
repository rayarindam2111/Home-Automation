using System.IO;
using System.Net;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace HomeAutomation
{
    [Activity(Label = "Devices" , Icon = "@drawable/icon")]
    public class DevicesActivity : Activity
    {
        string filePath, filename, directoryName;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Devices);

            var btnAdd = FindViewById<TextView>(Resource.Id.btnAdd);
            var btnCancel = FindViewById<Button>(Resource.Id.btnCancel);
            var btnOk = FindViewById<Button>(Resource.Id.btnOk);
            var addDevice = FindViewById<LinearLayout>(Resource.Id.addDevice);
            var txtName = FindViewById<EditText>(Resource.Id.txtName);
            var txtSerialOn = FindViewById<EditText>(Resource.Id.txtSerialOn);
            var txtSerialOff = FindViewById<EditText>(Resource.Id.txtSerialOff);
            var btnDelete = FindViewById<Button>(Resource.Id.btnDelete);

            initPath();

            btnAdd.Click += delegate { addDevice.Visibility = ViewStates.Visible; };
            btnCancel.Click += delegate {
                txtName.Text = "";
                txtSerialOff.Text = "";
                txtSerialOn.Text = "";
                addDevice.Visibility = ViewStates.Gone;
            };
            btnOk.Click += delegate {
                if (txtName.Text.Equals("") || txtSerialOn.Text.Equals("") || txtSerialOff.Text.Equals(""))
                {
                    Toast.MakeText(this, "Enter valid inputs!", ToastLength.Short).Show();
                    return;
                }
                addDeviceToList(txtName.Text, txtSerialOn.Text, txtSerialOff.Text);
                txtName.Text = "";
                txtSerialOff.Text = "";
                txtSerialOn.Text = "";
                addDevice.Visibility = ViewStates.Gone;
            };
            btnDelete.LongClick += delegate {
                writeFile("Devicename#serialon#serialoff");
                populateList();
                Toast.MakeText(this, "All devices deleted" , ToastLength.Short).Show();
            };

            populateList();

        }

        void initPath()
        {
            filePath = Android.OS.Environment.ExternalStorageDirectory.ToString();
            filename = Path.Combine(filePath, "HomeAutomation/DeviceList.txt");
            directoryName = Path.Combine(filePath, "HomeAutomation");
        }

        void populateList()
        {
            var frameLoading = FindViewById<FrameLayout>(Resource.Id.frameLoading);
            var mainLayout = FindViewById<LinearLayout>(Resource.Id.mainLayout);

            mainLayout.RemoveAllViews();

            frameLoading.Visibility = ViewStates.Visible;

            string fullString;

            if (File.Exists(filename))
                fullString = File.ReadAllText(filename);
            else
            {
                fullString = new StreamReader(Assets.Open("DeviceList.txt")).ReadToEnd();
                writeFile(fullString);
            }

          
            string[] lineString = fullString.Split('\n');
            string[,] dev = new string[lineString.Length, 3];


            for (int i = 1; i < lineString.Length; i++)
            {
                dev[i, 0] = lineString[i].Split('#')[0];
                dev[i, 1] = lineString[i].Split('#')[1];
                dev[i, 2] = lineString[i].Split('#')[2];

                var frame = new FrameLayout(this);
                frame.SetBackgroundColor(Android.Graphics.Color.Rgb(71, 75, 54));
                frame.SetPadding(10, 10, 10, 10);

                var textSwitch = new TextView(this);
                textSwitch.SetPadding(10, 10, 10, 10);
                textSwitch.SetTextSize(Android.Util.ComplexUnitType.Dip, 25);
                textSwitch.SetPadding(10, 7, 10, 10);
                textSwitch.Gravity = GravityFlags.FillVertical;

                var selectSwitch = new Switch(this);
                selectSwitch.SetPadding(10, 14, 10, 10);
                selectSwitch.Gravity = GravityFlags.FillVertical;

                var spaceDivider = new Space(this);
                spaceDivider.SetMinimumHeight(10);

                textSwitch.Text = dev[i, 0];

                selectSwitch.Tag = i;

                selectSwitch.CheckedChange += (sender, e) => {
                    if (selectSwitch.Checked)
                        turnOnDevice(dev[(int)selectSwitch.Tag, 0]/*Device Name*/, dev[(int)selectSwitch.Tag, 1]/*DeviceOn Serial*/);
                    else
                        turnOffDevice(dev[(int)selectSwitch.Tag, 0]/*Device Name*/, dev[(int)selectSwitch.Tag, 2]/*DeviceOff Serial*/);
                };

                frame.AddView(textSwitch);
                frame.AddView(selectSwitch);

                mainLayout.AddView(frame);
                mainLayout.AddView(spaceDivider);
            }

            frameLoading.Visibility = ViewStates.Gone;
        }

       
        void addDeviceToList(string name, string serialOn, string serialOff)
        {
            string fullString = File.ReadAllText(filename);
            string newString = fullString + "\n" + name + "#" + serialOn + "#" + serialOff;
            writeFile(newString);
            populateList();
        }

        void writeFile(string data)
        {
            if(!(Directory.Exists(directoryName)))
                Directory.CreateDirectory(directoryName);
            File.WriteAllText(filename, data);
        }

        void turnOnDevice(string deviceName, string serialCode) {
            addToDeviceQueue(serialCode);
            //Toast.MakeText(this, deviceName + " ON\nSerial: " + serialCode,ToastLength.Short).Show();
        }
        void turnOffDevice(string deviceName, string serialCode) {
            addToDeviceQueue(serialCode);
            //Toast.MakeText(this, deviceName + " OFF\nSerial: " + serialCode, ToastLength.Short).Show();
        }

       async void addToDeviceQueue(string serialCode)
        {
            try
            {
                WebRequest request = WebRequest.Create(Resources.GetString(Resource.String.serverURL) + "appHandler.php?appToDeviceSerial=" + serialCode);
                request.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = await request.GetResponseAsync();
                response.Close();
                Toast.MakeText(this, "Device status changed!", ToastLength.Short).Show();
            }
            catch (System.Exception)
            {
                Toast.MakeText(this, "Network Failure!", ToastLength.Short).Show();
            }

        }
    }
}