using Android.Graphics;
using Android.Media;

namespace BitmapHelper
{
    public static class BitmapHelpers
    {
        public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
        {
            //First we get the dimenstions of the file on disk
            BitmapFactory.Options option = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(fileName, option);

            //Next we calculate the ratio that we need to resize the image by
            // in order to fit the request dimensions.

            int outHeight = option.OutHeight;
            int outWidth = option.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight ? outHeight / height : outWidth / width;

            }

            //Now we will load the image and have BitmapFactory resize it for us
            option.InSampleSize = inSampleSize;
            option.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, option);

            //*******************************Rotate function *********************************//
            #region Rotate Image if its needed
            //Rotate image if it was captured in ladscape or portrait

            Matrix mtx = new Matrix();
            ExifInterface exif = new ExifInterface(fileName);
            var orientation = (Orientation)exif.GetAttributeInt(ExifInterface.TagOrientation, (int)Orientation.Undefined);

            switch (orientation)
            {
                case Orientation.Undefined: // Nexus 7 landscape...
                    break;
                case Orientation.Normal: // landscape
                    break;
                case Orientation.FlipHorizontal:
                    break;
                case Orientation.Rotate180:
                    mtx.PreRotate(180);
                    resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, mtx, false);
                    mtx.Dispose();
                    mtx = null;
                    break;
                case Orientation.FlipVertical:
                    break;
                case Orientation.Transpose:
                    break;
                case Orientation.Rotate90: // portrait
                    mtx.PreRotate(90);
                    resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, mtx, false);
                    mtx.Dispose();
                    mtx = null;
                    break;
                case Orientation.Transverse:
                    break;
                case Orientation.Rotate270: // might need to flip horizontally too...
                    mtx.PreRotate(270);
                    resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, mtx, false);
                    mtx.Dispose();
                    mtx = null;
                    break;
                default:
                    mtx.PreRotate(90);
                    resizedBitmap = Bitmap.CreateBitmap(resizedBitmap, 0, 0, resizedBitmap.Width, resizedBitmap.Height, mtx, false);
                    mtx.Dispose();
                    mtx = null;
                    break;
                    
            }

            return resizedBitmap;
        }
        #endregion


    }

}