using System;
using System.Text;
using Android.App;
using Android.Content;
using Android.Nfc;
using Android.Nfc.CardEmulators;
using Android.Nfc.Tech;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Remote_hotel_customer
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        public NfcAdapter _nfcAdapter = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            _nfcAdapter = NfcAdapter.GetDefaultAdapter(this);

            //Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            //SetSupportActionBar(toolbar);
            //FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            //fab.Click += FabOnClick;
            Button setNdef = (Button)FindViewById(Resource.Id.write_tag_button);
            setNdef.Click += (sender, e) =>
             {
                EditText getNdefString = (EditText)FindViewById(Resource.Id.editText1);
                String test = getNdefString.Text;

                Intent intent = new Intent(Application.Context,
                    typeof(CardEmulatorService));
                intent.PutExtra("ndefMessage", test);
                StartService(intent);

            };
        }
        public virtual void SetOnClickListener(View.IOnClickListener l)
        {

        }

        public void OnClick(View v)
        {
            throw new NotImplementedException();
        }
    }
}




