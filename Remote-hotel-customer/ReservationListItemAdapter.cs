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
    public class ReservationListItemAdapter : BaseAdapter<ReservationViewModel>
    {
        List<ReservationViewModel> reservations;
        public ReservationListItemAdapter(List<ReservationViewModel> reservations)
        {
            this.reservations = reservations;
        }
        public override ReservationViewModel this[int position]
        {
            get
            {
                return reservations[position];
            }
        }

        public override int Count => reservations.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;

            if (view == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.reservation_item_view, parent, false);

                var roomNumber = view.FindViewById<TextView>(Resource.Id.reservationRoomNumber);
                var dateFrom = view.FindViewById<TextView>(Resource.Id.reservationDateFrom);
                var dateTo = view.FindViewById<TextView>(Resource.Id.reservationDateTo);

                view.Tag = new ViewHolder() { RoomNumber = roomNumber, DateFrom = dateFrom, DateTo = dateTo };
            }

            var holder = (ViewHolder)view.Tag;

            holder.RoomNumber.Text = reservations[position].RoomNumber;
            holder.DateFrom.Text = reservations[position].DateFrom.ToShortDateString();
            holder.DateTo.Text = reservations[position].DateTo.ToShortDateString();

            return view;

        }
    }
}
