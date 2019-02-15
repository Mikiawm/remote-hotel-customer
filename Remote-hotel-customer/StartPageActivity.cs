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
    [Activity(Label = "Login page", MainLauncher = true)]
    public class StartPageActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Load the UI created in Main.axml          
            SetContentView(Resource.Layout.start_page);

            // Get a reference to the button
            var loginButton = FindViewById<Button>(Resource.Id.loginButton);
            var enterPasswordButton = FindViewById<Button>(Resource.Id.enterPasswordButton);
      
            loginButton.Click += (sender, e) => {
                var dashboard = new Intent(this, typeof(DashboardActivity));
                dashboard.PutExtra("UserName", "Michal");
                dashboard.PutExtra("Password", "Mikios12");
                StartActivity(dashboard);
            };
   
            enterPasswordButton.Click += (sender, e) => {
                var beamActivity = new Intent(this, typeof(BeamActivity));
                StartActivity(beamActivity);
            };
        }
    };
}