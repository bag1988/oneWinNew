using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oneWin.Data;
using oneWin.Models.generalModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Controllers
{
    public class OrgIssuesController : Controller
    {
        private AppDbContext _context;
        public OrgIssuesController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string searchName = "")
        {
            if (searchName == null)
                searchName = "";
            return View(_context.OrgIssue.Where(x => x.Name.Contains(searchName)).ToList());
        }

        public ActionResult Create(Guid idDoc)
        {
            if (idDoc != Guid.Empty)
                return View(_context.OrgIssue.OrderBy(x => x.Name).First(x => x.Id == idDoc));
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(orgIssueModel doc)
        {
            if (ModelState.IsValid)
            {
                if (doc.Id == Guid.Empty)
                {
                    await _context.OrgIssue.AddAsync(doc);
                }
                else
                {
                    _context.OrgIssue.Update(doc);
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
            var doc = _context.OrgIssue.OrderBy(x => x.Name).FirstOrDefault(x => x.Id == idDoc);
            if (doc == null)
                return NoContent();
            return View(doc);
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid idDoc, string passswordAdmin)
        {
            var doc = _context.OrgIssue.OrderBy(x => x.Name).First(x => x.Id == idDoc);            
            _context.OrgIssue.Remove(doc);
            await _context.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpPost]
        public IActionResult viewPartial(Guid idDoc)
        {
            var doc = _context.OrgIssue.OrderBy(x => x.Name).First(x => x.Id == idDoc);
            return PartialView(doc);
        }

        [HttpPost]
        public async Task<JsonResult> getOrgIssue(string nameOrgIssue)
        {
            return Json(await _context.OrgIssue.AsNoTracking().Where(x => x.Name.Contains(nameOrgIssue)).Select(x => x.Name).Distinct().OrderBy(x => x).Take(10).ToListAsync());
        }

        //Наименование органа по коду
        [HttpPost]
        public async Task<JsonResult> getOrgIssueKod(int kod)
        {
            return Json(await _context.OrgIssue.AsNoTracking().OrderBy(x => x.Name).FirstAsync(x => x.kod== kod));
        }
    }
}
