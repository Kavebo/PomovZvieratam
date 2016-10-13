using Android.App;
using Android.Views;
using Android.OS;
using Android.Support.V4.Widget;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;
using SupportToolBar = Android.Support.V7.Widget.Toolbar;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.App;
using System.Collections.Generic;
using Java.Lang;
using PomocZvieratam.Fragments;
using System;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using Android.Widget;
using System.Net.NetworkInformation;
using Android.Content;
using Android.Content.PM;

namespace PomocZvieratam
{
    [Activity(Label = "ANIMAL RESCUE SK", MainLauncher = false, Icon = "@drawable/dog_icon", Theme = "@style/Theme.DesignDemo", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, ICommunicator
    {
        private DrawerLayout mDrawerLayout;
        RequestedAction requestedAction = new RequestedAction();
        ProgressDialog progressDialog;
        TabAdapter adapter;
        ViewPager viewPager;



        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            SupportToolBar toolBar = FindViewById<SupportToolBar>(Resource.Id.toolBar);
            SetSupportActionBar(toolBar);

            SupportActionBar ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            ab.SetDisplayHomeAsUpEnabled(true);

            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            if (navigationView != null)
            {
                SetUpDrawerContent(navigationView);
            }


            TabLayout tabs = FindViewById<TabLayout>(Resource.Id.tabs);
            viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);

            SetUpViewPager(viewPager);
            tabs.SetupWithViewPager(viewPager);

           

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += (o, e) =>
            {
                View anchor = o as View;
                Snackbar.Make(anchor, "Potvrdenie", Snackbar.LengthLong)
                .SetAction("Odoslať", v =>
                {
                    if (IsInternetAvailable())
                    {
                        if (requestedAction._logntitude == null || requestedAction._latitude == null)
                            Toast.MakeText(this, "Vyplnte lokalitu", ToastLength.Short).Show();
                        else if (requestedAction._imageFile == null)
                            Toast.MakeText(this, "Pridajte fotku", ToastLength.Short).Show();
                        else if (requestedAction._infoAboutAction == "")
                            Toast.MakeText(this, "Pridajte Popis", ToastLength.Short).Show();
                        else if (requestedAction._telephone == "")
                            Toast.MakeText(this, "Zadajte telefónne číslo", ToastLength.Short).Show();
                        else
                        {
                            WebClient client = new WebClient();
                            Uri uri = new Uri("http://animalrescue.koled.sk/CreateAction.php");
                            //Uri uri = new Uri("http://127.0.0.1/CreateAction.php");
                            NameValueCollection parameters = new NameValueCollection();

                            parameters.Add("_typeOfAction", requestedAction._typeOfAction);
                            parameters.Add("_typeOfAnimal", requestedAction._typeOfAnimal);
                            parameters.Add("_latitude", requestedAction._latitude.Replace(',','.'));
                            parameters.Add("_longtitude", requestedAction._logntitude.Replace(',', '.'));
                            parameters.Add("_infoAboutAction", requestedAction._infoAboutAction);
                            parameters.Add("_phoneNumber", requestedAction._telephone);
                            parameters.Add("_imageFile", Convert.ToBase64String(requestedAction._imageFile));

                            client.UploadValuesAsync(uri, parameters);
                            client.UploadValuesCompleted += Client_UploadValuesCompleted;
                            progressDialog = new ProgressDialog(this);
                            progressDialog.SetMessage("Nahrávanie...");
                            progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                            progressDialog.Progress =0;
                            progressDialog.Max = 100;
                            progressDialog.Show();
                        }
                    }
                })

                .Show();
            };


            //var uiOptions = SystemUiFlags.HideNavigation | SystemUiFlags.Immersive | SystemUiFlags.Fullscreen;
            //Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;

        }
        // Hide navbar and icons
        //public override void OnUserInteraction()
        //{
        //    base.OnUserInteraction();
        //    var uiOptions = SystemUiFlags.HideNavigation | SystemUiFlags.Immersive |SystemUiFlags.Fullscreen;
        //    Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;
        //}
        //************************************ Function to find out if internet is vailable ****************************
        public bool IsInternetAvailable()
        {
            try
            {
                Ping myPing = new Ping();
                string host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 500;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return true;
            }
            catch 
            {
                Toast.MakeText(this, "Internet nie je k dispozícií", ToastLength.Short).Show();
                return false;
            }
        }
        //************************************** Values was uploaded to server correctly action **************************
        private void Client_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                //requestedAction._infoAboutAction = "";
                progressDialog.Dismiss();

            });

        }


        //***************************************** Adding fragment to ViewPager **********************************
        private void SetUpViewPager(ViewPager viewPager)
        {
            adapter = new TabAdapter(SupportFragmentManager);
            adapter.AddFragment(new Fragment3(), "Popis");
            adapter.AddFragment(new Fragment2(), "Poloha");
            adapter.AddFragment(new Fragment1(), "Fotografia");

            viewPager.Adapter = adapter;
        }

        //*********************************** Hamburger button to draw left drawer *********************************
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    mDrawerLayout.OpenDrawer((int)GravityFlags.Left);
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }

        }
        private void SetUpDrawerContent(NavigationView navigationView)
        {
            navigationView.NavigationItemSelected += (object sender, NavigationView.NavigationItemSelectedEventArgs e) =>
            {
                SupportFragmentManager fragmentManager = SupportFragmentManager;
                e.MenuItem.SetChecked(true);
                
                mDrawerLayout.CloseDrawers();

                switch (e.MenuItem.ItemId)
                {
                    
                    case Resource.Id.nav_home:
                        Toast.MakeText(this, "Popis!", ToastLength.Short).Show();
                        viewPager.SetCurrentItem(0, true);
                        e.MenuItem.SetChecked(false);
                        //e.MenuItem.Icon.
                        return;
                    case Resource.Id.nav_messages:
                        Toast.MakeText(this, "Poloha!", ToastLength.Short).Show();
                        viewPager.SetCurrentItem(1, true);
                        e.MenuItem.SetChecked(false);
                        return;
                    case Resource.Id.nav_friends:
                        Toast.MakeText(this, "Fotografia!", ToastLength.Short).Show();
                        viewPager.SetCurrentItem(2, true);
                        e.MenuItem.SetChecked(false);
                        return;
                    case Resource.Id.nav_mail:
                        Intent email = new Intent(Intent.ActionSend);
                        email.PutExtra(Intent.ExtraEmail,new string[] { "odchytzvierat@odchytzvierat.eu" });
                        email.PutExtra(Intent.ExtraSubject, "Pomoc Zvieratam");
                        email.PutExtra(Intent.ExtraText, "Text...");
                        email.SetType("text/email");
                        e.MenuItem.SetChecked(false);
                        StartActivity(email);
                        return;
                    case Resource.Id.nav_about:
                        Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                        alert.SetTitle("Info");
                        alert.SetMessage("Aplikácia vytvorená pre firmu \nTD, s.r.o Košice. \nhttp://www.odchytzvierat.eu\n\nVytvoril v roku 2016 Bobo. ");
                        e.MenuItem.SetChecked(false);
                        alert.SetPositiveButton("OK", (senderAlert, args) => {
                            // write your own set of instructions
                        });

                        //run the alert in UI thread to display in the screen
                        RunOnUiThread(() => {
                            alert.Show();
                        });
                        return;

                }
            };
        }
        // *************************************** ICommunicator functions *******************************************
        public void SendPhoto(byte[] _image)
        {
            requestedAction._imageFile = (_image);
            Console.WriteLine(">>>>>>>>>>> Image bol ulozeny do Class");
        }

        public void SendLocation(string _latitude, string _longtitude)
        {
            requestedAction._latitude = _latitude;// , _longtitude);
            requestedAction._logntitude = _longtitude;
        }

        public void SendInfo(string _typeOfAction, string _typeOfAnimal, string _info, string _phoneNumber)
        {
            requestedAction._typeOfAction = _typeOfAction;
            requestedAction._typeOfAnimal = _typeOfAnimal;
            requestedAction._infoAboutAction = _info;
            requestedAction._telephone = _phoneNumber;
        }
        //******************************************* Tab Adapter set up Fragments adapter *****************************
        private class TabAdapter : FragmentPagerAdapter
        {
            public List<SupportFragment> Fragments { get; set; }
            public List<string> FragmentNames { get; set; }

            public TabAdapter(SupportFragmentManager sfm) : base(sfm)
            {
                Fragments = new List<SupportFragment>();
                FragmentNames = new List<string>();
            }

            public void AddFragment(SupportFragment fragment, string name)
            {
                Fragments.Add(fragment);
                FragmentNames.Add(name);
            }

            public override int Count
            {
                get
                {
                    return Fragments.Count;

                }
            }

            public override SupportFragment GetItem(int position)
            {
                return Fragments[position];
            }
            public override ICharSequence GetPageTitleFormatted(int position)
            {
                try
                {
                    return new Java.Lang.String(FragmentNames[position]);
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(">>>>>>>>>>>>>>>>>. nieco sa posralo pri Jave string:" + e.Message);
                    return base.GetPageTitleFormatted(0);
                }
            }
        }
    }
}

