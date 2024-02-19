using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public class Area : BaseEntity
    {
        public string? Name { get; set; }
        public int widthSize { get; set; }
        public int lengthtSize { get; set; }

        public int StadiumId { get; set; }
        public Stadium? Stadium { get; set; }
        public List<Reservation>? Reservations { get; set; }
    }
}
