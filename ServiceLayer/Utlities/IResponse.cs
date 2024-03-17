using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Utlities
{
    public interface IResponse
    {
        string? Message { get; set; }
        RespType RespType { get; set; }
    }
}
