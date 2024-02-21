using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Dtos.Stadium.Dash
{
    public class UpdateStadiumImageDto
    {
        public int Id { get; set; }
        public int stadiumId { get; set; }
        public string? Path { get; set; }
        public bool Main { get; set; }
    }
}
