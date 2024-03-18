using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Dtos.StadiumDiscount
{
    public class DashStadiumDiscountDto
    {
        public int Id { get; set; }
        public string? Path { get; set; }
        public string? stadiumName { get; set; }
        public int stadiumId { get; set; }
    }
}
