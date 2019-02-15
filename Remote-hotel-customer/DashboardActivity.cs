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
    [Activity(Label = "Dashboard")]
    public class DashboardActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Load the UI defined in Second.axml
            SetContentView(Resource.Layout.dashboard);

            // Get a reference to the button
            var createReservationButton = FindViewById<Button>(Resource.Id.goTocreateReservationButton);
            var enterPasswordButton = FindViewById<Button>(Resource.Id.goToEnterPasswordActivityButton);
            var reservationsList = FindViewById<ListView>(Resource.Id.reservationList);
            //var reservations =  ["223", "123"];
            reservationsList.Adapter = new ReservationListItemAdapter(ReservationData.ReservationViewModels);

            //reservationsList.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
            //{
            //    //Get our item from the list adapter
            //    reservationsList.
            //    var item = GetItemAtPosition(e.Position);

            //    //Make a toast with the item name just to show it was clicked
            //    Toast.MakeText(this, item.Name + " Clicked!", ToastLength.Short).Show();
            //};

            createReservationButton.Click += (sender, e) =>
            {
                var createReservationActivity = new Intent(this, typeof(CreateReservationActivity));
                StartActivity(createReservationActivity);
            };

            enterPasswordButton.Click += (sender, e) =>
            {
                var beamActivity = new Intent(this, typeof(BeamActivity));
                StartActivity(beamActivity);
            };
        }
    }
}