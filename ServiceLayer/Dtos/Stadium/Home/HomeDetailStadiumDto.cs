using ServiceLayer.Dtos.Reservation.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Dtos.Stadium.Home
{
    public class HomeDetailStadiumDto
    {
        public string? name { get; set; }
        public decimal price { get; set; }
        public string? addres { get; set; }
        public string? phoneNumber { get; set; }
        public string? description { get; set; }
        public int view { get; set; }

        public string? OpenDay { get; set; }
        public string? CloseDay { get; set; }
        public DateTime OpenTime { get; set; }
        public DateTime CloseTime { get; set; }

        public List<string>? emptyDates { get; set; }

        public List<StadiumImageDto> stadiumImages { get; set; }
    }
}
