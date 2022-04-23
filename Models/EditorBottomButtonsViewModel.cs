using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
    public class EditorBottomButtonsViewModel
    {
        public bool ShowAction { get; set; } = false;
        public string Theme { get; set; } = "primary";
        public string ActionName { get; set; } = "Подтвердить";
        public string Action { get; set; }
        public string BackAction { get; set; }
        public string BackName { get; set; } = "Назад";

    }
}
