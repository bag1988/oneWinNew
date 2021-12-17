using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using oneWin.Data;
using oneWin.Models;
using oneWin.Models.baseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace oneWin.Service
{
    public class getList
    {
        private readonly oneWinDbContext _context;

        public getList(oneWinDbContext context)
        {
            _context = context;
        }
        public Task<SelectList> headsList(bool? legal = null)
        {
            return Task.FromResult(new SelectList(_context.Heads.Include(x => x.Sections).ThenInclude(x => x.DocRegs).AsEnumerable().Where(x=> legal==null|| x.Sections.Any(s => s.DocRegs.Any(x => x.IP==legal))).OrderBy(x => x.Number).Select(r => new SelectListItem
            {
                Text = r.Number + " " + r.Name + "----" + (r.Sections.Any() ? r.Sections.Any(x=>x.DocRegs.Any()) ? r.Sections.Any(x=>x.DocRegs.Any(d=>d.IP==true)) ? "для юр.лиц" : "для физ. лиц" : "неопределено" : "неопределено"),
                Value = r.HedID.ToString()
            }), "Value", "Text"));
        }
        public Task<SelectList> sectList(string idHead)
        {
            return Task.FromResult(new SelectList(_context.Sections.AsEnumerable().OrderBy(x => x.Number).Where(x => x.HeadID.ToString() == idHead).Select(r => new SelectListItem
            {
                Text =r.Number+" "+ r.Name,
                Value = r.SectionID.ToString()
            }), "Value", "Text"));
        }

        public async Task<SelectList> adminProcList(string idSection)
        {
            if (!await _context.DocRegistry.AnyAsync(x => x.SectionID.ToString() == idSection))
                return null;

            return (new SelectList(_context.DocRegistry.AsNoTracking().OrderBy(x => x.Number).Where(x => x.SectionID.ToString() == idSection).Select(r => new SelectListItem
            {
                Text =(r.Number+" "+ r.RegName),
                Value = r.RegID.ToString()
            }), "Value", "Text"));
        }


        public Task<List<SelectListItem>> isssueList()
        {
            return Task.FromResult(new List<SelectListItem>()
           {
                 new SelectListItem() {Text="в случае запроса документов и сведений", Value="1"},
                 new SelectListItem() {Text="после приемки жилого дома в эксплуатацию", Value="2"},
                 new SelectListItem() {Text="после получения последнего необходимого документа", Value="3", Selected=true},
                 new SelectListItem() {Text="со дня оплаты работ по договору подряда", Value="4"}
            });
        }

        public Task<List<SelectListItem>> TypeIssueList()
        {            
            return Task.FromResult(new List<SelectListItem>()
           {
                 new SelectListItem() {Text="Календарных дней", Value="1"},
                 new SelectListItem() {Text="Рабочих дней", Value="2"},
                 new SelectListItem() {Text="Месяц", Value="3", Selected=true},
                 new SelectListItem() {Text="Неделя", Value="4"},
                 new SelectListItem() {Text="Календарных дней со дня подачи", Value="5"},
                 new SelectListItem() {Text="Рабочих дней со дня подачи", Value="6"},
                 new SelectListItem() {Text="Месяц со дня подачи", Value="7"},
                 new SelectListItem() {Text="Неделя со дня подачи", Value="8"},
            });
        }

        public Task<SelectList> siteInssueList()
        {
            return Task.FromResult(new SelectList(_context.ViewSiteInssue.AsEnumerable().Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id.ToString()
            }), "Value", "Text"));
        }

        public Task<SelectList> costList()
        {
            return Task.FromResult(new SelectList(_context.ViewSiteCost.AsEnumerable().Select(r => new SelectListItem
            {
                Text = r.name,
                Value = r.idCost.ToString()
            }), "Value", "Text"));
        }
        public Task<SelectList> validatyList()
        {
            return Task.FromResult(new SelectList(_context.ViewSiteValidaty.AsEnumerable().Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id.ToString()
            }), "Value", "Text"));
        }

        public Task<SelectList> listingList()
        {
            return Task.FromResult(new SelectList(_context.ViewSiteSections.AsEnumerable().Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id.ToString()
            }), "Value", "Text"));
        }

        public Task<SelectList> orgZaprList()
        {
            return Task.FromResult(new SelectList(_context.OrgsZapr.AsEnumerable().OrderBy(x=>x.Name).Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.OrgZaprID.ToString()
            }), "Value", "Text"));
        }

        public async Task<orgsZaprModel> getOrgZapr(Guid idOrg)
        {
            return await _context.OrgsZapr.FirstOrDefaultAsync(x => x.OrgZaprID == idOrg);
        }

        public Task<SelectList> sogOrgList()
        {
            return Task.FromResult(new SelectList(_context.SoglOrg.AsNoTracking().Select(r => new SelectListItem
            {
                Text = r.DeptName,
                Value = r.SoglOrgID.ToString()
            }), "Value", "Text"));
        }

        public Task<SelectList> areaList()
        {
            return Task.FromResult(new SelectList(_context.Areas.AsNoTracking().Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id.ToString()
            }), "Value", "Text"));
        }

        public Task<SelectList> curatorList(string idArea=null)
        {
            if(idArea==null)
            {
                idArea = _context.Settings.First().ValueArea.ToString();
                return Task.FromResult(new SelectList(_context.Curators.AsNoTracking().Where(x => x.Areas.Number.ToString() == idArea).Select(r => new SelectListItem
                {
                    Text = r.FIO,
                    Value = r.Id.ToString()
                }), "Value", "Text"));
            }
            return Task.FromResult(new SelectList(_context.Curators.AsNoTracking().Where(x => x.Area_Id.ToString() == idArea).Select(r => new SelectListItem
            {
                Text = r.FIO,
                Value = r.Id.ToString()
            }), "Value", "Text"));
        }

        public Task<SelectList> departmentList(string idCurator)
        {
            return Task.FromResult(new SelectList(_context.Departments.AsNoTracking().Where(x => x.Curator_Id.ToString() == idCurator).Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id.ToString()
            }), "Value", "Text"));
        }

        public Task<SelectList> performertList(string idDepartment)
        {
            return Task.FromResult(new SelectList(_context.Performers.AsNoTracking().Where(x => x.Department_ID.ToString() == idDepartment).Select(r => new SelectListItem
            {
                Text = r.FIO,
                Value = r.Id.ToString()
            }), "Value", "Text"));
        }

        public Task<SelectList> departmentList()
        {
            return Task.FromResult(new SelectList(_context.Departments.AsNoTracking().OrderBy(x=>x.Name).Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id.ToString()
            }), "Value", "Text"));
        }


        public IQueryable<docRegModel> docForDept (string idDepartment)
        {
            return _context.PerformersDocRegistry.AsNoTracking().Where(x => x.Performers.Department_ID.ToString() == idDepartment).Select(x => x.DocRegistry).
                Union(_context.DocumentAccept.AsNoTracking().Where(x => x.Performers.Department_ID.ToString() == idDepartment).Select(x => x.DocRegistry));
        }

        public Task<SelectList> docForDepartmentList(string idDepartment)
        { 
            return Task.FromResult(new SelectList(docForDept(idDepartment).
                Select(r => new SelectListItem
            {
                Text =r.Number+" "+ r.RegName,
                Value = r.RegID.ToString()
            }).OrderBy(x=>x.Text), "Value", "Text"));
        }

        public Task<SelectList> performerForDoc(Guid idDoc, object selectValue=null)
        {
            return Task.FromResult(new SelectList(_context.PerformersDocRegistry.AsNoTracking().Where(x=>x.DocRegistry_Id==idDoc).Select(r => new SelectListItem
            {
                Text = r.Performers.FIO,
                Value = r.Performers_Id.ToString()
            }), "Value", "Text", selectValue==null?"":selectValue.ToString()));
        }

        public async Task<SelectList> getZaprForTransfer(Guid regId)
        {
            var reg = await _context.Registration.FindAsync(regId);

            var zaprDoc = await getZaprForProc((Guid)reg.RegID);

            return (new SelectList(zaprDoc.Select(r => new SelectListItem
            {
                Text = r.Value,
                Value = r.Key.ToString()
            }), "Value", "Text"));
        }

        public async Task<Dictionary<Guid, string>> getZaprForProc(Guid RegId)
        {
            if (!await _context.BufOrgsZApr.AnyAsync(x => x.RegID == RegId))
                return new Dictionary<Guid, string>();
            return await _context.BufOrgsZApr.Include(x => x.ZaprDocs).AsNoTracking().Where(x => x.RegID == RegId).ToDictionaryAsync(x => x.ZaprDocID, x => x.ZaprDocs.Name);
        }

        public async Task<Dictionary<Guid?, string>> getDocForProc(Guid RegId)
        {
            if (!await _context.BufDocRegistry.AnyAsync(x => x.RegID == RegId))
                return new Dictionary<Guid?, string>();

            return await _context.BufDocRegistry.Include(x => x.Docs).AsNoTracking().Where(x => x.RegID == RegId).ToDictionaryAsync(x => (Guid?)x.DocID, x => x.Docs.Name);

        }

        public async Task<List<attachFile>> getAttachFile(Guid RegId)
        {
            if (!await _context.AttachedFile.AnyAsync(x => x.RegistrationId == RegId))
                return new List<attachFile>();
            return await _context.AttachedFile.AsNoTracking().Where(x => x.RegistrationId == RegId).Select(x => new attachFile { id = x.Id, urlFile = x.Url, nameFile = x.Name }).ToListAsync();

        }

        public async Task<List<familyModel>> loadFamily(Guid RegId)
        {
            if (!await _context.Family.AnyAsync(x => x.RegistrationID == RegId))
                return new List<familyModel>();
            return await _context.Family.AsNoTracking().Where(x => x.RegistrationID == RegId).ToListAsync();
        }

        public async Task<List<msgModel>> loadTransfer(Guid RegId)
        {
            if (!await _context.MSG.AnyAsync(x => x.RegistrationId == RegId))
                return new List<msgModel>();
            return await _context.MSG.Include(x=>x.zaprDoc).AsNoTracking().Where(x => x.RegistrationId == RegId).ToListAsync();
        }

        public Task<SelectList> getMfos()
        {
            return Task.FromResult(new SelectList(_context.MFOS.AsEnumerable().Select(r => new SelectListItem
            {
                Text = r.MFO,
                Value = r.MFO
            }), "Value", "Text"));
        }

    }
}
