using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalaxyCinema.Models
{
	public class SeatBookingRequest
	{
        public long SeatId { get; set; }
        public int TicketTypeId { get; set; }
    }
}