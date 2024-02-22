using ServiceLayer.Dtos.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Interface
{
    public interface ITokenService
    {
        TokenResponseDto GenerateJwtToken(string username, List<string> roles);
    }
}
