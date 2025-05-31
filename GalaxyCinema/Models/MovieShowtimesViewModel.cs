using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalaxyCinema.Models
{
	public class MovieShowtimesViewModel
	{
        public MOVIE Movie { get; set; }
        public List<SHOWTIME> Showtimes { get; set; }
        public string Genres { get; set; }
        public int ShowtimesCount { get; set; }
    }
}