using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using oneWin.Models;
using oneWin.Service;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace oneWin.TagHelpers
{
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;
        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public PageViewModel PageModel { get; set; }

        public string PageAction { get; set; }

        private Dictionary<string, object> listQuery;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            listQuery = new HttpContextAccessor().HttpContext.Request.Query.ToDictionary(x => x.Key, x => (object)x.Value);
           
            output.TagName = "div";
                        
            TagBuilder tag = new TagBuilder("ul");
            tag.AddCssClass("pagination");


            int p = PageModel.PageNumber-4;
            
            if(p<1)
            {
                p = 1;
            }

            if(PageModel.PageNumber>5)
            {
                TagBuilder currentItem = CreateTag(1, urlHelper);
                tag.InnerHtml.AppendHtml(currentItem);

                currentItem = CreateTag(0, urlHelper);
                tag.InnerHtml.AppendHtml(currentItem);
            }

            for (int i = p; i <= p + 9 && i <= PageModel.TotalPages; i++)
            {
                TagBuilder currentItem = CreateTag(i, urlHelper);
                tag.InnerHtml.AppendHtml(currentItem);
            }

            if (PageModel.TotalPages-5 > PageModel.PageNumber)
            {
                TagBuilder currentItem = CreateTag(0, urlHelper);
                tag.InnerHtml.AppendHtml(currentItem);

                currentItem = CreateTag(PageModel.TotalPages, urlHelper);
                tag.InnerHtml.AppendHtml(currentItem);
            }


            output.Content.AppendHtml(tag);
        }

        TagBuilder CreateTag(int pageNumber, IUrlHelper urlHelper)
        {
            if (listQuery.Any(x => x.Key == "page"))
                listQuery["page"] = pageNumber;
            else
                listQuery.Add("page", pageNumber);

            TagBuilder item = new TagBuilder("li");
            TagBuilder link = new TagBuilder("a");
            if (pageNumber == this.PageModel.PageNumber)
            {
                item.AddCssClass("active");
            }
            else
            {
                if (pageNumber != 0)
                    link.Attributes["href"] = urlHelper.Action(PageAction, null, new RouteValueDictionary(listQuery));               
            }
            item.AddCssClass("page-item");
            link.AddCssClass("page-link");
            if (pageNumber != 0)
                link.InnerHtml.Append(pageNumber.ToString());
            else
                link.InnerHtml.Append("...");
            item.InnerHtml.AppendHtml(link);
            return item;
        }
    }
}
