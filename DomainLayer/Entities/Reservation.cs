using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public class Reservation : BaseEntity
    {
        public string? ByName { get; set; }
        public decimal Price { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime Date { get; set; }
        public int AreaId { get; set; }
        public Area? Area { get; set; }
    }
}
