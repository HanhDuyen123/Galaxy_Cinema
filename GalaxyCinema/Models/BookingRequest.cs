using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalaxyCinema.Models
{
    public class BookingRequest
    {
        public long ShowtimeId { get; set; }
        public int DayCatId { get; set; }
        public long? CustomerId { get; set; }
        public long StaffId { get; set; }
        public List<SeatBookingRequest> SelectedSeats { get; set; }
    }
}