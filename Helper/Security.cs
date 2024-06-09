using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace BookingPhongHoc.Helper
{
    public class Security
    {
       
        

        public static string GenerateCode(int length)
        {
            var random = new Random();

            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

      

        public static bool VerifyPassword(string enteredPassword, byte[] salt, string hashPassword)
        {
            string encryptedPassw = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: enteredPassword,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

            return encryptedPassw == hashPassword;
        }
    }
}

