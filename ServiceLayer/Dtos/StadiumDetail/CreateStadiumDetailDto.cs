using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Dtos.StadiumDetail
{
    public class CreateStadiumDetailDto
    {
        public int StadiumId { get; set; }
        public string? description { get; set; }
    }
}
