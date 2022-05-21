using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
namespace Project.Models
{
    public static class MD5_Extension
    {
        public static Encoding Encoding { get; set; } = Encoding.Unicode;
        public static string HashToString(string str) =>
            MD5
            .Create()
            .ComputeHash(Encoding.GetBytes(str))
            .Aggregate( new StringBuilder(),
                        (str, next) => str.Append(next.ToString("X2"))
                      )
            .ToString();



    }
}
