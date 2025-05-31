using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalaxyCinema.Models
{
	public class SeatViewModel
	{
        public long SeatId { get; set; }
        public string SeatName { get; set; }
        public bool IsAvailable { get; set; }
        public string SeatType { get; set; } // Regular, VIP, Couple, etc.
        public decimal Price { get; set; }
    }
}