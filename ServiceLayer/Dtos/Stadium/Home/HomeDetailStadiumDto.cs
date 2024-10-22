﻿using ServiceLayer.Dtos.Reservation.Home;
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
        public decimal minPrice { get; set; }
        public decimal maxPrice { get; set; }
        public string? addres { get; set; }
        public string? phoneNumber { get; set; }

        public string? OpenCloseDay { get; set; }
        public string? OpenCloseHour { get; set; }

        public List<string>? emptyDates { get; set; }
        public List<string?>? descriptions { get; set; }
        public List<string?>? discounts { get; set; }

        public List<StadiumImageDto>? stadiumImages { get; set; }
    }
}
