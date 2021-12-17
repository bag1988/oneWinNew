using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using oneWin.Data;
using oneWin.Models.generalModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace oneWin.Service
{
    public class RoleFiltrAuthorized:Attribute, IAsyncResourceFilter
    {
        private AppDbContext _context;
        public RoleFiltrAuthorized(AppDbContext context)
        {
            _context = context;
        }
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {   
            
            var controllerName = context.HttpContext.Request.RouteValues["controller"];
            var actionName = context.HttpContext.Request.RouteValues["action"];
            var razorPage = context.HttpContext.Request.RouteValues["page"];
            bool forbid = false;
            List<string> noRequest = new List<string> { "/identity/account/login", "/identity/account/register" };
            List<string> nameForm = new List<string> { "LName", "FName", "MName", "PhoneNo", "MobPhone", "e_mail", "PassportNo", "PersonalNo", "City" };
            List<string> queryRequest = new();
            if (!noRequest.Contains(context.HttpContext.Request.Path.ToString().ToLower()))
            {
                if (context.HttpContext.Request.Query.Count > 0)
                {
                    queryRequest.AddRange(context.HttpContext.Request.Query.Select(x => x.Key + "=" + x.Value));
                }
                if (context.HttpContext.Request.HasFormContentType)
                {
                    queryRequest.AddRange(context.HttpContext.Request.Form.Where(x => !nameForm.Select(x=>x.ToLower()).Contains(x.Key.ToLower())).Select(x => x.Key + "=" + x.Value));
                }
            }
            actionModel action = new();
            if(controllerName!=null&& actionName!=null)
            {
                if(await _context.action.Include(x => x.controller).AsNoTracking().AnyAsync(x => x.controller.nameController == controllerName.ToString().ToLower() && x.nameAction == actionName.ToString().ToLower()))
                {
                    action = await _context.action.Include(x => x.controller).AsNoTracking().FirstAsync(x => x.controller.nameController == controllerName.ToString().ToLower() && x.nameAction == actionName.ToString().ToLower());
                }
            }
               
            logerModel log = new logerModel
            {
                dateRequest = DateTime.Now,
                userName = context.HttpContext.User.Identity.IsAuthenticated? context.HttpContext.User.Identity.Name: "noAuthenticated",
                actionId = action.id != 0 ? action.id : null,
                queryRequest = string.Join("&", queryRequest),
                urlRequest = context.HttpContext.Request.Path,
                ipAdres = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                otdelName = context.HttpContext.User.Identity.IsAuthenticated? context.HttpContext.User.Claims.First(x => x.Type == "otdel").Value: ""
            };

            if (context.HttpContext.User.Identity.Name != "admin")
            {
                logger l = new logger();
                l.saveLoger(log);
            }

            if (!context.HttpContext.User.IsInRole("administrator"))
            {
                if ((string)actionName == "getPageUser")
                    forbid = true;
                else if (controllerName != null)
                {
                    var userRole = await _context.UserRoles.AsNoTracking().Where(x => x.UserId == context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)).Select(x => x.RoleId).ToListAsync();
                    var roleController = await _context.roleController.Include(x => x.controller).AsNoTracking().Where(x => x.controller.addressController == controllerName.ToString()).Select(x => x.RoleId).ToListAsync();
                    var roleAction = await _context.roleAction.Include(x => x.action).ThenInclude(x => x.controller).AsNoTracking().Where(x => x.action.addressAction == (string)actionName && x.action.controller.addressController == controllerName.ToString()).Select(x => x.RoleId).ToListAsync();

                    if (roleAction.Count > 0)
                    {
                        forbid = roleAction.Any(x => userRole.Contains(x));
                    }
                    else if (roleController.Count > 0)
                    {
                        forbid = roleController.Any(x => userRole.Contains(x));
                    }
                    else
                        forbid = false;
                }
                else
                {
                    if (razorPage != null)
                        forbid = true;
                }

               

            }
            else
                forbid = true;

            if (forbid)
                await next();
            else
                context.Result = new ForbidResult();

        }
       
    }

}
