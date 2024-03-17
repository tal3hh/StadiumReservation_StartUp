using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Utlities
{
    public class Response : IResponse
    {
        public Response(RespType responseType)
        {
            RespType = responseType;
        }

        public Response(RespType responseType, string message)
        {
            RespType = responseType;
            Message = message;
        }

        public string? Message { get; set; }
        public RespType RespType { get; set; }
    }


    public enum RespType
    {
        Success,
        NotFound,
        BadReqest
    }
}
