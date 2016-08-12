using System;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SupportFragment = Android.Support.V4.App.Fragment;
using Android.App;
using System.Collections.Generic;
using System.Linq;
using Android.Util;
using System.Threading.Tasks;
using System.Text;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.App.Usage;
using System.Net.NetworkInformation;
using Android.Support.Design.Widget;
using System.Threading;

namespace PomocZvieratam.Fragments
{
    public class Fragment2 : SupportFragment, ILocationListener, IOnMapReadyCallback
    {

        public GoogleMap mMap;
        ICommunicator iComm;
        static readonly string TAG = "X:" + typeof(Activity).Name;
        //TextView _addressText;
        Location _currentLocation;
        LocationManager _locationManager;
        //TextView _locationText;
        public string _addressOfDevice;
        string _locationProvider;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            checkGPSLocationService();
            InitializeLocationManager();

        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            iComm = (ICommunicator)Activity;
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            var view = inflater.Inflate(Resource.Layout.MapFragment, null);

            FloatingActionButton fabGps = view.FindViewById<FloatingActionButton>(Resource.Id.fabGps);
            fabGps.Click += AddressButton_OnClick;

            IsNetworkAccessable();
            
            return view;
        }
        

        public void IsNetworkAccessable()
        {
            try
            {
                if (IsInternetAvailable())
                { SetUpMap(); }
                else
                {
                    AlertDialog.Builder builder = new AlertDialog.Builder(Context);
                    builder.SetTitle("Chyba");
                    builder.SetMessage("Internet nie je dostupný, chcete sa pripojit?.");
                    builder.SetCancelable(false);
                    builder.SetPositiveButton("Áno", delegate
                    {
                        var startNetworkIntent = new Intent(Android.Provider.Settings.ActionWifiSettings);
                        StartActivity(startNetworkIntent);
                        builder.SetNegativeButton("Nie", (s, e) =>
                        { Toast.MakeText(Context, "Nedostupný internet, mapa sa nenaèíta", ToastLength.Short).Show(); });
                    });
                    builder.Show();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(">>>>>>>>>>>> Chyba pri hladani internetu: " + e.Message);
            }
        }

        public bool IsInternetAvailable()
        {
            try
            {
                Ping myPing = new Ping();
                string host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        #region Capture location Activity

        private void SetUpMap()
        {
            if (mMap == null)
            {
                var mapFragment = (SupportMapFragment)ChildFragmentManager.FindFragmentById(Resource.Id.map);
                mapFragment.GetMapAsync(this);
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {

            LatLng latlng = new LatLng(48.6776384, 21.002087); // Kosice
            mMap = googleMap;
            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latlng, 10);
            mMap.MoveCamera(camera);

        }

        //Check if Location service is enabled , if not open GPS settings
        public bool checkGPSLocationService()
        {
            LocationManager _locationManager = Activity.GetSystemService(Context.LocationService) as LocationManager;
            if (!_locationManager.IsProviderEnabled(LocationManager.GpsProvider))
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(Context);
                builder.SetTitle("Chyba");
                builder.SetMessage("GPS nie je povolná, chcete ju povoli?.");
                //builder.SetCancelable(false);
                builder.SetPositiveButton("Áno", delegate {
                    var startGPSintent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                    StartActivity(startGPSintent);
                    builder.SetNegativeButton("Nie", (s, e) =>
                    { Toast.MakeText(Context, "GPS nie je povolná, nájdite vašu polohu", ToastLength.Short).Show(); });
                });
                builder.Show();
                //var startGPSintent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                //StartActivity(startGPSintent);
                return true;
            }
            else
                return false;
        }

        void InitializeLocationManager()
        {
            _locationManager = (LocationManager)Activity.GetSystemService(Context.LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };

            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);


            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                _locationProvider = string.Empty;

            }
            Log.Debug(TAG, "Using " + _locationProvider + ".");
        }

        public async void OnLocationChanged(Location location)
        {
            try
            {
                _currentLocation = location;
                if (_currentLocation != null && IsInternetAvailable())
                {
                    Address address = await ReverseGeocodeCurrentLocation();
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(">>>>>>>>>>>>> Chyba pri OnLocationChanged: " + e.Message);
            }
        }

        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {

            Log.Debug(TAG, "{0}, {1}", provider, status);
        }
        public override void OnResume()
        {
            base.OnResume();
            if (_locationProvider.Any())
            {
                _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
            }
            if (IsInternetAvailable())
            {
                SetUpMap();
            }
            Log.Debug(TAG, "Listening for location updates using " + _locationProvider + ".");
        }

        public override void OnPause()
        {
            base.OnPause();
            _locationManager.RemoveUpdates(this);
        }

        // Button to set the actual Adress and put marker on it
        async void AddressButton_OnClick(object sender, EventArgs eventArgs)
        {
            if (_currentLocation == null)
            {
                _addressOfDevice = "Can't determine the current address. Try again in a few minutes.";
                //_addressText.Text = "Can't determine the current address. Try again in a few minutes.";
                return;
            }
            //Send actual position to main activity
            iComm.SendLocation(string.Format("{0:f6}", _currentLocation.Latitude),
                string.Format("{0:f6}", _currentLocation.Longitude));
            
            // Show actual position
            LatLng latlng = new LatLng(_currentLocation.Latitude, _currentLocation.Longitude); // Kosice
            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latlng, mMap.MaxZoomLevel - 5);
            mMap.MoveCamera(camera);



            Address address = await ReverseGeocodeCurrentLocation();
            DisplayAddress(address);

            mMap.Clear();
            MarkerOptions options = new MarkerOptions()
                .SetPosition(latlng)
                .SetTitle("Aktualna Poloha")
                .SetSnippet(_addressOfDevice)
                .Draggable(true);

            mMap.AddMarker(options);
        }

        async Task<Address> ReverseGeocodeCurrentLocation()
        {
            Geocoder geocoder = new Geocoder(this.Context);
            IList<Address> addressList =
                await geocoder.GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 10);

            Address address = addressList.FirstOrDefault();
            return address;
        }

        void DisplayAddress(Address address)
        {
            if (address != null)
            {
                StringBuilder deviceAddress = new StringBuilder();
                for (int i = 0; i < address.MaxAddressLineIndex; i++)
                {
                    deviceAddress.AppendLine(address.GetAddressLine(i));
                }
                // Remove the last comma from the end of the address.
                _addressOfDevice = deviceAddress.ToString();
                //_addressText.Text = _adressOfDevice;
            }
            else
            {
                _addressOfDevice = "Unable to determine the address. Try again in a few minutes.";
                // _addressText.Text = "Unable to determine the address. Try again in a few minutes.";
            }
        }


        #endregion


    }
}