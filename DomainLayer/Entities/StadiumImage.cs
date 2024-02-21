using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public class StadiumImage : BaseEntity
    {
        public string? Path { get; set; }
        public bool Main { get; set; }

        public int StadiumId { get; set; }
        public Stadium? Stadium { get; set; }
    }
}
