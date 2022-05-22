using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
namespace Project.Models
{
    public static class Hash_Extension
    {
        public static string StandartPassword { get; set; }
        public static Encoding Encoding { get; set; } = Encoding.Unicode;
        public static HashAlgorithm HashAlgorithm { get; set; } = SHA256.Create();
        public static string HashToString(string str) =>
            HashAlgorithm
            .ComputeHash(Encoding.GetBytes(str))
            .Aggregate( new StringBuilder(),
                        (str, next) => str.Append(next.ToString("X2"))
                      )
            .ToString();
        public static string GetPassword() => HashToString(StandartPassword);
    }
}
