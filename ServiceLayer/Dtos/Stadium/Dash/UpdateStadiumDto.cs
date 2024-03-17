using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Dtos.Stadium.Dash
{
    public class UpdateStadiumDto
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? City { get; set; }
        public decimal minPrice { get; set; }
        public decimal maxPrice { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }

        public int openHour { get; set; }
        public int closeHour { get; set; }
        public int nightHour { get; set; }

        public string? OpenCloseDay { get; set; }
        public string? OpenCloseHour { get; set; }
    }
}
