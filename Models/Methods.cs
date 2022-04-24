using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Project.Migrations;

namespace Project.Models
{
    public class Methods
    {
        public static bool IsStr(string str) =>
               !(string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str));

        public static string Message(Material material)
        {
            string MaterialMetric = material.Metric;
            string MaterialTypeMetric = material.Type.Metric;
            string metric = IsStr(MaterialMetric) ? MaterialMetric : IsStr(MaterialTypeMetric) ? MaterialTypeMetric : "";
            return IsStr(metric) ? $"в {metric}" : "";
        }

        public static string Metric (Material material)
        {
            string MaterialMetric = material.Metric;
            string MaterialTypeMetric = material.Type.Metric;
            string metric = IsStr(MaterialMetric) ? MaterialMetric : IsStr(MaterialTypeMetric) ? MaterialTypeMetric : "";
            return IsStr(metric) ? $"{metric}" : "";
        }

        public static string YesNo(bool Bool) => Bool ? "Да" : "Нет";

        public static string CoalesceDateTime(DateTime? dateTime) => dateTime == null ? "[Дата окончания отсутствует]" : dateTime.Value.ToString();
    }
}
