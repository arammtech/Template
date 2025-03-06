using Template.Domain.Identity;

namespace Template.Service.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateToken(ApplicationUser user);
        string ShortenToken(string token);
        string DecodeShortenToken(string shortToken);
    }
}
