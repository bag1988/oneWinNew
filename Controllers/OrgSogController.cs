using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using oneWin.Data;
using oneWin.Models.baseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Controllers
{
    public class OrgSogController : Controller
    {
        private oneWinDbContext _context;
        public OrgSogController(oneWinDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string searchName = "")
        {
            if (searchName == null)
                searchName = "";
            return View(_context.SoglOrg.Where(x => x.DeptName.Contains(searchName)).ToList());
        }

        public ActionResult Create(Guid idDoc)
        {
            if (idDoc != Guid.Empty)
                return View(_context.SoglOrg.First(x => x.SoglOrgID == idDoc));
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(sogOrgModel doc)
        {
            if (ModelState.IsValid)
            {
                if (doc.SoglOrgID == Guid.Empty)
                {
                    await _context.SoglOrg.AddAsync(doc);
                }
                else
                {
                    _context.SoglOrg.Update(doc);
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
            var doc = _context.SoglOrg.FirstOrDefault(x => x.SoglOrgID == idDoc);
            if (doc == null)
                return NoContent();
            return View(doc);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid idDoc, string passswordAdmin)
        {
            var doc = _context.SoglOrg.First(x => x.SoglOrgID == idDoc);

            _context.SoglOrg.Remove(doc);
            await _context.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpPost]
        public IActionResult viewPartial(Guid idDoc)
        {
            var doc = _context.SoglOrg.First(x => x.SoglOrgID == idDoc);
            return PartialView(doc);
        }
    }
}
