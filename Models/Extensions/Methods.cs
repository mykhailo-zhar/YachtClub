using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Project.Migrations;

namespace Project.Models
{
    public class MethodCRUD
    {
        public string ClassName { get; set; }
        public string Create => CRUD.Create + ClassName;
        public string Edit => CRUD.Edit + ClassName;
        public string Delete => CRUD.Delete + ClassName;
    }



    public class Methods
    {
        public static string MoneyCulture { get; set; } = "₴";

        public static string Money(decimal money) => $"{money,0:f2} {MoneyCulture}";


        public static readonly string Limitier= "%%&-//";
        public static bool IsDev { get; set; } = true;
        public static bool IsStr(string str) =>
               !(string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str));

        public static string Message(Material material)
        {
            string MaterialMetric = material.Metric;
            string MaterialTypeMetric = material.Type.Metric;
            string metric = IsStr(MaterialMetric) ? MaterialMetric : IsStr(MaterialTypeMetric) ? MaterialTypeMetric : "";
            return IsStr(metric) ? $"в {metric}" : "";
        }

        public static string Metric(Material material)
        {
            string MaterialMetric = material.Metric;
            string MaterialTypeMetric = material.Type.Metric;
            string metric = IsStr(MaterialMetric) ? MaterialMetric : IsStr(MaterialTypeMetric) ? MaterialTypeMetric : "";
            return IsStr(metric) ? $"{metric}" : "";
        }

        public static string PersonEmail(Person person) => $"{person.Name} {person.Surname} {person.Email}";
        public static string PersonPhone(Person person) => $"{person.Name} {person.Surname} {person.Phone}";
        public static string PersonNameSurname(Person person) => $"{person.Name} {person.Surname}";
        
        public static string YachtNameType(Yacht yacht) => $"{yacht.Name} ( {yacht.Type.Name} )";



        public static string YesNo(bool Bool) => Bool ? "Да" : "Нет";

        public static string CoalesceString(string str) => IsStr(str) ? str : string.Empty;
        public static string CoalesceDateTime(DateTime? dateTime) => dateTime == null ? "[Дата окончания отсутствует]" : dateTime.Value.ToString();

        public static string RanGuid => Guid.NewGuid().ToString("N");

        public static int ExtradationStatusPrio(string status)
        {
            switch (status)
            {
                case "Created": return 100;
                case "Waits": return 50;
                case "Done": return 5;
                case "Canceled": return 0;
                default:
                    return -1;
            }
        }
         public static int RepairStatusPrio(string status)
        {
            switch (status)
            {
                case "Created": return 100;
                case "Waits": return 50;
                case "In Progress": return 25;
                case "Done": return 5;
                case "Canceled": return 0;
                default:
                    return -1;
            }
        }

    }
}
