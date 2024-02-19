using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public class Stadium : BaseEntity
    {
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public int View { get; set; }


        
        public List<Area>? Areas { get; set; }
        //Hefte ici basqa saatlarda, Hefte sonu ferqli saat ola biler.
        public List<OpenStadium>? OpenStadiums { get; set; }
    }
}
