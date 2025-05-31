using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalaxyCinema.Models
{
	public class SeatSelectionViewModel
	{
        public SHOWTIME Showtime { get; set; }
        public IEnumerable<SHOWTIME> OtherShowtimes { get; set; }
        public Dictionary<string, List<SEAT>> SeatsByRow { get; set; }
        public Dictionary<long, bool> SeatAvailability { get; set; }
        public List<long> SelectedSeatIds { get; set; } = new List<long>();
        public IEnumerable<TICKETTYPE> TicketTypes { get; set; }
        public int CurrentDayCatId { get; set; }
        public string CurrentDayCatName { get; set; }
    }
}