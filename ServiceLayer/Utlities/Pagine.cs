using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Utlities
{
    public class Paginate<T>
    {
        public Paginate(List<T> datas, int currentPage, int totalPage)
        {
            Datas = datas;
            CurrentPage = currentPage;
            TotalPage = totalPage;
        }

        public List<T> Datas { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }

        public bool HasPrevious
        {
            get
            {
                return CurrentPage > 1;
            }
        }


        public bool HasNext
        {
            get
            {
                return CurrentPage < TotalPage;
            }
        }
    }
}
