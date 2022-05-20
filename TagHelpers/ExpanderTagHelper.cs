using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.TagHelpers
{
    public class ExpanderTagHelper: TagHelper
    {
        public string Message { get; set; }
        public string ButtonClass { get; set; }
        public override async void Process(TagHelperContext context, TagHelperOutput output)
        {
            string id = Guid.NewGuid().ToString("N");
            string chilren = output.GetChildContentAsync().Result.GetContent();

            string exp = $"expimg_{id}";
            string dexp = $"dexpimg_{id}";

            output.TagName = "div";

            output.Content.SetHtmlContent($@"
<button class=""w-100 btn"" type=""button""
        data-bs-toggle=""collapse""
        data-bs-target=""#collapseExample{id}""
        aria-expanded=""false""
        aria-controls=""collapseExample{id}""
        onclick=""if ( !$(this).hasClass('collapsed') ) {{ $({exp}).hide(); $({dexp}).show(); }} else {{ $({exp}).show(); $({dexp}).hide(); }}"">
    <div>
        <div class=""position-relative {ButtonClass} float-start"">{Message}</div>
        <div class=""position-relative float-end"" style=""height: 16px; width:1 16px;"">
            <img src=""/image/expander.png"" id=""{exp}"" />
            <img src=""/image/dexpander.png"" id=""{dexp}"" style=""display: none;"" />
        </div>
    </div>
</button>
<div class=""collapse"" id=""collapseExample{id}"">{chilren}</div>
");
        }
    }
}
