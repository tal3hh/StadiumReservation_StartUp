using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public class StadiumDiscount : BaseEntity
    {
        public string? Path { get; set; }

        public int StadiumId { get; set; }
        public Stadium? Stadium { get; set; }
    }
}
