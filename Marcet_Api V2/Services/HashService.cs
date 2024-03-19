using Marcet_Api.Services.IServices;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Text;

public class HashService : IHashService
{
    public KeyValuePair<string, string> GetHash(string request)
    {
        string salt = GenerateSalt();
        int iterationCount = 5000;
        int keyLength = 256;

        byte[] hash = GeneratePBKDF2Hash(request, salt, iterationCount, keyLength);
        string base64Hash = Convert.ToBase64String(hash);
        return new KeyValuePair<string, string>(salt, base64Hash);
    }

    public KeyValuePair<string, string> GetHash(string request, string salt)
    {
        int iterationCount = 5000;
        int keyLength = 256;

        byte[] hash = GeneratePBKDF2Hash(request, salt, iterationCount, keyLength);
        string base64Hash = Convert.ToBase64String(hash);
        return new KeyValuePair<string, string>(salt, base64Hash);
    }

    public string GenerateSalt()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        SecureRandom random = new SecureRandom();
        return new string(Enumerable.Repeat(chars, 16)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public string HashPassword(string password, string salt)
    {
        return BCrypt.Net.BCrypt.HashPassword(password + salt);
    }

    public bool VerifyPassword(string password, string hashedPassword, string salt)
    {
        return BCrypt.Net.BCrypt.Verify(password + salt, hashedPassword);
    }

    private byte[] GeneratePBKDF2Hash(string request, string salt, int iterationCount, int keyLength)
    {
        char[] passwordChars = request.ToCharArray();
        Pkcs5S2ParametersGenerator generator = new Pkcs5S2ParametersGenerator(new Sha256Digest());
        generator.Init(PbeParametersGenerator.Pkcs5PasswordToUtf8Bytes(passwordChars), Encoding.UTF8.GetBytes(salt), iterationCount);
        KeyParameter key = (KeyParameter)generator.GenerateDerivedParameters("AES", keyLength);

        return key.GetKey();
    }
}

