using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.Models
{
    public class RolesReadonly
    {
        public const string Owner = "Owner";
        public const string Personell_Officer = "Personell Officer";
        public const string Storekeeper = "Storekeeper";
        public const string Repairman = "Repairman";
        public const string DB_Admin = "DB Admin";
        public const string Captain = "Captain";
    }

    public static class Claim_Extensions
    {
        public static string Role(this ClaimsPrincipal principal) => principal.FindFirst(p => p.Type == ClaimsIdentity.DefaultRoleClaimType)?.Value ?? "Default";

        public static int PersonId(this ClaimsPrincipal principal) => int.Parse(principal.FindFirst(p => p.Type == "PersonId")?.Value ?? "0");
        public static int AccountId(this ClaimsPrincipal principal) => int.Parse(principal.FindFirst(p => p.Type == "AccountId")?.Value ?? "0");

    }
}
