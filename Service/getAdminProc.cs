using Microsoft.AspNetCore.Http;
using oneWin.Data;
using oneWin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace oneWin.Service
{
    public class getAdminProc
    {
        private readonly oneWinDbContext _context;

        public getAdminProc(oneWinDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// список процедур
        /// </summary>
        /// <param name="legalStr">физ(false), юр(true) лица</param>
        /// <param name="performerCount">true - только процедуры имеющие исполнителя </param>
        /// <returns></returns>
        public List<adminProcModel> getList(string legalStr="false", bool performerCount = false)
        {
            bool legal = legalStr == "true" ? true : false;

            List<adminProcModel> head = new();
            HttpContext Current = new HttpContextAccessor().HttpContext;
            var User = Current.User;
            //string idUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
           
                head = _context.Heads.Select(x => new adminProcModel
                {
                    head = x,
                    section = x.Sections.Where(s => s.HeadID == x.HedID).Select(s => new sectionAdminProc
                    {
                        section = s,
                        docReg = s.DocRegs.Where(d => d.SectionID == s.SectionID && d.IP == legal && (!performerCount || _context.PerformersDocRegistry.AsNoTracking().Any(x => x.DocRegistry_Id == d.RegID))).OrderBy(x => x.Num).ToList()
                    }).Where(x => x.docReg.Count > 0).OrderBy(x => x.section.Number).ToList()
                }).Where(x => x.section.Count > 0).OrderBy(x => x.head.Number).ToList();
            
            //else if (User.IsInRole("ispolnitel3") || User.IsInRole("ispolnitel2") || User.IsInRole("ispolnitel4"))
            //{
                
            //    //список процедур доступных для пользователя
            //    List<Guid> idDoc = _context.PerformersDocRegistry.Where(x => x.Performers_Id.ToString() == idUser).Select(x => x.DocRegistry_Id).ToList();
            //    head = _context.Heads.Select(x => new adminProcModel
            //    {
            //        head = x,
            //        section = x.Sections.Where(s => s.HeadID == x.HedID).Select(s => new sectionAdminProc
            //        {
            //            section = s,
            //            docReg = s.DocRegs.Where(d => d.SectionID == s.SectionID && d.IP == legal && idDoc.Contains(d.RegID)).OrderBy(x => x.Num).ToList()
            //        }).Where(x => x.docReg.Count > 0).OrderBy(x => x.section.Number).ToList()
            //    }).Where(x => x.section.Count > 0).OrderBy(x => x.head.Number).ToList();
            //}
            return head;
        }
    }
}
