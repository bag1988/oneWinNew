using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oneWin.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using oneWin.Models.generalModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Reflection;

namespace oneWin.Areas.admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "administrator")]
    public class ActionController : Controller
    {
        private AppDbContext _context;
        public ActionController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int? idController)
        {
            var controllers = Assembly.GetExecutingAssembly().GetTypes().Where(type => typeof(Controller).IsAssignableFrom(type)).Select(x => x.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Select(m=>m.Name.ToLower()).Distinct()).ToList();
            List<string> methods = new();

            foreach(var c in controllers)
            {
                methods.AddRange(c.Where(x=>!methods.Contains(x)));
            }


            ViewBag.Action = methods;
            ViewBag.Controller = new SelectList(await _context.controller.AsNoTracking().ToListAsync(), "id", "nameController", idController);

            return View(await _context.action.AsNoTracking().Include(x => x.controller).Include(x => x.roleAction).ThenInclude(x => x.Role).AsNoTracking().Where(x => idController == null || x.idController == idController).ToListAsync());
            
        }

        public async Task<IActionResult> Controller()
        {

            ViewBag.Controller = Assembly.GetExecutingAssembly().GetTypes().Where(type => typeof(Controller).IsAssignableFrom(type)).Select(x => x.Name.ToLower().Replace("controller", "")).ToList();

            return View(await _context.controller.AsNoTracking().Include(x => x.roleController).ThenInclude(x => x.Role).AsNoTracking().ToListAsync());
        }

        public async Task<IActionResult> CreateController(int? id)
        {            
            if(id!=null)
            {
                if (await _context.controller.AnyAsync(x => x.id == id))
                    return View(await _context.controller.FindAsync(id));
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateController(controlerModel model)
        {
            if(model.id==0)
            {
                ModelState.Remove("id");
            }
            if (ModelState.IsValid)
            {
                model.addressController = model.addressController.ToLower();
                if (model.id == 0)
                {
                    if(await _context.controller.AnyAsync(x=>x.addressController == model.addressController))
                    {
                        ModelState.AddModelError("addressController", "Контроллер с таким адресом уже существует!");
                        return View(model);
                    }
                    await _context.controller.AddAsync(model);
                }
                else
                    _context.controller.Update(model);

                await _context.SaveChangesAsync();

                return Redirect("/admin/action/Controller");
            }
            
            return View(model);
        }

        public async Task<IActionResult> CreateAction(int? id)
        {
            ViewBag.Controller = new SelectList(await _context.controller.AsNoTracking().ToListAsync(), "id", "nameController");
            if (id != null)
            {
                if (await _context.action.AnyAsync(x => x.id == id))
                    return View(await _context.action.FindAsync(id));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateAction(actionModel model)
        {
            if (model.id == 0)
            {
                ModelState.Remove("id");
            }
            ViewBag.Controller = new SelectList(await _context.controller.AsNoTracking().ToListAsync(), "id", "nameController");
            if (ModelState.IsValid)
            {
                model.addressAction = model.addressAction.ToLower();
                if (model.id == 0)
                {
                    if (await _context.action.AnyAsync(x => x.addressAction == model.addressAction&&model.idController==x.idController))
                    {
                        ModelState.AddModelError("addressAction", "Действие с таким адресом уже существует!");
                        return View(model);
                    }
                    await _context.action.AddAsync(model);
                }                    
                else
                    _context.action.Update(model);

                await _context.SaveChangesAsync();

                return Redirect("/admin/action");
            }
            
            return View(model);
        }

        public async Task<IActionResult> getRoleList()
        {
            return Json(await _context.Roles.AsNoTracking().OrderBy(x=>x.Name).ToDictionaryAsync(x => x.Id, x => x.Name));
        }
        [HttpPost]
        public async Task<IActionResult> AddRoleAction(int idAction, List<string> roleList)
        {            
            if (idAction > 0)
            {
                var actionRole = await _context.roleAction.AsNoTracking().Where(x => x.idAction == idAction).ToListAsync();

                if (actionRole != null)
                {
                    _context.roleAction.RemoveRange(actionRole.Where(x => !roleList.Contains(x.RoleId)));
                }

                await _context.roleAction.AddRangeAsync(roleList.Where(x => !actionRole.Select(x => x.RoleId).Contains(x)).Select(x => new roleForActionModel { idAction = idAction, RoleId = x }));

                await _context.SaveChangesAsync();
            }
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> AddRoleController(int idController, List<string> roleList)
        {
            if (idController > 0)
            {
                var controllerRole = await _context.roleController.AsNoTracking().Where(x => x.idController == idController).ToListAsync();

                if (controllerRole != null)
                {
                    _context.roleController.RemoveRange(controllerRole.Where(x => !roleList.Contains(x.RoleId)));
                }

                await _context.roleController.AddRangeAsync(roleList.Where(x => !controllerRole.Select(x => x.RoleId).Contains(x)).Select(x => new roleForControllerModel { idController = idController, RoleId = x }));

                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string typeRole)
        {
            if (typeRole =="action")
            {
                if (await _context.action.AnyAsync(x => x.id == id))
                {
                    _context.roleAction.RemoveRange(_context.roleAction.Where(x => x.idAction == id));
                    _context.action.Remove(_context.action.Find(id));
                    await _context.SaveChangesAsync();
                }
            }
            if (typeRole == "controller")
            {
                if (await _context.controller.AnyAsync(x => x.id == id))
                {
                    _context.roleController.RemoveRange(_context.roleController.Where(x => x.idController == id));
                    _context.roleAction.RemoveRange(_context.roleAction.Where(x => x.action.idController == id));
                    _context.action.RemoveRange(_context.action.Where(x => x.idController == id));
                    _context.controller.Remove(_context.controller.Find(id));
                    await _context.SaveChangesAsync();
                }
            }
            return Ok();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> getPageUser()
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();
            actionForUser forUser = new();
            var userRoles = await _context.UserRoles.AsNoTracking().Where(x => x.UserId == HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)).Select(x => x.RoleId).ToListAsync();

            var controllers = Assembly.GetExecutingAssembly().GetTypes().Where(type => typeof(Controller).IsAssignableFrom(type)).ToList();
            var roleAction = await _context.roleAction.Include(x => x.action).ThenInclude(x=>x.controller).AsNoTracking().ToListAsync();
            forUser.roleName = User.IsInRole("administrator");

            var nameController = await _context.roleController.Include(x => x.controller).AsNoTracking().Where(x => userRoles.Contains(x.RoleId)).Select(x => (x.controller.addressController + "controller").ToLower()).ToListAsync();
            forUser.url = new();
            forUser.addressAction = roleAction.Where(x => userRoles.Contains(x.RoleId)).Select(x => x.action.addressAction).ToList();

            if (forUser.roleName)
                return Json(forUser);

            foreach (var controller in controllers)
            {
                
                if (nameController.Contains(controller.Name.ToLower()))
                {
                    var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Select(x=>x.Name.ToLower()).Distinct();
                    foreach (var method in methods)
                    {
                        if (roleAction.Any(x => x.action.addressAction.ToLower() == method && (x.action.controller.addressController + "controller").ToLower() == controller.Name.ToLower()))
                            if (!roleAction.Any(x => x.action.addressAction.ToLower() == method && (x.action.controller.addressController + "controller").ToLower() == controller.Name.ToLower() && userRoles.Contains(x.RoleId)))
                                continue;
                        if (!forUser.addressAction.Select(x => x.ToLower()).Contains(method))
                            forUser.addressAction.Add(method);
                        forUser.url.Add("/" + controller.Name.ToLower().Replace("controller", "/") + method);
                    }
                }                
            }
            return Json(forUser);
        }
    }
}
