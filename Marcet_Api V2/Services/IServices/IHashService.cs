
namespace Marcet_Api.Services.IServices
{
    public interface IHashService
    {
        KeyValuePair<string, string> GetHash(string request);
        KeyValuePair<string, string> GetHash(string request, string salt);
        string GenerateSalt();
        string HashPassword(string password, string salt);
        bool VerifyPassword(string password, string hashedPassword, string salt);
    }

}
