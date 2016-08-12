
using System;
using Android.OS;
using Android.Views;
using SupportFragment = Android.Support.V4.App.Fragment;
using Android.Widget;
using Android.Graphics;
using Uri = Android.Net.Uri;
using Android.Content;
using Android.Provider;
using Android.Content.PM;
using System.Collections.Generic;
using BitmapHelper;
using System.IO;
using Square.Picasso;

namespace PomocZvieratam.Fragments
{
    public class Fragment1 : SupportFragment
    {

        ImageView photoImageView;
        ICommunicator iComm;
        public byte[] fileEncoded;
        //private InputMethodManager imm;
        // MediaFile file;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            iComm = (ICommunicator)Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            

            View view = inflater.Inflate(Resource.Layout.Fragment1, container, false);

            Button captureImage = view.FindViewById<Button>(Resource.Id.captureImage);
            photoImageView = view.FindViewById<ImageView>(Resource.Id.photoImageView);

            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            if (savedInstanceState != null)
            {
                fileEncoded = savedInstanceState.GetByteArray("Image");
                App.bitmap = BitmapFactory.DecodeByteArray(fileEncoded, 0, fileEncoded.Length);
                MemoryStream memStream = new MemoryStream();
                photoImageView.SetImageBitmap(App.bitmap);
                App.bitmap = null;
                savedInstanceState.Clear();
            }

            // If there is App to take photo Take a Picture
            if (IsThereAnAppToTakePicture())
            {
                CreateDirectoryForPicture();
                captureImage.Click += TakePicture;
            }
            return view;
        }

        #region Another methon to take a picture
        //        public async void TakePicture(object sender, EventArgs e)
        //        {
        //            string fileName = "";
        //            Log.Info("ABCDDebug", "Take picture was hit");
        //            var picker = new MediaPicker(this.Context);
        //            if (!picker.IsCameraAvailable)
        //                System.Console.WriteLine("No camera");
        //            else
        //            {
        //                try
        //                {
        //                    //Fortate Date to "yyyy_MM_dd_HH_mm_ss" to be used as a file name
        //                    Java.Text.SimpleDateFormat formatter = new Java.Text.SimpleDateFormat("yyyy_MM_dd_HH_mm_ss");
        //                    Java.Util.Date now = new Java.Util.Date();
        //                    fileName = formatter.Format(now) + ".jpg";
        //                    System.Console.WriteLine("Cas pre subor:" + fileName);



        //                   //Take photo async 
        //#pragma warning disable CS0618 // Type or member is obsolete
        //                    file = await picker.TakePhotoAsync(new StoreCameraMediaOptions
        //                    {
        //                        DefaultCamera = CameraDevice.Rear,
        //                        Name = fileName,
        //                        Directory = "MediaPickerSample"

        //                    });
        //#pragma warning restore CS0618 // Type or member is obsolete


        //                    System.Console.WriteLine("Cesta k suboru je:" + file.Path);

        //                    var stream = file.GetStream();
        //                    Bitmap bitmap = BitmapFactory.DecodeFile(file.Path);
        //                    photoImageView.SetImageBitmap(bitmap);
        //                    //Bitmap bm = BitmapFactory.DecodeFile(@"/storage/emulated/0/Android/data/PomocZvieratam.PomocZvieratam/files/Pictures/MediaPickerSample/"+fileName);


        //                }
        //                catch (Android.Support.V4.OS.OperationCanceledException)
        //                {
        //                    System.Console.WriteLine("Canceled");
        //                }
        //                //using (var bitmap = BitmapFactory.DecodeFile(@"/storage/emulated/0/Android/data/PomocZvieratam.PomocZvieratam/files/Pictures/MediaPickerSample/" + fileName))
        //                //    photoImageView.SetImageBitmap(bitmap);


        //            }


        //            //Method to create intent to take the picture by MediaStore.ActionImageCapture

        //        }
        #endregion

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            System.Console.WriteLine(">>>>>>>>>>>>On activity result was called");
            //// Make it available in the gallery
            //Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            //Uri contentUri = Uri.FromFile(App._file);
            //mediaScanIntent.SetData(contentUri);
            //Context.SendBroadcast(mediaScanIntent);

            // Display in ImageView. We will resize the bitmap to fit the dispaly
            // Loading the full sized image will consume to much memory
            // and cause the application to crash

          

            int height = Resources.DisplayMetrics.HeightPixels;
            int width = photoImageView.Height;
            App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
            if (App.bitmap != null)
            {
                MemoryStream memStream = new MemoryStream();
                App.bitmap.Compress(Bitmap.CompressFormat.Jpeg, 60, memStream);
                fileEncoded = memStream.ToArray();
                
                iComm.SendPhoto(fileEncoded);           // send foto to class via Interface
                //App.bitmap = null;
            }
            // Dispose of the Java side Bitmap
            GC.Collect();
            
        }
        
        // *************************** Picture taking Methods ******************************
        private bool IsThereAnAppToTakePicture()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                Activity.PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }
        private void TakePicture(object sender, EventArgs e)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            App._file = new Java.IO.File(App._dir, string.Format("Anim_{0}.jpg", Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(App._file));
            StartActivityForResult(intent, 0);
        }
        
        private void CreateDirectoryForPicture()
        {
            App._dir = new Java.IO.File(
                Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryPictures), "CameraAppDemo");
            if (!App._dir.Mkdirs())
            {
                App._dir.Mkdirs();
            }
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutByteArray("Image", fileEncoded);
            fileEncoded = null;
        }
        public override void OnPause()
        {
            base.OnPause();

        }
        public override void OnResume()
        {
            base.OnResume();
            if (App.bitmap != null)
            {
                photoImageView.SetImageBitmap(App.bitmap);
                App.bitmap = null;
            }
            Console.WriteLine(">>>>>>>>>>>>> Resume na fragment");
        }
    }



    public static class App
    {
        public static Java.IO.File _file;
        public static Java.IO.File _dir;
        public static Bitmap bitmap;
    }


}

