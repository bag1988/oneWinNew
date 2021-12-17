using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oneWin.Data;
using oneWin.Models;
using oneWin.Models.baseModel;
using oneWin.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Controllers
{
    public class DocsZaprController : Controller
    {
        private oneWinDbContext _context;

        public DocsZaprController (oneWinDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string searchName = "", bool legal = false)
        {
            if (searchName == null)
                searchName = "";
            var doc = _context.BufOrgsZApr.Where(x => x.DocRegistry.IP == legal && x.ZaprDocs.Name.Contains(searchName)).Select(x => new zaprViewModel { idZapr = x.ZaprDocs.ZaprDocID, Name = x.ZaprDocs.Name }).Distinct().ToList();
            var g = _context.ZaprDocs.Where(x => !_context.BufOrgsZApr.Select(x => x.ZaprDocID).Contains(x.ZaprDocID) && x.Name.Contains(searchName)).Select(x => new zaprViewModel { idZapr = x.ZaprDocID, Name = x.Name }).Distinct().ToList();
            doc.AddRange(g);
            return View(doc);
        }

        [HttpPost]
        public async Task<IActionResult> viewPartial(Guid idDoc)
        {
            ViewBag.idDoc = idDoc;
            var docReg = await _context.BufOrgsZApr.Where(x => x.ZaprDocID == idDoc).Select(x => x.DocRegistry).ToListAsync();
            return PartialView(docReg);
        }

        public async Task<IActionResult> Create(Guid idDoc)
        {
            if (idDoc != Guid.Empty)
                return View(await _context.ZaprDocs.FirstOrDefaultAsync(x => x.ZaprDocID == idDoc));
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(zaprDocModel doc, IFormFile fileUrl)
        {
            if (doc.Name == null || doc.Name == "")
            {
                ModelState.AddModelError("Name", "Значение не может быть пустым");
            }
            string fileExe = Path.GetExtension(doc.File);
            if (fileExe != null&&fileExe != "")
            {
                doc.File = doc.File.Replace(fileExe, "");
            }
            if (ModelState.IsValid)
            {
                getUrlFile getUrl = new getUrlFile();
                string nameDir = "Template/";
                string urlSave;
                if (fileUrl != null)
                {
                    urlSave = getUrl.urlFile() + nameDir;
                    if (!Directory.Exists(urlSave))
                        Directory.CreateDirectory(urlSave);
                    urlSave += doc.File + Path.GetExtension(fileUrl.FileName);
                    doc.File = doc.File + Path.GetExtension(fileUrl.FileName);
                    using (var stream = new FileStream(urlSave, FileMode.Create))
                    {
                        await fileUrl.CopyToAsync(stream);
                    }
                }
                else
                    doc.File = doc.File + fileExe;
                if (doc.ZaprDocID == Guid.Empty)
                {
                    await _context.ZaprDocs.AddAsync(doc);                    
                }
                else
                {
                    _context.ZaprDocs.Update(doc);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("index");
            }
            return View(doc);
        }
       
        public async Task<IActionResult> Delete(Guid idDoc)
        {
            if (idDoc == Guid.Empty)
                RedirectToAction("index");
            var doc =await _context.ZaprDocs.FirstOrDefaultAsync(x => x.ZaprDocID == idDoc);
            if (doc == null)
                return NoContent();
            return View(doc);
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid idDoc, string passswordAdmin)
        {
            var doc = _context.ZaprDocs.First(x => x.ZaprDocID == idDoc);

            _context.ZaprDocs.Remove(doc);
            await _context.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpPost]
        public async Task<IActionResult> addAdminProc(Guid idDoc, List<Guid?> idDocReg)
        {
            var docsList = _context.BufOrgsZApr.Where(x => x.ZaprDocID == idDoc).ToList();

            var deleteDoc = docsList.Where(x => !idDocReg.Contains(x.RegID)).ToList();
            if (deleteDoc.Count > 0)
            {
                _context.BufOrgsZApr.RemoveRange(deleteDoc);
            }

            var docs = docsList.Select(x => x.RegID).ToList();
            foreach (Guid g in idDocReg)
            {
                if (!docs.Contains(g))
                {
                    await _context.BufOrgsZApr.AddAsync(new bufOrgsZAprModel { CheckViewSite = g.ToString(), ZaprDocID = idDoc, RegID = g });
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> details(Guid idDoc)
        {
            var doc = await _context.ZaprDocs.FirstOrDefaultAsync(x => x.ZaprDocID == idDoc);
            return PartialView(doc);
        }
    }
}
