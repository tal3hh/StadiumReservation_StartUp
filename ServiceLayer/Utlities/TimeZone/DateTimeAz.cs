using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Utlities.TimeZone
{
    public class DateTimeAz
    {
        private static readonly TimeZoneInfo TimeZoneAz = TimeZoneInfo.FindSystemTimeZoneById("Azerbaijan Standard Time");

        public static DateTime Now
        {
            get { return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneAz); }
        }

        public static DateTime Today
        {
            get { return Now.Date; }
        }
    }
}
