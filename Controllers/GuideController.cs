using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using oneWin.Data;
using oneWin.Models.generalModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using oneWin.Models;
using Microsoft.EntityFrameworkCore;
using oneWin.Models.baseModel;

namespace oneWin.Controllers
{
    public class GuideController : Controller
    {
        private oneWinDbContext _context;
        public GuideController(oneWinDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string searchName = "", string nameTable="")
        {
            guide g = new();
            if (searchName == null)
                searchName = "";
            switch (nameTable)
            {
                case "ViewSiteInssue":
                    g.nameTitle = "Справочник виды сроков";
                    g.list = _context.ViewSiteInssue.AsNoTracking().Where(x=>x.Name.Contains(searchName)).Select(x=> new guideList { id=x.Id, Name=x.Name }).ToList(); break;
                case "ViewSiteCost":
                    g.nameTitle = "Справочник размер оплаты";
                    g.list = _context.ViewSiteCost.AsNoTracking().Where(x => x.name.Contains(searchName)).Select(x => new guideList { id = x.idCost, Name = x.name }).ToList(); break;                    
                case "ViewSiteSections":
                    g.nameTitle = "Справочник перечней";
                    g.list = _context.ViewSiteSections.AsNoTracking().Where(x => x.Name.Contains(searchName)).Select(x => new guideList { id = x.Id, Name = x.Name }).ToList(); break;
                case "ViewSiteValidaty":
                    g.nameTitle = "Справочник сроков действия документов";
                    g.list = _context.ViewSiteValidaty.AsNoTracking().Where(x => x.Name.Contains(searchName)).Select(x => new guideList { id = x.Id, Name = x.Name }).ToList(); break;
                default: return NoContent();
            }

            return View(g);
        }
        [HttpPost]
        public ActionResult GetInfo(int idDoc, string nameTable = "")
        {            
            if (idDoc != 0)
            {
                guideCreate g = new();
                switch (nameTable)
                {
                    case "ViewSiteInssue":
                        g = _context.ViewSiteInssue.AsNoTracking().Where(x => x.Id == idDoc).Select(x => new guideCreate { nameTitle = "Редактировать вид сроков", id=x.Id, Name=x.Name }).FirstOrDefault() ; break;
                    case "ViewSiteCost":
                        g = _context.ViewSiteCost.AsNoTracking().Where(x => x.idCost == idDoc).Select(x => new guideCreate { nameTitle = "Редактировать размер оплаты", id = x.idCost, Name = x.name }).FirstOrDefault(); break;
                    case "ViewSiteSections":
                        g = _context.ViewSiteSections.AsNoTracking().Where(x => x.Id == idDoc).Select(x => new guideCreate { nameTitle = "Редактировать перечень", id = x.Id, Name = x.Name }).FirstOrDefault(); break;
                    case "ViewSiteValidaty":
                        g = _context.ViewSiteValidaty.AsNoTracking().Where(x => x.Id == idDoc).Select(x => new guideCreate { nameTitle = "Редактировать срок действия документа", id = x.Id, Name = x.Name }).FirstOrDefault(); break;
                    default: return BadRequest();
                }
                return Json(g);
            }
            switch (nameTable)
            {
                case "ViewSiteInssue":
                    return Json("Добавить вид сроков");
                case "ViewSiteCost":
                    return Json("Добавить размер оплаты");
                case "ViewSiteSections":
                    return Json("Добавить перечень");
                case "ViewSiteValidaty":
                    return Json("Добавить срок действия документа");
                default: return BadRequest();
            }
        }

       
        [HttpPost]
        public async Task<IActionResult> Create(guideCreate doc, string nameTable)
        {
            if (ModelState.IsValid)
            {
                if (doc.id == 0)
                {
                    switch (nameTable)
                    {
                        case "ViewSiteInssue":
                            await _context.ViewSiteInssue.AddAsync(new siteInssueModel {Name=doc.Name }); break;
                        case "ViewSiteCost":
                            await _context.ViewSiteCost.AddAsync(new siteCostModel { name = doc.Name }); break;
                        case "ViewSiteSections":
                            await _context.ViewSiteSections.AddAsync(new siteSectionsModel { Name = doc.Name }); break;
                        case "ViewSiteValidaty":
                            await _context.ViewSiteValidaty.AddAsync(new siteValidatyModel { Name = doc.Name }); break;                        
                        default: return BadRequest();
                    }
                }
                else
                {
                    switch (nameTable)
                    {
                        case "ViewSiteInssue":
                            _context.ViewSiteInssue.Update(new siteInssueModel {Id=doc.id, Name = doc.Name }); break;
                        case "ViewSiteCost":
                            _context.ViewSiteCost.Update(new siteCostModel { idCost = doc.id, name = doc.Name }); break;
                        case "ViewSiteSections":
                            _context.ViewSiteSections.Update(new siteSectionsModel { Id = doc.id, Name = doc.Name }); break;
                        case "ViewSiteValidaty":
                            _context.ViewSiteValidaty.Update(new siteValidatyModel { Id = doc.id, Name = doc.Name }); break;                        
                        default: return BadRequest();
                    }
                }

                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete(int idDoc, string nameTable)
        {
            if (idDoc == 0 || nameTable == null)
                RedirectToAction("index");
            switch (nameTable)
            {
                case "ViewSiteInssue":
                    _context.ViewSiteInssue.Remove(_context.ViewSiteInssue.FirstOrDefault(x=>x.Id==idDoc)); break;
                case "ViewSiteCost":
                    _context.ViewSiteCost.Remove(_context.ViewSiteCost.FirstOrDefault(x => x.idCost == idDoc)); break;
                case "ViewSiteSections":
                    _context.ViewSiteSections.Remove(_context.ViewSiteSections.FirstOrDefault(x => x.Id == idDoc)); break;
                case "ViewSiteValidaty":
                    _context.ViewSiteValidaty.Remove(_context.ViewSiteValidaty.FirstOrDefault(x => x.Id == idDoc)); break;
                default: return BadRequest();
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
