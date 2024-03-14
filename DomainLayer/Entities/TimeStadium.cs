using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public class TimeStadium : BaseEntity
    {
        public int openTime { get; set; }
        public int closeTime { get; set; }

        public int nightTime { get; set; }

        public int StadiumId { get; set; }
        public Stadium? Stadium { get; set; }
    }
}
