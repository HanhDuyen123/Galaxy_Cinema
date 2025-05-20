using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalaxyCinema.Models
{
	public class Revenue
	{
        public DateTime? Date { get; set; }
        public string MovieName { get; set; }
        public int Tickets { get; set; }
        public decimal Amount { get; set; }
    }
}