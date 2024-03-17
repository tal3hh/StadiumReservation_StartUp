using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public class Stadium : BaseEntity
    {
        public string? Name { get; set; }
        public decimal minPrice { get; set; }
        public decimal maxPrice { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }

        public int openHour { get; set; }
        public int closeHour { get; set; }
        public int nightHour { get; set; }

        public string? OpenCloseDay { get; set; }
        public string? OpenCloseHour { get; set; }


        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public List<Area>? Areas { get; set; }
        public List<StadiumImage>? StadiumImages { get; set; }
        public List<StadiumDetail>? StadiumDetails { get; set; }
        public List<StadiumDiscount>? StadiumDiscounts { get; set; }
    }
}
