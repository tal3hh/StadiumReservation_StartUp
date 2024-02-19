using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public class OpenStadium : BaseEntity
    {
        public string? OpenDay { get; set; }
        public string? CloseDay { get; set; }
        public string? OpenTime { get; set; }
        public string? CloseTime { get; set; }

        public int StadiumId { get; set; }
        public Stadium? Stadium { get; set; }
    }
}
