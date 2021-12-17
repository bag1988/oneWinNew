using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oneWin.Data;
using oneWin.Models;
using oneWin.Models.baseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace oneWin.Controllers
{
    public class DocsController : Controller
    {
        private oneWinDbContext _context;

        public DocsController(oneWinDbContext context)
        {
            _context = context;
        }
        public ActionResult Index(string searchName = "", bool legal = false)
        {
            if (searchName == null)
                searchName = "";
            
            var doc = _context.BufDocRegistry.Where(x => x.DocRegistry.IP == legal && x.Docs.Name.Contains(searchName)).Select(x => x.Docs).Distinct().ToList();
            var g = _context.Docs.Where(x => !_context.BufDocRegistry.Select(x => x.DocID).Contains(x.DocID) && x.Name.Contains(searchName)).ToList();
            doc.AddRange(g);
            return View(doc);
        }

        [HttpPost]
        public ActionResult viewPartial(Guid idDoc)
        {
            ViewBag.idDoc = idDoc;
            var docReg = _context.BufDocRegistry.Where(x => x.DocID == idDoc).Select(x => x.DocRegistry).ToList();
            return PartialView(docReg);
        }


        public async Task<ActionResult> Create(Guid idDoc)
        {
            if (idDoc != Guid.Empty && await _context.Docs.AnyAsync(x => x.DocID == idDoc))
                return View(await _context.Docs.FirstOrDefaultAsync(x => x.DocID == idDoc));
            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(docsModel doc)
        {
            if (doc.Name == null || doc.Name == "")
            {
                ModelState.AddModelError("Name", "Значение не может быть пустым");
            }
            if (ModelState.IsValid)
            {                
                if (doc.DocID == Guid.Empty)
                {
                    await _context.Docs.AddAsync(doc);                    
                }
                else
                {
                    _context.Docs.Update(doc);
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
            var doc = _context.Docs.FirstOrDefault(x => x.DocID == idDoc);
            if (doc == null)
                return NoContent();
            return View(doc);
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid idDoc, string passswordAdmin)
        {
            var doc = _context.Docs.First(x => x.DocID == idDoc);

            _context.Docs.Remove(doc);
            await _context.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpPost]
        public async Task<IActionResult> addAdminProc(Guid idDoc, List<Guid?> idDocReg)
        {
            var docsList = _context.BufDocRegistry.Where(x => x.DocID == idDoc).ToList();
            
            var deleteDoc = docsList.Where(x => !idDocReg.Contains(x.RegID)).ToList();
            if (deleteDoc.Count > 0)
            {
                _context.BufDocRegistry.RemoveRange(deleteDoc);                
            }

            var docs = docsList.Select(x => x.RegID).ToList();
            foreach (Guid g in idDocReg)
            {
                if (!docs.Contains(g))
                {
                    await _context.BufDocRegistry.AddAsync(new bufDocRegistryModel { CheckViewSite = g.ToString(), DocID = idDoc, RegID = g });
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}
