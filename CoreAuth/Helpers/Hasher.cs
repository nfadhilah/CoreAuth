using System;
using System.Security.Cryptography;
using System.Text;

namespace CoreAuth.Helpers
{
  public static class Hasher
  {
    public static string Create(string input)
    {
      byte[] inputSalt, inputHash;

      using (var hmac = new HMACSHA256())
      {
        inputSalt = hmac.Key;
        inputHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
      }

      return $"{Convert.ToBase64String(inputSalt)}:{Convert.ToBase64String(inputHash)}";
    }

    public static bool Validate(string hashedInput, string input)
    {
      var parts = hashedInput.Split(':');

      var inputSalt = Convert.FromBase64String(parts[0]);

      var inputHash = Convert.FromBase64String(parts[1]);

      using (var hmac = new HMACSHA256(inputSalt))
      {
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));

        for (var i = 0; i < computedHash.Length; i++)
        {
          if (computedHash[i] != inputHash[i])
            return false;
        }

        return true;
      }
    }
  }
}
