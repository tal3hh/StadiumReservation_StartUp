using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.ViewModels
{
    public class TimeFilterStadiumVM
    {
        public DateTime Date { get; set; }

        public int startTime { get; set; }
        public int endTime { get; set; }

        public decimal minPrice { get; set; }
        public decimal maxPrice { get; set; }

        public int page { get; set; }
        public int take { get; set; }
    }
}
