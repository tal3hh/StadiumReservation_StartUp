using ServiceLayer.Dtos.Account;

namespace ServiceLayer.Services.Interface
{
    public interface ITokenService
    {
        TokenResponseDto GenerateJwtToken(string username, List<string> roles);
    }
}
