using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Service
{
    public class getUrlFile
    {
        public string urlFile()
        {
            HttpContext Current = new HttpContextAccessor().HttpContext;
            string s = "oneWin";
            string nameUrl = "//192.168.209.232/s$/";
            if (Current.User.Identity.IsAuthenticated)
            {
                s = Current.User.Claims.First(x => x.Type == "otdel").Value;
            }
            if (Current.User.IsInRole("administrator"))
            {
                if (Current.Request.Cookies.ContainsKey("otdel"))
                {
                    s = Current.Request.Cookies["otdel"];
                    if (s == "" || s == null)
                        s = "test";
                }
            }
            return (nameUrl + s + "/");
        }
        public string urlFileGeneral()
        {
            HttpContext Current = new HttpContextAccessor().HttpContext;           
            string nameUrl = "//192.168.209.232/s$/";
            return (nameUrl);
        }
    }
}
