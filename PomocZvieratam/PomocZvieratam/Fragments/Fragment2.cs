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

namespace PomocZvieratam.Fragments
{
    public class Fragment2 : SupportFragment, ILocationListener, IOnMapReadyCallback
    {
       
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here


            checkGPSLocationService();
            InitializeLocationManager();

        }
        

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            
                var view = inflater.Inflate(Resource.Layout.MapFragment, null);
                _locationText = view.FindViewById<TextView>(Resource.Id.location_text);
                view.FindViewById<TextView>(Resource.Id.captureLocation).Click += AddressButton_OnClick;


                //Button btnNajdiPolohu = view.FindViewById<Button>(Resource.Id.btnNajdiPolohu);

                //btnNajdiPolohu.Click += BtnNajdiPolohu_Click;
                SetUpMap();
                return view;
           
            //_addressText = view.FindViewById<TextView>(Resource.Id.address_text);
           
        }

        public GoogleMap mMap;
        #region Capture location Activity
        static readonly string TAG = "X:" + typeof(Activity).Name;
        TextView _addressText;
        Location _currentLocation;
        LocationManager _locationManager;
        TextView _locationText;
        public string _addressOfDevice;
        string _locationProvider;

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
                var startGPSintent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                StartActivity(startGPSintent);
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
            _currentLocation = location;
            if (_currentLocation == null)
            {
                _locationText.Text = "Unable to determine your location. Try again in a short while.";
            }
            else
            {
                _locationText.Text = string.Format("{0:f6},{1:f6}", _currentLocation.Latitude, _currentLocation.Longitude);
                Address address = await ReverseGeocodeCurrentLocation();
                //DisplayAddress(address);

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
            Log.Debug(TAG, "Listening for location updates using " + _locationProvider + ".");
        }

        public override void OnPause()
        {
            base.OnPause();
            _locationManager.RemoveUpdates(this);
            Log.Debug(TAG, "No longer listening for location updates.");
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

            // Show actual position
            LatLng latlng = new LatLng(_currentLocation.Latitude, _currentLocation.Longitude); // Kosice
            CameraUpdate camera = CameraUpdateFactory.NewLatLngZoom(latlng, mMap.MaxZoomLevel-5);
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

      

        //private void BtnNajdiPolohu_Click(object sender, System.EventArgs e)
        //{
        //    var geoUri = Android.Net.Uri.Parse("geo:16.053200,08.20284?q=16.053200, 108.20284");
        //    Intent mapIntent = new Intent(Intent.ActionView, geoUri);
        //    StartActivity(mapIntent);
        //}


    }
}