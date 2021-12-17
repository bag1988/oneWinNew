using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using oneWin.Data;
using oneWin.Models.baseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace oneWin.Controllers
{
    public class AddressController : Controller
    {
        private oneWinDbContext _context;
        public AddressController(oneWinDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string searchName = "")
        {
            if (searchName == null)
                searchName = "";
            return View(_context.StreetNames.Where(x => x.Name.Contains(searchName)).ToList());
        }

        public ActionResult Create(string idDoc = "")
        {
            if (idDoc != "")
                return View(_context.StreetNames.FirstOrDefault(x => x.Name==idDoc));
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(streetModel doc, string oldName)
        {
            if (ModelState.IsValid)
            {
                var d = _context.StreetNames.FirstOrDefault(x => x.Name == oldName);
                if (d==null)
                {
                    _context.Database.ExecuteSqlRaw("insert into StreetNames (Name) VALUES ({0})", doc.Name);
                }
                else
                {                  
                    _context.Database.ExecuteSqlRaw("update StreetNames set Name={0} where Name={1}", doc.Name, d.Name);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("index");
            }
            return View(doc);
        }
       
        public ActionResult Delete(string idDoc = "")
        {
            if (idDoc == "")
                RedirectToAction("index");
            var doc = _context.StreetNames.FirstOrDefault(x => x.Name == idDoc);
            if (doc == null)
                return NoContent();
            return View(doc);
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string idDoc, string passswordAdmin)
        {
            _context.Database.ExecuteSqlRaw("delete from StreetNames where Name={0}", idDoc);
            await _context.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpPost]
        public async Task<IActionResult> viewPartial(string idDoc)
        {
            return PartialView(await _context.StreetNames.FirstAsync(x => x.Name == idDoc));
        }


        [HttpPost]
        public async Task<JsonResult> getAddress(string nameAddress)
        {
            return Json(await _context.StreetNames.AsNoTracking().Where(x=>x.Name.Contains(nameAddress)).Select(x=>x.Name).Distinct().OrderBy(x => x).Take(10).ToListAsync());
        }

        [HttpPost]
        public async Task<JsonResult> getHomeForAddress(string nameAddress)
        {
            return Json(await _context.Temp.AsNoTracking().OrderBy(x => x.NDOM).Where(x => x.NAME == nameAddress).Select(x => x.NDOM).Distinct().ToListAsync());
        }
        [HttpPost]
        public async Task<JsonResult> getFlatForAddress(string nameAddress, string nDom)
        {
            return Json(await _context.Temp.AsNoTracking().OrderBy(x => x.NDOM).Where(x => x.NAME == nameAddress && x.NDOM == nDom).Select(x => x.FLAT).Distinct().ToListAsync());
        }
    }
}
