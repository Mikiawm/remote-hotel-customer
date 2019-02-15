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
    public static class ReservationData
    {
        public static List<ReservationViewModel> ReservationViewModels { get; private set; }

        static ReservationData()
        {
            var temp = new List<ReservationViewModel>();

            AddReservation(temp);

            ReservationViewModels = temp.OrderBy(i => i.RoomNumber).ToList();
        }

        private static void AddReservation(List<ReservationViewModel> temp)
        {
            temp.Add(new ReservationViewModel()
            {
                RoomNumber = "223",
                DateFrom = DateTime.Now.AddDays(3),
                DateTo = DateTime.Now.AddDays(5)
            });
            temp.Add(new ReservationViewModel()
            {
                RoomNumber = "123",
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddDays(5)
            });
            temp.Add(new ReservationViewModel()
            {
                RoomNumber = "224",
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddDays(3)
            });
        }
    }
}