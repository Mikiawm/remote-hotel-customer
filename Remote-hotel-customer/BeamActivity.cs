using System;
using System.Text;
using Android.App;
using Android.Nfc;
using Android.Content;
using Android.Provider;
using Android.Runtime;
using Android.OS;
using Android.Text.Format;
using Android.Views;
using Android.Widget;
using Android;
using System.Globalization;

namespace Remote_hotel_customer
{
	[Activity (Label = "Nfc-activity")]
	public class BeamActivity : Activity, NfcAdapter.ICreateNdefMessageCallback, NfcAdapter.IOnNdefPushCompleteCallback
	{
		public BeamActivity ()
		{
			mHandler = new MyHandler (HandlerHandleMessage);
		}
		
		NfcAdapter mNfcAdapter;
		TextView mInfoText;
        EditText passwordEditLetter1;
        EditText passwordEditLetter2;
        EditText passwordEditLetter3;
        EditText passwordEditLetter4;
        Button setPasswordButton;
        string passwordText = "";


        private const int MESSAGE_SENT = 1;
		
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			this.SetContentView (Resource.Layout.activity_main);

			mInfoText = FindViewById <TextView> (Resource.Id.addPasswordTextInfo);
            passwordEditLetter1 = FindViewById<EditText>(Resource.Id.passwordEditLetter1);
            passwordEditLetter2 = FindViewById<EditText>(Resource.Id.passwordEditLetter2);
            passwordEditLetter3 = FindViewById<EditText>(Resource.Id.passwordEditLetter3);
            passwordEditLetter4 = FindViewById<EditText>(Resource.Id.passwordEditLetter4);
            setPasswordButton = FindViewById<Button>(Resource.Id.setPasswordButton);
            passwordEditLetter1.Click += (sender, e) =>
            {
                passwordEditLetter2.Text = "";
                passwordEditLetter3.Text = "";
                passwordEditLetter4.Text = "";
                passwordEditLetter2.Enabled = false;
                passwordEditLetter3.Enabled = false;
                passwordEditLetter4.Enabled = false;
            };
            passwordEditLetter1.AfterTextChanged += (sender, e) =>
            {
                passwordEditLetter2.Enabled = true;
                passwordEditLetter2.RequestFocus();
            };
            passwordEditLetter2.AfterTextChanged += (sender, e) =>
            {
                passwordEditLetter3.Enabled = true;
                passwordEditLetter3.RequestFocus();
            };
            passwordEditLetter3.AfterTextChanged += (sender, e) =>
            {
                passwordEditLetter4.Enabled = true;
                passwordEditLetter4.RequestFocus();
            };
            passwordEditLetter4.AfterTextChanged += (sender, e) =>
            {
                setPasswordButton.RequestFocus();
            };
            setPasswordButton.Click += (sender, e) =>
            {
                passwordText = passwordEditLetter1.Text + passwordEditLetter2.Text + passwordEditLetter3.Text + passwordEditLetter4.Text;
                mInfoText.Text = passwordText;
            };

            // Check for available NFC Adapter
            mNfcAdapter = NfcAdapter.GetDefaultAdapter (this);

			if (mNfcAdapter == null) {
				mInfoText = FindViewById <TextView> (Resource.Id.addPasswordTextInfo);
				mInfoText.Text = "NFC is not available on this device.";
			} else {
				// Register callback to set NDEF message
				mNfcAdapter.SetNdefPushMessageCallback (this, this);
				// Register callback to listen for message-sent success
				mNfcAdapter.SetOnNdefPushCompleteCallback (this, this);
			}
		}
		
		public NdefMessage CreateNdefMessage (NfcEvent evt) 
		{
			DateTime time = DateTime.Now;
            var password = passwordText;
			NdefMessage msg = new NdefMessage (
			new NdefRecord[] { NdefRecord.CreateTextRecord(CultureInfo.CurrentCulture.Name, password) });
			return msg;
		}
		
		public void OnNdefPushComplete (NfcEvent arg0)
		{
			// A handler is needed to send messages to the activity when this
			// callback occurs, because it happens from a binder thread
			mHandler.ObtainMessage (MESSAGE_SENT).SendToTarget ();
		}
		
		class MyHandler : Handler
		{
			public MyHandler (Action<Message> handler)
			{
				this.handle_message = handler;
			}
			
			Action<Message> handle_message;
			public override void HandleMessage (Message msg)
			{
				handle_message (msg);
			}
		}
		
		private readonly Handler mHandler;
		
		protected void HandlerHandleMessage (Message msg) 
		{
			switch (msg.What) {
			case MESSAGE_SENT:
				Toast.MakeText (this.ApplicationContext, "Message sent!", ToastLength.Long).Show ();
				break;
			}
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			// Check to see that the Activity started due to an Android Beam
			if (NfcAdapter.ActionNdefDiscovered == Intent.Action) {
				ProcessIntent (Intent);
			}
		}
		
		protected override void OnNewIntent (Intent intent) 
		{
			// onResume gets called after this to handle the intent
			Intent = intent;
		}

		void ProcessIntent (Intent intent)
		{
			IParcelable [] rawMsgs = intent.GetParcelableArrayExtra (
				NfcAdapter.ExtraNdefMessages);
			// only one message sent during the beam
			NdefMessage msg = (NdefMessage) rawMsgs [0];
			// record 0 contains the MIME type, record 1 is the AAR, if present
			mInfoText.Text = Encoding.UTF8.GetString (msg.GetRecords () [0].GetPayload ());
		}

		public NdefRecord CreateMimeRecord (String mimeType, byte [] payload) 
		{
            NdefRecord textRecord = NdefRecord.CreateTextRecord("en", "Hello world");
            return textRecord;
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			// If NFC is not available, we won't be needing this menu
			if (mNfcAdapter == null) {
				return base.OnCreateOptionsMenu (menu);
			}
			MenuInflater inflater = MenuInflater;
			inflater.Inflate (Resource.Menu.Options, menu);
			return true;
		}
		
		public override bool OnOptionsItemSelected (IMenuItem item) 
		{
			switch (item.ItemId) {
			case Resource.Id.menu_settings:
				Intent intent = new Intent (Settings.ActionNfcsharingSettings);
				StartActivity (intent);
				return true;
			default:
				return base.OnOptionsItemSelected (item);
			}
		}
	}
}


