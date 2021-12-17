using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using oneWin.Data;
using oneWin.Models.baseModel;
using oneWin.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace oneWin.Controllers
{
   
    public class HeadController : Controller
    {
        private oneWinDbContext _context;

        public HeadController(oneWinDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string searchName = "", bool? legal = null)
        {
            if (searchName == null)
                searchName = "";
            if (legal == null)
                return View(await _context.Heads.AsNoTracking().Where(x => x.Name.Contains(searchName)&&(x.Sections.Any(s=>!s.DocRegs.Any())||!x.Sections.Any())).OrderBy(x=>x.Number).ToListAsync());
            if (legal == true)
                return View(await _context.Heads.AsNoTracking().Where(x => x.Name.Contains(searchName) && x.Sections.Any(s => s.DocRegs.Any(x=>x.IP))).OrderBy(x => x.Number).ToListAsync());
            return View(await _context.Heads.AsNoTracking().Where(x => x.Name.Contains(searchName) && x.Sections.Any(s => s.DocRegs.Any(x => !x.IP))).OrderBy(x => x.Number).ToListAsync());
        }

        [HttpPost]
        public ActionResult viewPartial(Guid idDoc)
        {
            ViewBag.idDoc = idDoc;
            var docReg = _context.Sections.Where(x => x.HeadID == idDoc).ToList();
            return PartialView(docReg);
        }
       
        public ActionResult Create(Guid idDoc)
        {
            if (idDoc != Guid.Empty)
                return View(_context.Heads.First(x => x.HedID == idDoc));
            return View();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(headModel doc)
        {           
            if (ModelState.IsValid)
            {                
                if (doc.HedID == Guid.Empty)
                {
                    await _context.Heads.AddAsync(doc);                    
                }
                else
                {
                    _context.Heads.Update(doc);
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
            var doc = _context.Heads.FirstOrDefault(x => x.HedID == idDoc);
            if (doc == null)
                return NoContent();
            return View(doc);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid idDoc, string passswordAdmin)
        {
            var doc = _context.Heads.First(x => x.HedID == idDoc);
            _context.Heads.Remove(doc);
            await _context.SaveChangesAsync();
            return RedirectToAction("index");
        }


        [HttpPost]
        public async Task<IActionResult> CreateSection(sectionsModel doc)
        {
            if (ModelState.IsValid)
            {
                if (doc.SectionID == Guid.Empty)
                {
                    await _context.Sections.AddAsync(doc);
                }
                else
                {
                    _context.Sections.Update(doc);
                }
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

      
        [HttpPost]
        public async Task<IActionResult> DeleteSection(Guid idSection)
        {            
            var doc = _context.Sections.First(x => x.SectionID == idSection);
            Guid? idDoc = doc.HeadID;
            _context.Sections.Remove(doc);
            await _context.SaveChangesAsync();
            return Json(idDoc);
        }
    }
}
