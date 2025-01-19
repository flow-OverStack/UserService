using System.Security.Cryptography;
using System.Text;

namespace UserService.Tests.Extensions;

public static class UserExtensions
{
    public static string HashPassword(this string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}