using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ogma3.Services.TagHelpers
{
    public class TextTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var content = await output.GetChildContentAsync(HtmlEncoder.Default);
            var innerHtml = string.Join("</p><p>",
                content.GetContent()
                    .Split('\n', '\r')
                    .Where(s => !string.IsNullOrEmpty(s))
                );
            
            output.TagName = "p";
            output.Content.SetHtmlContent(innerHtml + content.GetContent().Split('\n', '\r').Length);
        }
    }
}