using ServiceLayer.Dtos.Reservation;
using ServiceLayer.Dtos.Reservation.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Dtos.Stadium.Home
{
    public class HomeStadiumDto
    {
        public string? name { get; set; }
        public decimal price { get; set; }
        public string? addres { get; set; }
        public string? phoneNumber { get; set; }
        public List<HomeReservationDto>? Dates { get; set; }
    }
}
