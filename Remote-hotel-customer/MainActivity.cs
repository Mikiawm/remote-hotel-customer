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
    public class MainActivity : Activity, NfcAdapter.ICreateNdefMessageCallback, NfcAdapter.IOnNdefPushCompleteCallback
    {
        public NfcAdapter _nfcAdapter = null;
        TextView textView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            _nfcAdapter = NfcAdapter.GetDefaultAdapter(this);
            textView = (TextView)FindViewById(Resource.Id.textView1);
            //Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            //SetSupportActionBar(toolbar);
            //FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            //fab.Click += FabOnClick;
            Button setNdef = (Button)FindViewById(Resource.Id.write_tag_button);
            _nfcAdapter.SetNdefPushMessageCallback(this, this);
            _nfcAdapter.SetOnNdefPushCompleteCallback(this, this);
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

        public void OnNdefPushComplete(NfcEvent e)
        {
            textView.Text = "new intent";
            //throw new NotImplementedException();
        }

        public NdefMessage CreateNdefMessage(NfcEvent e) {
            String text = ("Beam me up, Android!\n\n" +
                    "Beam Time: " + DateTime.Now);
            NdefMessage msg = new NdefMessage(
                    new NdefRecord[] {  NdefRecord.CreateMime(
                        "application/vnd.com.example.android.beam", Encoding.ASCII.GetBytes(text.Length.ToString("X")))
                        /**
                         * The Android Application Record (AAR) is commented out. When a device
                         * receives a push with an AAR in it, the application specified in the AAR
                         * is guaranteed to run. The AAR overrides the tag dispatch system.
                         * You can add it back in to guarantee that this
                         * activity starts when receiving a beamed message. For now, this code
                         * uses the tag dispatch system.
                         */
                        ,NdefRecord.CreateApplicationRecord("com.example.android.beam")
                    });
            return msg;
        }
        public void OnNewIntent(Intent intent)
        {
            textView.Text = "new intent";
            // onResume gets called after this to handle the intent
             //SetIntent(intent);
        }

        /**
         * Parses the NDEF Message from the intent and prints to the TextView
         */
        void ProcessIntent(Intent intent)
        {
            textView = (TextView)FindViewById(Resource.Id.textView1);
            IParcelable[] rawMsgs = intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);
            // only one message sent during the beam
            NdefMessage msg = (NdefMessage)rawMsgs[0];
            // record 0 contains the MIME type, record 1 is the AAR, if present
            var payload = msg.GetRecords()[0].GetPayload();
            
            textView.SetText(Encoding.UTF8.GetString(payload).ToCharArray(), 0 , 10);
        }
    }
}




