using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project.Models
{
    public class RolesReadonly
    {
        public const string Owner = "Owner";
        public const string Guest = "guest";
        public const string Personell_Officer = "Personell Officer";
        public const string Storekeeper = "Storekeeper";
        public const string Repairman = "Repairman";
        public const string DB_Admin = "DB Admin";
        public const string Captain = "Captain";
        public const string Client = "Client";
    }

    public static class RegexExtension
    {
        public static string ReplaceAll(string str, Regex regex, string res)
        {
            foreach (var item in regex.Matches(str).Distinct().ToList())
            {
               str = str.Replace(item.Value, res);
            }
            return str;
        }
    }

    public static class Claim_Extensions
    {
        public static string Role(this ClaimsPrincipal principal) => principal.FindFirst(p => p.Type == ClaimsIdentity.DefaultRoleClaimType)?.Value ?? "Default";
        public static bool IsInRolesOr(this ClaimsPrincipal principal, params string[] Roles) => Roles.Aggregate(false, (start, th) => start || principal.IsInRole(th));
        public static bool IsInRolesAnd(this ClaimsPrincipal principal, params string[] Roles) => Roles.Aggregate(false, (start, th) => start && principal.IsInRole(th));
        

        public static int PersonId(this ClaimsPrincipal principal) => int.Parse(principal.FindFirst(p => p.Type == "PersonId")?.Value ?? "0");
        public static string Password(this ClaimsPrincipal principal) => principal.FindFirst(p => p.Type == "Password")?.Value ?? "";


    }
}
