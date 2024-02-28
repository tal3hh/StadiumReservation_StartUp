using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Dtos.StadiumDiscount
{
    public class UpdateStadiumDiscountDto
    {
        public int Id { get; set; }
        public int StadiumId { get; set; }
        public string? Path { get; set; }
    }
}
