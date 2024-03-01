using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Dtos.StadiumImage
{
    public class UpdateStadiumImageDto
    {
        public int StadiumId { get; set; }
        public int Id { get; set; }
        public string? Path { get; set; }
        public bool Main { get; set; }
    }
}
