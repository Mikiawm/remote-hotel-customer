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

namespace Remote_hotel_customer
{
    class Utils
    {
        protected static char[] hexArray = "0123456789ABCDEF".ToCharArray();
        //public static byte[] HexStringToByteArray(string data)
        //{

        //}

        // Simple way to output byte[] to hex (my readable preference)
        // This version quite speedy; originally from: http://stackoverflow.com/a/9855338

        public static string BytesToHex(byte[] byteArray)
        {
            char[] hexChars = new char[byteArray.Length * 2];
            for(int j = 0; j < byteArray.Length; j++)
            {
                int v = byteArray[j] & 0xFF;
                hexChars[j * 2] = hexArray[v >> 4];
                hexChars[j * 2 + 1] = hexArray[v & 0x0F];
            }
            return new string(hexChars);
        }
        public static Boolean IsEqual(byte[] a, byte[] b)
        {
            if(a.Length != b.Length)
            {
                return false;
            }

            int result = 0;
            for(int i = 0; i< a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }
            return result == 0;
        }
    }
}