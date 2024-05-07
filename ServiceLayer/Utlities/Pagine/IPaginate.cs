using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Utlities.Pagine
{
    public interface IPaginate<T> where T : class
    {
        Task<Paginate<T>> PaginateAsync(IQueryable<T> entities, int page, int take);
    }
}
