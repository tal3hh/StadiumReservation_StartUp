using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.ViewModels
{
    public class SearchStadiumVM
    {
        public string? search { get; set; }
        public int page { get; set; }
        public int take { get; set; }
    }
}
