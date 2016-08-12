using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using System.Collections;
using SupportFragment = Android.Support.V4.App.Fragment;


namespace PomocZvieratam.Fragments
{
    public class Fragment3 : SupportFragment
    {

        Spinner spinnerAction;
        ArrayAdapter adapter;
        ArrayList arrayList = new ArrayList();
        RadioGroup rgTypeOfAction;
        RadioButton rbOdchytZvierat;
        RadioButton rbZberMrtvychZvierat;
        RadioButton rbDeratizacia;
        EditText etPopis;
        ICommunicator iComm;
        public string _typeOfAnimal = "";
        public string _typeOfAction = "Odchyt zvierat";
        public string _information = "";
        private InputMethodManager imm;


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

            adapter = new ArrayAdapter(Activity, Android.Resource.Layout.SimpleSpinnerDropDownItem, arrayList);


            // Create your fragment here
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            iComm = (ICommunicator)Activity;
        }
        public override void OnPause()
        {
            base.OnPause();
            imm.HideSoftInputFromWindow(etPopis.WindowToken, 0);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            View view = inflater.Inflate(Resource.Layout.Fragment3, container, false);
            view.RequestFocus();

            rgTypeOfAction = view.FindViewById<RadioGroup>(Resource.Id.rgTypeOfAction);
            rbOdchytZvierat = view.FindViewById<RadioButton>(Resource.Id.rbOdchytZvierat);
            rbZberMrtvychZvierat = view.FindViewById<RadioButton>(Resource.Id.rbZberUhynutychZvierat);
            rbDeratizacia = view.FindViewById<RadioButton>(Resource.Id.rbDeratizaciaZvierat);
            etPopis = view.FindViewById<EditText>(Resource.Id.etPopis);
            spinnerAction = view.FindViewById<Spinner>(Resource.Id.spinnerAkcia);

            spinnerAction.RequestFocus();
            spinnerAction.SetSelection(0);
            spinnerAction.Adapter = adapter;
            spinnerAction.ItemSelected += SpinnerAction_ItemSelected;

            rgTypeOfAction.CheckedChange += (object sender, RadioGroup.CheckedChangeEventArgs e) =>
            {
                RadioButton checkedRadioButton = view.FindViewById<RadioButton>(rgTypeOfAction.CheckedRadioButtonId);
                _typeOfAction = checkedRadioButton.Text;
                iComm.SendInfo(_typeOfAction, _typeOfAnimal, _information);
                imm.HideSoftInputFromWindow(etPopis.WindowToken, 0);
            };
            etPopis.AfterTextChanged += EtPopis_AfterTextChanged;

            imm = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
            

            return view;
        }

        private void EtPopis_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            _information = etPopis.Text;
            iComm.SendInfo(_typeOfAction, _typeOfAnimal, _information);
            
        }
       
        private void SpinnerAction_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            //Toast.MakeText(Context, arrayList[e.Position].ToString(), ToastLength.Short).Show();
            _typeOfAnimal = arrayList[e.Position].ToString();
            iComm.SendInfo(_typeOfAction, _typeOfAnimal, _information);         // Send info to class when something in spinner is selected
            imm.HideSoftInputFromWindow(etPopis.WindowToken, 0);                //Hide keyboard when something in spinner is selected
        }


    }
}