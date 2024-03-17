using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Utlities
{
    public class Response<T> : Response, IResponse<T>
    {
        public Response(RespType RespType, string message) : base(RespType, message)
        {
        }

        public Response(RespType RespType, T data) : base(RespType)
        {
            Data = data;
        }

        public Response(RespType RespType, string message, T data) : base(RespType, message)
        {
            Data = data;
        }

        public T Data { get; set; }
    }
}
