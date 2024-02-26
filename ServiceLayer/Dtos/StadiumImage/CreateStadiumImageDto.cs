using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Dtos.StadiumImage
{
    public class CreateStadiumImageDto
    {
        public string? Path { get; set; }
        public bool Main { get; set; }
    }
}
