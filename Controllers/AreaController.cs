using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using oneWin.Data;
using oneWin.Models;
using oneWin.Models.baseModel;
using oneWin.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;

namespace oneWin.Controllers
{
    public class AreaController : Controller
    {
        private oneWinDbContext _context;
        private getAdminProc _proc;
        public AreaController(oneWinDbContext context, getAdminProc proc)
        {
            _context = context;
            _proc = proc;
        }

        [HttpPost]
        public IActionResult CuratorList(Guid idArea)
        {
            ViewBag.NameArea = _context.Areas.OrderBy(x => x.Name).First(x => x.Id == idArea).Name;
            return PartialView(_context.Curators.Where(x=>x.Area_Id==idArea).ToList());
        }

        [HttpPost]
        public IActionResult PerformerList(Guid idDepartment)
        {
            ViewBag.idCurator = _context.Departments.OrderBy(x => x.Name).First(x => x.Id == idDepartment).Curator_Id;
            ViewBag.NameDepartment = _context.Departments.OrderBy(x => x.Name).First(x => x.Id == idDepartment).Name;
            ViewBag.idDepartment = _context.Departments.OrderBy(x => x.Name).First(x => x.Id == idDepartment).Id;
            return PartialView(_context.Performers.Where(x => x.Department_ID == idDepartment).ToList());
        }

        [HttpPost]
        public IActionResult DepartmentList(Guid idCurator)
        {
            ViewBag.NameCurator = _context.Curators.OrderBy(x => x.FIO).First(x => x.Id == idCurator).FIO;
            ViewBag.idCurator = _context.Curators.OrderBy(x => x.FIO).First(x => x.Id == idCurator).Id;
            return PartialView(_context.Departments.Where(x => x.Curator_Id == idCurator).ToList());
        }

        public IActionResult Index()
        {            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> viewPartial(Guid idDoc)
        {
            ViewBag.idPerformer = idDoc;            
            return PartialView(await getAPFP(idDoc));
        }


        private async Task<adminProcForPerformer> getAPFP(Guid idPerformer)
        {
            adminProcForPerformer proc = new();
            if (await _context.ResponsibleDocRegistry.AnyAsync(x => x.Performers_Id == idPerformer))
                proc.DocsResponsible = await _context.ResponsibleDocRegistry.Include(x => x.DocRegistry).AsNoTracking().Where(x => x.Performers_Id == idPerformer).Select(x => x.DocRegistry).ToListAsync();
            if (await _context.PerformersDocRegistry.AnyAsync(x => x.Performers_Id == idPerformer))
                proc.DocsReg = await _context.PerformersDocRegistry.Include(x => x.DocRegistry).AsNoTracking().Where(x => x.Performers_Id == idPerformer).Select(x => x.DocRegistry).ToListAsync();
            if (await _context.DocumentAccept.AnyAsync(x => x.Performers_Id == idPerformer))
                proc.DocumentsAccept = await _context.DocumentAccept.Include(x => x.DocRegistry).AsNoTracking().Where(x => x.Performers_Id == idPerformer).Select(x => x.DocRegistry).ToListAsync();

            return proc;
        }

        [HttpPost]
        public async Task<IActionResult> viewAdminProc(Guid idPerformer, string legal)
        {
            viewAdminProcForPerformer procList = new();
            procList.adminProcList = _proc.getList(legal);
            var getA = await getAPFP(idPerformer);
            if (getA != null)
            {
                if (getA.DocsReg != null)
                    procList.docReg = getA.DocsReg.Select(x => x.RegID).ToList();
                if (getA.DocsResponsible != null)
                    procList.docRes = getA.DocsResponsible.Select(x => x.RegID).ToList();
                if (getA.DocumentsAccept != null)
                    procList.docAcc = getA.DocumentsAccept.Select(x => x.RegID).ToList();
            }
            return PartialView(procList);
        }

       
        [HttpPost]
        public async Task<IActionResult> deleteArea(Guid idArea)
        {
            var doc = _context.Areas.OrderBy(x => x.Name).First(x => x.Id == idArea);
            _context.Areas.Remove(doc);
            await _context.SaveChangesAsync();
            return Ok();
        }

       
        [HttpPost]
        public async Task<IActionResult> deleteCurator(Guid idCurator)
        {
            var doc = _context.Curators.OrderBy(x => x.FIO).First(x => x.Id == idCurator);
            _context.Curators.Remove(doc);
            await _context.SaveChangesAsync();
            return Ok();
        }

        
        [HttpPost]
        public async Task<IActionResult> deleteDepartment(Guid idDepartment)
        {
            var doc = _context.Departments.OrderBy(x => x.Name).First(x => x.Id == idDepartment);
            _context.Departments.Remove(doc);
            await _context.SaveChangesAsync();
            return Ok();
        }

       
        [HttpPost]
        public async Task<IActionResult> deletePerformer(Guid idPerformer)
        {
            var doc = _context.Performers.OrderBy(x => x.FIO).First(x => x.Id == idPerformer);
            _context.Performers.Remove(doc);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> addAdminProc(Guid idPerformer, List<Guid?> idDocReg, List<Guid?> idDocRes, List<Guid?> idDocAcc, bool legal=false)
        {
            //исполниель
            if (idDocReg != null)
            {
                var docsList = _context.PerformersDocRegistry.Where(x => x.Performers_Id == idPerformer&&x.DocRegistry.IP==legal).ToList();

                var deleteDoc = docsList.Where(x => !idDocReg.Contains(x.DocRegistry_Id)).ToList();
                if (deleteDoc.Count > 0)
                {
                    _context.PerformersDocRegistry.RemoveRange(deleteDoc);
                }
                var docs = docsList.Select(x => x.DocRegistry_Id).ToList();
                foreach (Guid g in idDocReg)
                {
                    if (!docs.Contains(g))
                    {
                        await _context.PerformersDocRegistry.AddAsync(new performersDocRegistryModel { DocRegistry_Id = g, Performers_Id = idPerformer });
                    }
                }
            }

            //ответственный
            if (idDocRes != null)
            {
                var docsListRes = _context.ResponsibleDocRegistry.Where(x => x.Performers_Id == idPerformer && x.DocRegistry.IP == legal).ToList();

                var deleteDocRes = docsListRes.Where(x => !idDocRes.Contains(x.DocRegistry_Id)).ToList();
                if (deleteDocRes.Count > 0)
                {
                    _context.ResponsibleDocRegistry.RemoveRange(deleteDocRes);
                }
                var docsRes = docsListRes.Select(x => x.DocRegistry_Id).ToList();
                foreach (Guid g in idDocRes)
                {
                    if (!docsRes.Contains(g))
                    {
                        await _context.ResponsibleDocRegistry.AddAsync(new responsibleDocRegistryModel { DocRegistry_Id = g, Performers_Id = idPerformer });
                    }
                }
            }

            //прием документов
            if (idDocAcc != null)
            {
                var docsListAcc = _context.DocumentAccept.Where(x => x.Performers_Id == idPerformer && x.DocRegistry.IP == legal).ToList();

                var deleteDocAcc = docsListAcc.Where(x => !idDocAcc.Contains(x.DocRegistry_Id)).ToList();
                if (deleteDocAcc.Count > 0)
                {
                    _context.DocumentAccept.RemoveRange(deleteDocAcc);
                }
                var docsAcc = docsListAcc.Select(x => x.DocRegistry_Id).ToList();
                foreach (Guid g in idDocAcc)
                {
                    if (!docsAcc.Contains(g))
                    {
                        await _context.DocumentAccept.AddAsync(new documentAcceptModel { DocRegistry_Id = g, Performers_Id = idPerformer });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

       
        public IActionResult CreateArea(Guid idArea)
        {
            if (idArea != Guid.Empty)
                return View(_context.Areas.OrderBy(x => x.Name).First(x => x.Id == idArea));
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArea(areaModel doc)
        {            
            if (ModelState.IsValid)
            {
                if (doc.Id == Guid.Empty)
                {
                    doc.Id = Guid.NewGuid();
                    await _context.Areas.AddAsync(doc);
                }
                else
                {
                    _context.Areas.Update(doc);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("index");
            }
            return View(doc);
        }

       
        public IActionResult CreateCurator(Guid idCurator)
        {
            if (idCurator != Guid.Empty)
                return View(_context.Curators.OrderBy(x => x.FIO).First(x => x.Id == idCurator));
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCurator(curatorModel doc)
        {
            if (ModelState.IsValid)
            {
                if (doc.Id == Guid.Empty)
                {
                    doc.Id = Guid.NewGuid();
                    await _context.Curators.AddAsync(doc);
                }
                else
                {
                    _context.Curators.Update(doc);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("index");
            }
            return View(doc);
        }

      
        public IActionResult CreateDepartment(Guid idDepartment)
        {
            if (idDepartment != Guid.Empty)
            {
                departmentModel dept = _context.Departments.OrderBy(x => x.Name).First(x => x.Id == idDepartment);
                dept.Curators = _context.Curators.OrderBy(x => x.FIO).First(x => x.Id == dept.Curator_Id);
                return View(dept);
            }
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDepartment(departmentModel doc)
        {
            ModelState.Remove("Curators.FIO");
            if (ModelState.IsValid)
            {
                doc.Curators = null;
                if (doc.Id == Guid.Empty)
                {
                    doc.Id = Guid.NewGuid();
                    await _context.Departments.AddAsync(doc);
                }
                else
                {
                    _context.Departments.Update(doc);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("index");
            }
            return View(doc);
        }

       
        public IActionResult CreatePerformer(Guid idPerformer)
        {
            if (idPerformer != Guid.Empty)
            {
                performerModel dept = _context.Performers.OrderBy(x => x.FIO).First(x => x.Id == idPerformer);
                dept.Department = _context.Departments.OrderBy(x => x.Name).First(x => x.Id == dept.Department_ID);
                dept.Department.Curators = _context.Curators.OrderBy(x => x.FIO).First(x => x.Id == dept.Department.Curator_Id);
                return View(dept);
            }
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePerformer(performerModel doc)
        {
            ModelState.Remove("Department.Name");
            ModelState.Remove("Department.Curators.FIO");
            if (ModelState.IsValid)
            {
                doc.Department = null;
                if (doc.Id == Guid.Empty)
                {
                    doc.Id = Guid.NewGuid();
                    doc.CheckPerAddress = true;
                    doc.CheckPerCabinet = true;
                    doc.CheckPermail = true;
                    doc.CheckPerName = true;
                    doc.CheckPerNum = true;
                    doc.CheckPerPhone = true;
                    doc.CheckPerTitle = true;
                    doc.ChekPerNotes = true;
                    await _context.Performers.AddAsync(doc);
                }
                else
                {
                    _context.Performers.Update(doc);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("index");
            }
            return View(doc);
        }


        
        public IActionResult CreateComment(int idDoc)
        {
            if (idDoc != 0)
            {
                siteDocRegComentModel dept = _context.ViewSiteDocRegComent.OrderBy(x => x.Id).First(x => x.Id == idDoc);
                dept.DocsReg = _context.DocRegistry.OrderBy(x => x.RegName).First(x => x.RegID == dept.IdDoc);
                return View(dept);
            }
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment(siteDocRegComentModel doc)
        {
            if (doc.Id == 0)
                ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                doc.DocsReg = null;
                if (doc.Id == 0)
                {
                    await _context.ViewSiteDocRegComent.AddAsync(doc);
                }
                else
                {
                    _context.ViewSiteDocRegComent.Update(doc);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("index");
            }
            return View(doc);
        }

        
        public async Task<IActionResult> deleteComment(int idComment)
        {
            var doc = _context.ViewSiteDocRegComent.OrderBy(x => x.Id).First(x => x.Id == idComment);
            _context.ViewSiteDocRegComent.Remove(doc);
            await _context.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpPost]
        public async Task<JsonResult> getCuratorList(string idArea)
        {
            getList g = new getList(_context);
            var d = await g.curatorList(idArea);
            return Json(d);
        }

        [HttpPost]
        public async Task<JsonResult> getDepartmentList(string idCurator)
        {
            getList g = new getList(_context);
            var d = await g.departmentList(idCurator);
            return Json(d);
        }

        [HttpPost]
        public async Task<JsonResult> getPerformerList(string idDepartment)
        {
            getList g = new getList(_context);
            var d = await g.performertList(idDepartment);
            return Json(d);
        }


        [HttpPost]
        public async Task<JsonResult> getDocForDeptList(string idDepartment)
        {
            getList g = new getList(_context);
            var d = await g.docForDepartmentList(idDepartment);
            return Json(d);
        }

        [HttpPost]
        public IActionResult getDocForComment(Guid idDoc, Guid IdDep)
        {
            if (_context.ViewSiteDocRegComent.AsNoTracking().FirstOrDefault(x => x.IdDoc == idDoc && x.IdDep == IdDep)!=null)
                return Json(_context.ViewSiteDocRegComent.AsNoTracking().OrderBy(x => x.Id).First(x => x.IdDoc == idDoc && x.IdDep == IdDep));
            return BadRequest();
        }
    }
}
