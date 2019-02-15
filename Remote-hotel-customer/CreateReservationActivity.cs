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
    [Activity(Label = "Create reservation")]
    public class CreateReservationActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Load the UI defined in Second.axml
            SetContentView(Resource.Layout.create_reservation_view);

            // Get a reference to the button
            var createReservationButton = FindViewById<Button>(Resource.Id.createReservationButton);
            var backButton = FindViewById<Button>(Resource.Id.backButton);

            createReservationButton.Click += (sender, e) => {
                var createReservationActivity = new Intent(this, typeof(CreateReservationActivity));
                StartActivity(createReservationActivity);
            };

            backButton.Click += (sender, e) => {
                var dashboard = new Intent(this, typeof(DashboardActivity));
                StartActivity(dashboard);
            };
        }
    }
}