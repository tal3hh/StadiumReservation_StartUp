using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Dtos.Account
{
    public class TokenResponseDto
    {
        public TokenResponseDto(string token, DateTime expiredate)
        {
            Token = token;
            ExpireDate = expiredate;
        }

        public string? Token { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
