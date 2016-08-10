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

namespace PomocZvieratam
{
    [Activity(Label = "PomocZvieratam", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/Theme.DesignDemo")]
    public class MainActivity : AppCompatActivity, ICommunicator
    {
        private DrawerLayout mDrawerLayout;
        RequestedAction requestedAction = new RequestedAction();

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
            if(navigationView != null)
            {
                SetUpDrawerContent(navigationView);
            }


            TabLayout tabs = FindViewById<TabLayout>(Resource.Id.tabs);
            ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);

            SetUpViewPager(viewPager);
            tabs.SetupWithViewPager(viewPager);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += (o, e) =>
            {
                View anchor = o as View;
                Snackbar.Make(anchor, "Potvrdenie", Snackbar.LengthLong)
                .SetAction("Odoslať", v =>
                {
                    //Sent class to database
                    //Intent intent new Intent();
                    //Console.WriteLine(">>>>>>>>>>> Clasa ma tieto udaje:");
                    //Console.WriteLine(">>>>>>>>>>> TypeOfAction: " + requestedAction._typeOfAction);
                    //Console.WriteLine(">>>>>>>>>>> TypeOfAnimal: " + requestedAction._typeOfAnimal);
                    //Console.WriteLine(">>>>>>>>>>> Location: " + requestedAction._latitude +
                    //    " " + requestedAction._logntitude );
                    //Console.WriteLine(">>>>>>>>>>> Popis: " + requestedAction._infoAboutAction);
                    //Console.WriteLine(">>>>>>>>>>> Image :" + requestedAction._imageFile.Length);

                    WebClient client = new WebClient();
                    Uri uri = new Uri("http://myprestage.euweb.cz/CreateAction.php");
                    NameValueCollection parameters = new NameValueCollection();

                    parameters.Add("_typeOfAction", requestedAction._typeOfAction);
                    parameters.Add("_typeOfAnimal", requestedAction._typeOfAnimal);
                    parameters.Add("_latitude", requestedAction._latitude);
                    parameters.Add("_longtitude", requestedAction._logntitude);
                    parameters.Add("_infoAboutAction", requestedAction._infoAboutAction);

                    client.UploadValuesAsync(uri, parameters);
                    client.UploadValuesCompleted += Client_UploadValuesCompleted;
                })
                .Show();
            };

            
        }

        private void Client_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                string id = Encoding.UTF8.GetString(e.Result);
                int newID = 0;
                int.TryParse(id, out newID);
                Console.WriteLine(">>>>>>>>>>>>>>>>>>> new ID is: " + id);
                
            });
            
        }


        //********************************Adding fragment to ViewPager **********************************
        private void SetUpViewPager(ViewPager viewPager)
        {
            TabAdapter adapter = new TabAdapter(SupportFragmentManager);
            adapter.AddFragment(new Fragment3(), "Popis");
            adapter.AddFragment(new Fragment2(), "Poloha");
            adapter.AddFragment(new Fragment1(), "Fotografia");
            
            viewPager.Adapter = adapter;
        }


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
                e.MenuItem.SetChecked(true);
                mDrawerLayout.CloseDrawers();
            };
        }

        public void SendPhoto(byte[] _image)
        {
            requestedAction.SetImageSource(_image);
            Console.WriteLine(">>>>>>>>>>> Image bol ulozeny do Class");
        }

        public void SendLocation(string _latitude, string _longtitude)
        {
            requestedAction.SetLocation(_latitude, _longtitude);
        }

        public void SendInfo(string _typeOfAction, string _typeOfAnimal, string _info)
        {
            requestedAction.SetTypeOfAction(_typeOfAction);
            requestedAction.SetTypeOfAnimal(_typeOfAnimal);
            requestedAction.SetInfoAboutAction(_info);
        }

        private class TabAdapter : FragmentPagerAdapter
        {
            public List<SupportFragment> Fragments { get; set; }
            public List<string> FragmentNames { get; set; }

            public TabAdapter (SupportFragmentManager sfm) : base (sfm)
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
                return new Java.Lang.String(FragmentNames[position]);
            }
        }
    }
}

