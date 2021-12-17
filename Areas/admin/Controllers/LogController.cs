using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using oneWin.Data;
using oneWin.Models.generalModel;
using oneWin.Service;

namespace oneWin.Areas.admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "administrator")]
    public class LogController : Controller
    {
        private AppDbContext _acontext;

        public LogController(AppDbContext acontext)
        {
            _acontext = acontext;
        }
        public async Task<IActionResult> Index()
        {
            var log = await logger.getLoger(DateTime.Now);
            var action = await _acontext.action.AsNoTracking().Where(x => log.Select(x => x.actionId).Contains(x.id)).ToListAsync();
            var logView = log.Select(x => new logerView
            {
                actionId = x.actionId,
                action = action.FirstOrDefault(a => a.id == (int)x.actionId),
                otdelName = x.otdelName,
                userName = x.userName,
                dateRequest = x.dateRequest,
                urlRequest = x.urlRequest,
                queryRequest = x.queryRequest,
                ipAdres = x.ipAdres
            }).ToList();
            return View(logView);
        }

        [HttpPost]
        public async Task<IActionResult> viewPartial(DateTime dateRequset, TimeSpan? dateTimeStart, TimeSpan? dateTimeStop, string userName = null, string controllerName = null, string otdelName = null, string actionName=null)
        {
            var log = await logger.getLoger(dateRequset, dateTimeStart, dateTimeStop, userName, controllerName, otdelName, actionName);
            if (log == null || !log.Any())
                return PartialView(null);
            var action = await _acontext.action.AsNoTracking().Where(x => log.Select(x => x.actionId).Contains(x.id)).ToListAsync();
            var logView = log.Select(x => new logerView
            {
                actionId = x.actionId,
                action = action.FirstOrDefault(a => a.id == (int)x.actionId),
                otdelName = x.otdelName,
                userName = x.userName,
                dateRequest = x.dateRequest,
                urlRequest = x.urlRequest,
                queryRequest = x.queryRequest,
                ipAdres = x.ipAdres
            }).ToList();



            return PartialView(logView);
        }

        [HttpPost]
        public IActionResult getRequest(string strRequest)
        {
            Dictionary<string, string> request = new();
            if (!string.IsNullOrEmpty(strRequest))
                request = strRequest.Split("&").ToDictionary(x => x.Split("=")[0], x => x.Split("=")[1]);
            return PartialView(request);
        }

        [HttpPost]
        public async Task<JsonResult> getUserName(DateTime date)
        {
            return Json(await logger.getUserLogList(date));
        }
        [HttpPost]
        public async Task<JsonResult> geControllerName(DateTime date)
        {
            var contName = await _acontext.controller.AsNoTracking().ToListAsync();
            var list = await logger.getControllerLogList(date);
            return Json(contName.Where(x => list.Contains(x.addressController.ToLower())));
        }

        [HttpPost]
        public async Task<JsonResult> geActionName(DateTime date)
        {
            return Json(await logger.getActionLogList(date));
        }

        [HttpPost]
        public async Task<JsonResult> getOtdelName(DateTime date)
        {
            return Json(await logger.getOtdelLogList(date));
        }
    }
}
