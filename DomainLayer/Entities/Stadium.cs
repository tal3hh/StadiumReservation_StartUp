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
        public string? City { get; set; }
        public decimal minPrice { get; set; }
        public decimal maxPrice { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public int View { get; set; }

        public string? OpenDay { get; set; }
        public string? CloseDay { get; set; }
        public DateTime OpenTime { get; set; }
        public DateTime CloseTime { get; set; }


        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public List<StadiumImage>? StadiumImages { get; set; }
        public List<Area>? Areas { get; set; }
    }
}
