using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace CC.Domain.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Generates a Random Password
        /// respecting the given strength requirements.
        /// </summary>
        /// <param name="opts">A valid PasswordOptions object
        /// containing the password strength requirements.</param>
        /// <returns>A random password</returns>
        public static string GenerateRandomPassword(PasswordOptions opts = null)
        {
            // Define los caracteres válidos para la contraseña
            const string validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_-+=[{]};:>|./?";

            // Usa un generador de números aleatorios criptográficamente seguro
            byte[] randomData = new byte[8];

            // Genera los bytes aleatorios usando RNGCryptoServiceProvider
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomData);
            }

            // Convierte los bytes aleatorios en caracteres de la contraseña
            char[] passwordChars = new char[8];

            // Itera sobre los bytes aleatorios y selecciona caracteres del conjunto válido
            for (int i = 0; i < 8; i++)
            {
                passwordChars[i] = validCharacters[randomData[i] % validCharacters.Length];
            }

            // Retorna la contraseña generada como una cadena
            return new string(passwordChars);
        }

        public const string USER_NOT_FOUND = "User Not Found";
        public const string USER_PASWORD_INCORRECT = "User pasword incorect";

        public static string GetSHA256(this string str)
        {
            SHA256 sha256 = SHA256.Create();
            ASCIIEncoding encoding = new();
            StringBuilder sb = new();
            byte[] stream = sha256.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
    }
}