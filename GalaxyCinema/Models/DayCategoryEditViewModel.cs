using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalaxyCinema.Models
{
	public class DayCategoryEditViewModel
	{
        public int CurrentDayCatId { get; set; }
        public string CurrentDayCatName { get; set; }

        public List<int> RegularDays { get; set; } = new List<int>();
        public List<int> HappyDays { get; set; } = new List<int>();
        public List<int> WeekendDays { get; set; } = new List<int>();

        // Holidays dưới dạng "MM-dd"
        public List<string> Holidays { get; set; } = new List<string>();
    }
}