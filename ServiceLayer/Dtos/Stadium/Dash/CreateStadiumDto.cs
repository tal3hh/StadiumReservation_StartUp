using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Dtos.Stadium.Dash
{
    public class CreateStadiumDto
    {
        public string? Name { get; set; }
        public decimal minPrice { get; set; }
        public decimal maxPrice { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }

    }
}
