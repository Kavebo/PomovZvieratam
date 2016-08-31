
using Android.App;
using Android.OS;
using Android.Content.PM;

namespace PomocZvieratam
{
    [Activity(Label = "ANIMAL RESCUE SK", MainLauncher = true,
        Theme = "@style/Theme.Splash", ScreenOrientation = ScreenOrientation.Portrait, NoHistory = true) ]
    public class SplashScreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            StartActivity(typeof(MainActivity));
        }
    }
}