using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Service
{
    public class getConnectDB
    {
        public static string get
        {
            get
            {
                HttpContext Current = new HttpContextAccessor().HttpContext;
                string s = "test";
                if (Current.User.Identity.IsAuthenticated)
                {
                    s = Current.User.Claims.First(x => x.Type == "otdel").Value;
                }
                if (Current.User.IsInRole("administrator"))
                {
                    if (Current.Request.Query.ContainsKey("otdel"))
                    {
                        s = Current.Request.Query["otdel"];
                        Current.Response.Cookies.Append("otdel", s);
                    }
                    else if (Current.Request.Cookies.ContainsKey("otdel"))
                    {
                        s = Current.Request.Cookies["otdel"];
                        if (s == "" || s == null)
                            s = "test";
                    }
                }

                return s;
            }
        }
    }
}
