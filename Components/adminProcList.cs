using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using oneWin.Data;
using oneWin.Models;
using oneWin.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace oneWin.Components
{
    public class adminProcList:ViewComponent
    {
        private getAdminProc _proc;

        public adminProcList(getAdminProc proc)
        {
            _proc = proc;
        }
        public IViewComponentResult Invoke(string legalStr)
        {
            return View(_proc.getList(legalStr));
        }
    }
}
