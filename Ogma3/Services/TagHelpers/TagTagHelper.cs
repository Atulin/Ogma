using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Ogma3.Data.DTOs;
using Ogma3.Data.Models;
using Utils.Extensions;

namespace Ogma3.Services.TagHelpers
{
    public class TagTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly LinkGenerator _generator;

        public TagTagHelper(IHttpContextAccessor accessor, LinkGenerator generator)
        {
            _accessor = accessor;
            _generator = generator;
        }

        public TagDTO Tag { get; set; }
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var href = _generator
                .GetUriByPage(_accessor.HttpContext, "/Tag", null, new {id = Tag.Id, slug = Tag.Slug});

            output.TagName = "a";
            output.AddClass("tag", NullHtmlEncoder.Default);
            
            output.Attributes.Add("href", href);
            output.Attributes.Add("title", Tag.Namespace);

            output.Content.AppendHtml(Tag.Color == null
                ? "<div class='bg'></div>"
                : $@"<div class='bg' style='background-color: #{Tag.Color.Trim('#')}'></div>");

            output.Content.AppendHtml($@"<span class='name'>{Tag.Name}</span>");
            
        }
    }
}