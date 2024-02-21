using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Dtos.Reservation.Dash
{
    public class OwnerReservDto
    {
        public string? byName { get; set; }
        public decimal Price { get; set; }
        public string? phoneNumber { get; set; }
        public string? areaName { get; set; }
        public string? date { get; set; }
        public string? hour { get; set; }
    }
}
