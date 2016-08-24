using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace PomocZvieratam.Fragments
{
    interface ICommunicator
    {

        void SendPhoto(byte[] image);
        void SendLocation(string latitude, string longtitude);
        void SendInfo(string typeOfAction, string typeOfAnimal, string info, string phoneNumber);
        
    }
}
