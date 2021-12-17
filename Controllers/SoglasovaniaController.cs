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
    public class SoglasovaniaController : Controller
    {
        private oneWinDbContext _context;
        public SoglasovaniaController(oneWinDbContext context)
        {
            _context = context;
        }
        public ActionResult Index(string searchName = "", bool legal = false)
        {
            if (searchName == null)
                searchName = "";

            var doc = _context.BufSoglDoc.Where(x => x.DocRegistry.IP == legal && x.Soglasovaniya.Name.Contains(searchName)).Select(x => x.Soglasovaniya).Distinct().ToList();
            var g = _context.Soglasovaniya.Where(x => !_context.BufSoglDoc.Select(x => x.SoglID).Contains(x.SoglID) && x.Name.Contains(searchName)).ToList();
            doc.AddRange(g);
            return View(doc);
        }

        [HttpPost]
        public ActionResult viewPartial(Guid idDoc)
        {
            ViewBag.idDoc = idDoc;
            var docReg = _context.BufSoglDoc.Where(x => x.SoglID == idDoc).Select(x => x.DocRegistry).ToList();
            return PartialView(docReg);
        }

        public ActionResult Create(Guid idDoc)
        {
            if (idDoc != Guid.Empty)
                return View(_context.Soglasovaniya.First(x => x.SoglID == idDoc));
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(soglasovaniyaModel doc)
        {
            if (ModelState.IsValid)
            {
                if (doc.SoglID == Guid.Empty)
                {
                    await _context.Soglasovaniya.AddAsync(doc);
                }
                else
                {
                    _context.Soglasovaniya.Update(doc);
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
            var doc = _context.Soglasovaniya.FirstOrDefault(x => x.SoglID == idDoc);
            if (doc == null)
                return NoContent();
            return View(doc);
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid idDoc, string passswordAdmin)
        {
            var doc = _context.Soglasovaniya.First(x => x.SoglID == idDoc);

            _context.Soglasovaniya.Remove(doc);
            await _context.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpPost]
        public async Task<IActionResult> addAdminProc(Guid idDoc, List<Guid?> idDocReg)
        {
            var docsList = _context.BufSoglDoc.Where(x => x.SoglID == idDoc).ToList();

            var deleteDoc = docsList.Where(x => !idDocReg.Contains(x.RegID)).ToList();
            if (deleteDoc.Count > 0)
            {
                _context.BufSoglDoc.RemoveRange(deleteDoc);
            }

            var docs = docsList.Select(x => x.RegID).ToList();
            foreach (Guid g in idDocReg)
            {
                if (!docs.Contains(g))
                {
                    await _context.BufSoglDoc.AddAsync(new bufSoglDocModel { SoglID = idDoc, RegID = g });
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public ActionResult details(Guid idDoc)
        {
            var doc = _context.Soglasovaniya.First(x => x.SoglID == idDoc);
            return PartialView(doc);
        }
    }
}
