using ServiceLayer.Dtos.Reservation.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Dtos.Stadium.Home
{
    public class HomeListStadiumDto
    {
        public int Id { get; set; }
        public string? name { get; set; }
        public string? path { get; set; }
        public decimal minPrice { get; set; }
        public decimal maxPrice { get; set; }
        public string? addres { get; set; }
        public string? phoneNumber { get; set; }
        public List<string>? emptyDates { get; set; }
    }
}
