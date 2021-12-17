using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oneWin.Data;
using oneWin.Models.baseModel;
using oneWin.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Controllers
{    
    public class OrgZaprController : Controller
    {
        private oneWinDbContext _context;
        public OrgZaprController(oneWinDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string searchName = "")
        {
            if (searchName == null)
                searchName = "";
            return View(_context.OrgsZapr.Where(x=>x.Name.Contains(searchName)).ToList());
        }

        public ActionResult Create(Guid idDoc)
        {
            if (idDoc != Guid.Empty)
                return View(_context.OrgsZapr.First(x => x.OrgZaprID == idDoc));
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(orgsZaprModel doc)
        {
            if (doc.Name == null || doc.Name == "")
            {
                ModelState.AddModelError("Name", "Значение не может быть пустым");
            }
            if (ModelState.IsValid)
            {

                if (doc.OrgZaprID == Guid.Empty)
                {
                    await _context.OrgsZapr.AddAsync(doc);                    
                }
                else
                {
                    _context.OrgsZapr.Update(doc);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("index");
            }
            return View(doc);
        }
        
        public ActionResult Delete(Guid idDoc)
        {
            if (idDoc == Guid.Empty)
                RedirectToAction("index");
            var doc = _context.OrgsZapr.FirstOrDefault(x => x.OrgZaprID == idDoc);
            if (doc == null)
                return NoContent();
            return View(doc);
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid idDoc, string passswordAdmin)
        {
            var doc = _context.OrgsZapr.First(x => x.OrgZaprID == idDoc);

            _context.OrgsZapr.Remove(doc);
            await _context.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpPost]
        public IActionResult viewPartial(Guid idDoc)
        {
            var doc = _context.OrgsZapr.First(x => x.OrgZaprID == idDoc);
            return PartialView(doc);
        }

        [HttpPost]
        public async Task<JsonResult> getOrgZaprName(string nameOrg)
        {
            return Json(await _context.OrgsZapr.AsNoTracking().Where(x => x.Name.Contains(nameOrg)).Select(x => new { name = x.Name, id = x.OrgZaprID }).Distinct().OrderBy(x => x.name).Take(10).ToListAsync());
        }

        [HttpPost]
        public async Task<JsonResult> getOrgZapr(Guid idOrg)
        {
            return Json(await new getList(_context).getOrgZapr(idOrg));
        }
    }
}
