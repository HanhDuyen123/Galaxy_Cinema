using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GalaxyCinema.Models
{
	public class DayTypeParameterModel
	{
        public List<string> NgayThuong { get; set; }
        public List<string> HappyDay { get; set; }
        public List<string> NgayCuoiTuan { get; set; }
        public List<string> Holidays { get; set; }
    }
}