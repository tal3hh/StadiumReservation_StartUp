using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public class StadiumDetail : BaseEntity
    {
        public int StadiumId { get; set; }
        public string? Description { get; set; }

        public Stadium? Stadium { get; set; }
    }
}
