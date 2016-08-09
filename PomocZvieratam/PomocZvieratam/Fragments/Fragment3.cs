using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Collections;
using System.Collections.Generic;
using SupportFragment = Android.Support.V4.App.Fragment;


namespace PomocZvieratam.Fragments
{
    public class Fragment3 : SupportFragment
    {

        Spinner spinnerAction;
        ArrayAdapter adapter;
        ArrayList arrayList = new ArrayList();
        RadioButton rbOdchytZvierat;
        EditText etPopis;




        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            arrayList.Add("Spoloèenské zvieratá");
            arrayList.Add("Vo¾ne žijúce zvieratá");
            arrayList.Add("Hospodárske zvieratá");
            arrayList.Add("Chránené živoèíchy");
            arrayList.Add("Po¾ovná zver");
            arrayList.Add("Hlodavce");
            arrayList.Add("Iné");

            adapter = new ArrayAdapter(Activity , Android.Resource.Layout.SimpleSpinnerDropDownItem, arrayList);
            
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            View view = inflater.Inflate(Resource.Layout.Fragment3, container, false);
            view.RequestFocus();

            rbOdchytZvierat = view.FindViewById<RadioButton>(Resource.Id.rbOdchytZvierat);
            etPopis = view.FindViewById<EditText>(Resource.Id.etPopis);
            spinnerAction = view.FindViewById<Spinner>(Resource.Id.spinnerAkcia);
            spinnerAction.SetSelection(0);
            spinnerAction.Adapter = adapter;
            spinnerAction.ItemSelected += SpinnerAction_ItemSelected;

            return view;
        }

        private void SpinnerAction_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Toast.MakeText(this.Context, arrayList[e.Position].ToString(), ToastLength.Short).Show();
        }
    }
}