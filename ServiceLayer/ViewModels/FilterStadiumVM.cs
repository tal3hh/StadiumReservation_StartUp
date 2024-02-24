using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.ViewModels
{
    public class FilterStadiumVM
    {
        public string? City { get; set; }

        public decimal minPrice { get; set; }
        public decimal maxPrice { get; set; }

        public int page { get; set; }
        public int take { get; set; }
    }
}
