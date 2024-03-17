using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Utlities
{
    internal interface IResponse<T> : IResponse
    {
        T Data { get; set; }
    }
}
