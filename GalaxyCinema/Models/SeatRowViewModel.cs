using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalaxyCinema.Models
{
	public class SeatRowViewModel
	{
        public string RowName { get; set; }
        public List<SeatViewModel> Seats { get; set; }
    }
}