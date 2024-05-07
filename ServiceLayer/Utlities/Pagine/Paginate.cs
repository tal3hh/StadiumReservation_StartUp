using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Utlities.Pagine
{
    public class Paginate<T> : IPaginate<T> where T : class
    {
        public Paginate(List<T> datas, int currentPage, int totalPage)
        {
            Datas = datas;
            CurrentPage = currentPage;
            TotalPage = totalPage;
        }

        public async Task<Paginate<T>> PaginateAsync(IQueryable<T> entities, int page = 1, int take = 10)
        {
            var allCount = await entities.CountAsync();
            var Totalpage = (int)Math.Ceiling((decimal)allCount / take);

            var entitiesOnPage = await entities.Skip((page - 1) * take).Take(take).ToListAsync();

            return new Paginate<T>(entitiesOnPage, page, Totalpage);
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
