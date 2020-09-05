using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;

namespace Ogma3.Services.TagHelpers
{
    public class AvatarTagHelper : TagHelper
    {
        private readonly IConfiguration _config;
        public AvatarTagHelper(IConfiguration config)
        {
            _config = config;
        }

        public string Src { get; set; }
        public int? Size { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var url = _config["cdn"] + Src;
            if (Size != null)
            {
                url = $"{url}?s={Size}";
                output.Attributes.SetAttribute("style", $"width:{Size}px;height:{Size}px");
            }

            output.TagName = "img";
            output.AddClass("avatar", HtmlEncoder.Default);
            output.Attributes.SetAttribute("src", url);
            output.Attributes.Remove(new TagHelperAttribute("title"));
        }
        
    }
}