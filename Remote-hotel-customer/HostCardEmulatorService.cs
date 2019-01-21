using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Nfc;
using Android.Nfc.CardEmulators;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Remote_hotel_customer
{
    public class CardEmulatorService : HostApduService
    {
        private static String TAG = "JDR HostApduService";

        //
        // We use the default AID from the HCE Android documentation
        // https://developer.android.com/guide/topics/connectivity/nfc/hce.html
        //
        // Ala... <aid-filter android:name="F0394148148100" />
        //
        private static byte[] APDU_SELECT = {
        (byte)0x00, // CLA	- Class - Class of instruction
        (byte)0xA4, // INS	- Instruction - Instruction code
        (byte)0x04, // P1	- Parameter 1 - Instruction parameter 1
        (byte)0x00, // P2	- Parameter 2 - Instruction parameter 2
        (byte)0x07, // Lc field	- Number of bytes present in the data field of the command
        (byte)0xF0, (byte)0x39, (byte)0x41, (byte)0x48, (byte)0x14, (byte)0x81, (byte)0x00, // NDEF Tag Application name
        (byte)0x00  // Le field	- Maximum number of bytes expected in the data field of the response to the command
    };

        private static byte[] CAPABILITY_CONTAINER = {
            (byte)0x00, // CLA	- Class - Class of instruction
            (byte)0xa4, // INS	- Instruction - Instruction code
            (byte)0x00, // P1	- Parameter 1 - Instruction parameter 1
            (byte)0x0c, // P2	- Parameter 2 - Instruction parameter 2
            (byte)0x02, // Lc field	- Number of bytes present in the data field of the command
            (byte)0xe1, (byte)0x03 // file identifier of the CC file
    };

        private static byte[] READ_CAPABILITY_CONTAINER = {
            (byte)0x00, // CLA	- Class - Class of instruction
            (byte)0xb0, // INS	- Instruction - Instruction code
            (byte)0x00, // P1	- Parameter 1 - Instruction parameter 1
            (byte)0x00, // P2	- Parameter 2 - Instruction parameter 2
            (byte)0x0f  // Lc field	- Number of bytes present in the data field of the command
    };

        // In the scenario that we have done a CC read, the same byte[] match
        // for ReadBinary would trigger and we don't want that in succession
        private Boolean READ_CAPABILITY_CONTAINER_CHECK = false;

        private static byte[] READ_CAPABILITY_CONTAINER_RESPONSE = {
            (byte)0x00, (byte)0x0F, // CCLEN length of the CC file
            (byte)0x20, // Mapping Version 2.0
            (byte)0x00, (byte)0x3B, // MLe maximum 59 bytes R-APDU data size
            (byte)0x00, (byte)0x34, // MLc maximum 52 bytes C-APDU data size
            (byte)0x04, // T field of the NDEF File Control TLV
            (byte)0x06, // L field of the NDEF File Control TLV
            (byte)0xE1, (byte)0x04, // File Identifier of NDEF file
            (byte)0x00, (byte)0x32, // Maximum NDEF file size of 50 bytes
            (byte)0x00, // Read access without any security
            (byte)0x00, // Write access without any security
            (byte)0x90, (byte)0x00 // A_OKAY
    };

        private static byte[] NDEF_SELECT = {
            (byte)0x00, // CLA	- Class - Class of instruction
            (byte)0xa4, // Instruction byte (INS) for Select command
            (byte)0x00, // Parameter byte (P1), select by identifier
            (byte)0x0c, // Parameter byte (P1), select by identifier
            (byte)0x02, // Lc field	- Number of bytes present in the data field of the command
            (byte)0xE1, (byte)0x04 // file identifier of the NDEF file retrieved from the CC file
    };

        private static byte[] NDEF_READ_BINARY_NLEN = {
            (byte)0x00, // Class byte (CLA)
            (byte)0xb0, // Instruction byte (INS) for ReadBinary command
            (byte)0x00, (byte)0x00, // Parameter byte (P1, P2), offset inside the CC file
            (byte)0x02  // Le field
    };

        private static byte[] NDEF_READ_BINARY_GET_NDEF = {
            (byte)0x00, // Class byte (CLA)
            (byte)0xb0, // Instruction byte (INS) for ReadBinary command
            (byte)0x00, (byte)0x00, // Parameter byte (P1, P2), offset inside the CC file
            (byte)0x0f  //  Le field
    };

        private static byte[] A_OKAY = {
            (byte)0x90,  // SW1	Status byte 1 - Command processing status
            (byte)0x00   // SW2	Status byte 2 - Command processing qualifier
    };

        private static byte[] NDEF_ID = {
            (byte)0xE1,
            (byte)0x04
    };

        private static NdefRecord NDEF_URI = new NdefRecord(
                NdefRecord.TnfWellKnown,
                NdefRecord.RtdText.ToArray(),
                NDEF_ID,
                Encoding.UTF8.GetBytes("Hello world!")
        );
        private static byte[] NDEF_URI_BYTES = NDEF_URI.ToByteArray();
        private byte[] NDEF_URI_LEN = Encoding.ASCII.GetBytes(NDEF_URI_BYTES.Length.ToString("X"));

        public override void OnDeactivated([GeneratedEnum] DeactivationReason reason)
        {
            throw new NotImplementedException();
        }

        public int onStartCommand(Intent intent, int flags, int startId)
        {

            if (intent.HasExtra("ndefMessage"))
            {
                NDEF_URI = new NdefRecord(
                        NdefRecord.TnfWellKnown,
                        NdefRecord.RtdText.ToArray(),
                        NDEF_ID,
                        Encoding.ASCII.GetBytes(intent.GetStringExtra("ndefMessage"))
                );

                NDEF_URI_BYTES = NDEF_URI.ToByteArray();
                byte[] NDEF_URI_LEN = Encoding.ASCII.GetBytes(NDEF_URI_BYTES.Length.ToString("X"));

                Context context = Application.Context;
                string text = "Your NDEF text has been set!";
                Toast toast = Toast.MakeText(context, text, ToastLength.Short);
                toast.SetGravity(GravityFlags.Center, 0, 0);
                toast.Show();
            }

            Log.Info("blabla", "onStartCommand() | NDEF" + NDEF_URI.ToString());

            return 0;
        }

        public override byte[] ProcessCommandApdu(byte[] commandApdu, Bundle extras)
        {

            //
            // The following flow is based on Appendix E "Example of Mapping Version 2.0 Command Flow"
            // in the NFC Forum specification
            //
            Log.Info(TAG, "processCommandApdu() | incoming commandApdu: " + Utils.BytesToHex(commandApdu));

            //
            // First command: NDEF Tag Application select (Section 5.5.2 in NFC Forum spec)
            //
            if (Utils.IsEqual(APDU_SELECT, commandApdu))
            {
                Log.Info(TAG, "APDU_SELECT triggered. Our Response: " + Utils.BytesToHex(A_OKAY));
                return A_OKAY;
            }

            //
            // Second command: Capability Container select (Section 5.5.3 in NFC Forum spec)
            //
            if (Utils.IsEqual(CAPABILITY_CONTAINER, commandApdu))
            {
                Log.Info(TAG, "CAPABILITY_CONTAINER triggered. Our Response: " + Utils.BytesToHex(A_OKAY));
                return A_OKAY;
            }

            //
            // Third command: ReadBinary data from CC file (Section 5.5.4 in NFC Forum spec)
            //
            if (Utils.IsEqual(READ_CAPABILITY_CONTAINER, commandApdu) && !READ_CAPABILITY_CONTAINER_CHECK)
            {
                Log.Info(TAG, "READ_CAPABILITY_CONTAINER triggered. Our Response: " + Utils.BytesToHex(READ_CAPABILITY_CONTAINER_RESPONSE));
                READ_CAPABILITY_CONTAINER_CHECK = true;
                return READ_CAPABILITY_CONTAINER_RESPONSE;
            }

            //
            // Fourth command: NDEF Select command (Section 5.5.5 in NFC Forum spec)
            //
            if (Utils.IsEqual(NDEF_SELECT, commandApdu))
            {
                Log.Info(TAG, "NDEF_SELECT triggered. Our Response: " + Utils.BytesToHex(A_OKAY));
                return A_OKAY;
            }

            //
            // Fifth command:  ReadBinary, read NLEN field
            //
            if (Utils.IsEqual(NDEF_READ_BINARY_NLEN, commandApdu))
            {

                byte[] start = {
                    (byte)0x00
            };

                // Build our response
                byte[] response = new byte[start.Length + NDEF_URI_LEN.Length + A_OKAY.Length];

                Array.Copy(start, 0, response, 0, start.Length);
                Array.Copy(NDEF_URI_LEN, 0, response, start.Length, NDEF_URI_LEN.Length);
                Array.Copy(A_OKAY, 0, response, start.Length + NDEF_URI_LEN.Length, A_OKAY.Length);

                Log.Info(TAG, response.ToString());
                Log.Info(TAG, "NDEF_READ_BINARY_NLEN triggered. Our Response: " + Utils.BytesToHex(response));

                return response;
            }

            //
            // Sixth command: ReadBinary, get NDEF data
            //
            if (Utils.IsEqual(NDEF_READ_BINARY_GET_NDEF, commandApdu))
            {
                Log.Info(TAG, "processCommandApdu() | NDEF_READ_BINARY_GET_NDEF triggered");

                byte[] start = {
                    (byte)0x00
            };

                // Build our response
                byte[] response = new byte[start.Length + NDEF_URI_LEN.Length + NDEF_URI_BYTES.Length + A_OKAY.Length];

                Array.Copy(start, 0, response, 0, start.Length);
                Array.Copy(NDEF_URI_LEN, 0, response, start.Length, NDEF_URI_LEN.Length);
                Array.Copy(NDEF_URI_BYTES, 0, response, start.Length + NDEF_URI_LEN.Length, NDEF_URI_BYTES.Length);
                Array.Copy(A_OKAY, 0, response, start.Length + NDEF_URI_LEN.Length + NDEF_URI_BYTES.Length, A_OKAY.Length);

                Log.Info(TAG, NDEF_URI.ToString());
                Log.Info(TAG, "NDEF_READ_BINARY_GET_NDEF triggered. Our Response: " + Utils.BytesToHex(response));

                Context context = Application.Context;
                string text = "NDEF text has been sent to the reader!";

                Toast toast = Toast.MakeText(context, text, ToastLength.Short);
                toast.SetGravity(GravityFlags.Center, 0, 0);
                toast.Show();

                READ_CAPABILITY_CONTAINER_CHECK = false;
                return response;
            }

            //
            // We're doing something outside our scope
            //
            Log.Wtf(TAG, "processCommandApdu() | I don't know what's going on!!!.");
            return Encoding.ASCII.GetBytes("Can I help you?");
        }

        public void onDeactivated(int reason)
        {
            Log.Info(TAG, "onDeactivated() Fired! Reason: " + reason);
        }

    }
}