using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Common.Result
{
    public interface IResponse
    {
        string? Message { get; set; }
        RespType RespType { get; set; }
    }
}
