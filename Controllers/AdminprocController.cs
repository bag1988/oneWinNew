using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using oneWin.Data;
using oneWin.Models;
using oneWin.Models.baseModel;
using oneWin.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace oneWin.Controllers
{
    public class AdminprocController : Controller
    {
        private oneWinDbContext _context;
        private readonly getList _getSec;

        public AdminprocController(oneWinDbContext context, getList getSec)
        {
            _context = context;
            _getSec = getSec;
        }
        public IActionResult Index(bool legal = false)
        {           
            return View();
        }

        public IActionResult Create(string idDoc="")
        {
            if(idDoc!="")
            {
                var doc = _context.DocRegistry.FirstOrDefault(x => x.RegID.ToString() == idDoc);
                doc.TypeIssue = doc.TypeIssue != null ? doc.TypeIssue.Trim() : null;
                doc.Cost = doc.Cost != null ? doc.Cost.Trim() : null;
                doc.Validaty = doc.Validaty != null ? doc.Validaty.Trim() : null;
                return View(doc);
            }
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(docRegModel doc, IFormFile fileUrlSite, IFormFile fileUrl)
        {

            if (ModelState.IsValid)
            {               
                getUrlFile getUrl = new getUrlFile();
                string nameDir = "TemplateZayav/";
                string nameDirSite = "TemplateZayav/Site/";
                string urlSave;
                string fileExe = Path.GetExtension(doc.URL);
                if (fileExe != null && fileExe != "")
                {
                    doc.URL = doc.URL.Replace(fileExe, "");
                }
                if (fileUrl != null)
                {
                    urlSave = getUrl.urlFile() + nameDir;
                    if (!Directory.Exists(urlSave))
                        Directory.CreateDirectory(urlSave);                    
                    using (var stream = new FileStream(urlSave + doc.URL + Path.GetExtension(fileUrl.FileName), FileMode.Create))
                    {
                        await fileUrl.CopyToAsync(stream);
                    }
                    doc.URL = doc.URL + Path.GetExtension(fileUrl.FileName);
                }
                else
                    doc.URL = doc.URL + fileExe;
                fileExe = Path.GetExtension(doc.URLSite);
                if (fileExe != null && fileExe != "")
                {
                    doc.URLSite = doc.URLSite.Replace(fileExe, "");
                }
                if (fileUrlSite != null)
                {
                    urlSave = getUrl.urlFile() + nameDirSite;
                    if (!Directory.Exists(urlSave))
                        Directory.CreateDirectory(urlSave);
                    using (var stream = new FileStream(urlSave + doc.URLSite + Path.GetExtension(fileUrlSite.FileName), FileMode.Create))
                    {
                        await fileUrlSite.CopyToAsync(stream);
                    }
                    doc.URLSite = doc.URLSite + Path.GetExtension(fileUrlSite.FileName);
                }
                else
                    doc.URLSite = doc.URLSite + fileExe;


                if (doc.RegID == Guid.Empty)
                {
                   await _context.DocRegistry.AddAsync(doc); 
                }
                else
                {
                    _context.DocRegistry.Update(doc);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("index");
            }
            return View(doc);
        }

        [HttpPost]
        public async Task<JsonResult> sectionsList(string idHead)
        {            
            var d = await _getSec.sectList(idHead);
            return Json(d);
        }

        [HttpPost]
        public async Task<JsonResult> adminProcList(string idSection)
        {
            var d = await _getSec.adminProcList(idSection);            
            return Json(d);
        }

        public async Task<IActionResult> selected(string idDoc)
        {
            if(_context.DocRegistry.Any(x=>x.RegID.ToString()==idDoc))
            {
                var doc = _context.DocRegistry.OrderBy(x => x.RegName).First(x => x.RegID.ToString() == idDoc);
                if (doc.Selected)
                    doc.Selected = false;
                else
                    doc.Selected = true;
                _context.DocRegistry.Update(doc);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("index");
        }

        
        [HttpGet]
        public IActionResult delete(Guid idDoc)
        {
            if (idDoc == Guid.Empty)
                RedirectToAction("index");
            var doc =  _context.DocRegistry.FirstOrDefault(x => x.RegID == idDoc);
            if (doc == null)
                return NoContent();
            return View(doc);
        }

       
        [HttpPost]
        public async Task<IActionResult> delete(Guid idDoc, string passswordAdmin)
        {
            var doc = _context.DocRegistry.OrderBy(x => x.RegName).First(x => x.RegID == idDoc);
            _context.DocRegistry.Remove(doc);
            await _context.SaveChangesAsync();
            return RedirectToAction("index");
        }

      
        [HttpPost]
        public IActionResult Details(Guid idDoc)
        {
            
            if (idDoc != Guid.Empty)
            {
                if(!_context.DocRegistry.Any(x=>x.RegID==idDoc))
                {
                    return NoContent();
                }
                adminProcDetalis detalisList = new();
                detalisList.docReg = _context.DocRegistry.FirstOrDefault(x => x.RegID == idDoc);
                //исполнители
                detalisList.performerList = _context.PerformersDocRegistry.Where(x => x.DocRegistry_Id == idDoc).Select(x => x.Performers).ToList();

                List<Guid?> idDept = detalisList.performerList.Select(x => x.Department_ID).ToList();

                detalisList.dept = _context.Departments.Where(x => idDept.Contains(x.Id)).ToList();

                detalisList.performer = _context.ResponsibleDocRegistry.Where(x => x.DocRegistry_Id == idDoc).Select(x => x.performer).ToList();
                //документы
                detalisList.docs = _context.BufDocRegistry.Where(x => x.RegID == idDoc).Select(x => x.Docs).ToList();
                //Документы для запросов
                detalisList.zarp = _context.BufOrgsZApr.Where(x => x.RegID == idDoc).Select(x => x.ZaprDocs).ToList();
                                
                detalisList.normDoc = _context.BufNormDoc.Where(x => x.RegID == idDoc).Select(x => x.NormDoc).ToList();

                detalisList.soglasovaniyas = _context.BufSoglDoc.Where(x => x.RegID == idDoc).Select(x => x.Soglasovaniya).ToList();

                return PartialView(detalisList);
            }
            return NoContent();
        }

        [HttpPost]
        public void updateSelected(Guid idDoc)
        {
            var doc = _context.DocRegistry.OrderBy(x => x.RegName).First(x => x.RegID == idDoc);
            bool select = doc.Selected;

            if (select)
                doc.Selected = false;
            else
                doc.Selected = true;
            _context.DocRegistry.Update(doc);
            _context.SaveChanges();

        }
        [HttpPost]
        public void updatePMC(Guid idDoc)
        {
            var doc = _context.DocRegistry.OrderBy(x => x.RegName).First(x => x.RegID == idDoc);
            bool select = doc.Regulation740;

            if (select)
                doc.Regulation740 = false;
            else
                doc.Regulation740 = true;
            _context.DocRegistry.Update(doc);
            _context.SaveChanges();

        }

    }
}
