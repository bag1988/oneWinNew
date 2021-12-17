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
    public class NormDocController : Controller
    {
        private oneWinDbContext _context;
        public NormDocController(oneWinDbContext context)
        {
            _context = context;
        }
        public ActionResult Index(string searchName = "", bool legal = false)
        {
            if (searchName == null)
                searchName = "";

            var doc = _context.BufNormDoc.Where(x => x.DocRegistry.IP == legal && x.NormDoc.Name.Contains(searchName)).Select(x => x.NormDoc).Distinct().ToList();
            var g = _context.NormDoc.Where(x => !_context.BufNormDoc.Select(x => x.NormID).Contains(x.NormID) && x.Name.Contains(searchName)).ToList();
            doc.AddRange(g);
            return View(doc);
        }

        [HttpPost]
        public ActionResult viewPartial(Guid idDoc)
        {
            ViewBag.idDoc = idDoc;
            ViewBag.urlDoc = _context.NormDoc.First(x => x.NormID == idDoc).URL;
            var docReg = _context.BufNormDoc.Where(x => x.NormID == idDoc).Select(x => x.DocRegistry).ToList();
            return PartialView(docReg);
        }

        public ActionResult Create(Guid idDoc)
        {
            if (idDoc != Guid.Empty)
                return View(_context.NormDoc.First(x => x.NormID == idDoc));
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(normDocModel doc)
        {
            if (ModelState.IsValid)
            {
                if (doc.NormID == Guid.Empty)
                {
                    await _context.NormDoc.AddAsync(doc);                    
                }
                else
                {
                    _context.NormDoc.Update(doc);
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
            var doc = _context.NormDoc.FirstOrDefault(x => x.NormID == idDoc);
            if (doc == null)
                return NoContent();
            return View(doc);
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid idDoc, string passswordAdmin)
        {
            var doc = _context.NormDoc.First(x => x.NormID == idDoc);

            _context.NormDoc.Remove(doc);
            await _context.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpPost]
        public async Task<IActionResult> addAdminProc(Guid idDoc, List<Guid?> idDocReg)
        {
            var docsList = _context.BufNormDoc.Where(x => x.NormID == idDoc).ToList();

            var deleteDoc = docsList.Where(x => !idDocReg.Contains(x.RegID)).ToList();
            if (deleteDoc.Count > 0)
            {
                _context.BufNormDoc.RemoveRange(deleteDoc);
            }

            var docs = docsList.Select(x => x.RegID).ToList();
            foreach (Guid g in idDocReg)
            {
                if (!docs.Contains(g))
                {
                    await _context.BufNormDoc.AddAsync(new bufNormDocModel {NormID = idDoc, RegID = g });
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
