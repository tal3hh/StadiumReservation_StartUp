using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Dtos.Stadium.Dash
{
    public class CreateStadiumImageDto
    {
        public int stadiumId { get; set; }
        public string? Path { get; set; }
        public bool Main { get; set; }
    }
}
