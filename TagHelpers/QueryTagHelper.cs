using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using oneWin.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.TagHelpers
{

    public class QueryTagHelper : TagHelper
    {
        public object modelPage { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            HttpContext Current = new HttpContextAccessor().HttpContext;
            var queryStrCookie = Current.Response.Headers["Set-cookie"];
            Dictionary<string, string> queryList = null;
            if (queryStrCookie.Count()>0)
            {
                Dictionary<string, string> s = queryStrCookie[0].Split(";").ToDictionary(x=> x.Split("=")[0], x=> x.Split("=")[1]);

                if (s.ContainsKey("queryList"))
                {                   
                    queryList = System.Net.WebUtility.UrlDecode(s["queryList"]).Split("&").Where(x => x.Split("=")[1] != "").ToDictionary(x => x.ToString().Split("=")[0], x => x.ToString().Split("=")[1]);
                }
            }

            if (queryList != null)
            {
                if (modelPage != null)
                {
                    foreach (var o in queryList.Where(x => x.Value != "" && x.Key != "otdel" && x.Key != "page"))
                    {
                        if (!modelPage.GetType().GetProperties().Any(x => x.Name == o.Key))
                        {
                            TagBuilder input = new TagBuilder("input");
                            input.Attributes["type"] = "hidden";
                            input.Attributes["name"] = o.Key;
                            input.Attributes["value"] = o.Value;
                            output.Content.AppendHtml(input);
                        }
                    }
                }
            }
        }
    }
}
