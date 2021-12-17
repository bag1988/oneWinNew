using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using oneWin.Data;
using oneWin.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using DocumentFormat.OpenXml.Vml.Office;
using oneWin.OfficeCreate;
using oneWin.Models.baseModel;

namespace oneWin.Controllers
{
    public class ReportsController : Controller
    {
        private oneWinDbContext _context;

        public ReportsController(oneWinDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Отчет по количеству принятых заявлений
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="RegType"></param>
        /// <param name="radioOfDocType"></param>
        /// <returns></returns>
        public async Task<IActionResult> report(DateTime startDate, DateTime endDate, int RegType = 0, int radioOfDocType = 0)
        {
            string url = new getUrlFile().urlFileGeneral();
            string urlNew = new getUrlFile().urlFile(); 
            if (radioOfDocType == 0)
            {
                if (!System.IO.File.Exists(url + "Template/Отчет.docx"))
                    return BadRequest();
                urlNew += "Reports/" + DateTime.Now.ToString("yyyy_dd_MM_HH_mm_ss") + "_" + DateTime.Now.Ticks.ToString() + "_Отчет.docx";
                System.IO.File.Copy(url + "Template/Отчет.docx", urlNew);
            }
            if (radioOfDocType == 1)
            {               
                urlNew += "Reports/" + DateTime.Now.ToString("yyyy_dd_MM_HH_mm_ss") + "_" + DateTime.Now.Ticks.ToString() + "_Отчет.xlsx";
            }

            if (urlNew == "")
            {
                return BadRequest();
            }
            Dictionary<string, List<string>> tab = new Dictionary<string, List<string>>();
            string CountList = "";

            string regT = RegType.ToString();
            if (RegType == 9)
            {
                RegType = 0;
            }

            int ElPost = await _context.MSG.CountAsync(x => x.Registration.Deleted != true && x.Registration.GettingDate.Value.Date >= startDate && x.Registration.GettingDate.Value.Date <= endDate && x.zaprDoc.e_mailzapr == true && (RegType == 0 ? x.Registration.TypeReg != null : x.Registration.TypeReg == RegType));
            int WrPost =await _context.MSG.CountAsync(x => x.Registration.Deleted != true && x.Registration.GettingDate.Value.Date >= startDate && x.Registration.GettingDate.Value.Date <= endDate && x.zaprDoc.postzapr == true && (RegType == 0 ? x.Registration.TypeReg != null : x.Registration.TypeReg == RegType));
            int Bank =await _context.MSG.CountAsync(x => x.Registration.Deleted != true && x.Registration.GettingDate.Value.Date >= startDate && x.Registration.GettingDate.Value.Date <= endDate && x.zaprDoc.bankzapr == true && (RegType == 0 ? x.Registration.TypeReg != null : x.Registration.TypeReg == RegType));
            int BRTI =await _context.MSG.CountAsync(x => x.Registration.Deleted != true && x.Registration.GettingDate.Value.Date >= startDate && x.Registration.GettingDate.Value.Date <= endDate && x.zaprDoc.HTTPZapr == true && (RegType == 0 ? x.Registration.TypeReg != null : x.Registration.TypeReg == RegType));

            //int countKomplat = _context.CopLic.Count(x => x.Registration.GettingDate.Value.Date >= startDate && x.Registration.GettingDate.Value.Date <= endDate && (RegType == 0 ? x.Registration.TypeReg != null : x.Registration.TypeReg == RegType));
            int GettingEl = _context.Registration.Count(x => x.StatementForm.Contains("Через интернет") && x.GettingDate.Value.Date >= startDate && x.GettingDate.Value.Date <= endDate && (RegType == 0 ? x.TypeReg != null : x.TypeReg == RegType));

            string notification = _context.Registration.Count(p => p.NotificationDate != null && p.NotificationDate.Value.Date >= startDate &&
                                                                          p.NotificationDate.Value.Date <= endDate && (RegType == 0 ? p.TypeReg != null : p.TypeReg == RegType) &&
                                                                          p.Deleted != true).ToString();


            if (radioOfDocType == 1)
            {
                tab.Add("NoBorder" + (tab.Count + 1).ToString(), new List<string> { _context.Settings.OrderBy(x=>x.ID).FirstOrDefault().Name});
                tab.Add("emptyRow" + (tab.Count + 1).ToString(), new List<string> { "", "", "" });
                tab.Add("NoBorder" + (tab.Count + 1).ToString(), new List<string> { dictionaryList.typeReports[regT] });
                tab.Add("NoBorder" + (tab.Count + 1).ToString(), new List<string> { "с " + startDate.ToString("d") + " по " + endDate.ToString("d") });
                tab.Add("emptyRow" + (tab.Count + 1).ToString(), new List<string> { "", "", "" });
                tab.Add("head"+(tab.Count + 1).ToString(), new List<string> { "№ п/п", "Наименование административной процедуры", "Количество зарегистрированных заявлений" });
            }

            if (regT == "9")
            {
                var doc =await _context.Registration.AsNoTracking().Where(x => x.GettingDate.Value.Date >= startDate && x.GettingDate.Value.Date <= endDate && x.Deleted != true && x.TypeReg != 3 && x.DocRegistry.Heads.Name != null)
                .GroupBy(x => new
                {
                    x.DocRegistry.Heads.Number,
                    x.DocRegistry.Heads.Name,
                    sectionNumber = x.DocRegistry.Sections.Number,
                    sectionName = x.DocRegistry.Sections.Name,
                    docNumber = x.DocRegistry.Number,
                    docName = x.DocRegistry.RegName,
                    x.DocRegistry.IP,
                    x.RegID,
                    x.DocRegistry.HeadsID,
                    x.DocRegistry.SectionID
                })
                .Select(x => new
                {
                    x.Key.Number,
                    x.Key.Name,
                    x.Key.sectionNumber,
                    x.Key.sectionName,
                    x.Key.docNumber,
                    x.Key.docName,
                    x.Key.IP,
                    x.Key.RegID,
                    x.Key.HeadsID,
                    count = x.Count(),
                    x.Key.SectionID,
                }).OrderBy(x => x.Number).ThenBy(x => x.sectionNumber).ThenBy(x => x.sectionName).ThenBy(x => x.docNumber).ThenBy(x => x.docName).ToListAsync();

                CountList = doc.Sum(x => x.count).ToString();

                foreach (var docIP in doc.Select(x => x.IP).Distinct().ToList())
                {
                    string headName = "";
                    string sectionName = "";
                    if (doc.Any(x => x.IP == docIP))
                    {
                        if (!docIP)
                            tab.Add((tab.Count + 1).ToString(), new List<string> { "", "ФИЗ. ЛИЦА", "" });
                        else
                            tab.Add((tab.Count + 1).ToString(), new List<string> { "", "Юр. ЛИЦА", "" });
                    }

                    foreach (var d in doc.Where(x => x.IP == docIP).ToList())
                    {
                        if (d.Name == headName)
                        {
                            if (d.sectionName == sectionName)
                            {
                                tab.Add((tab.Count + 1).ToString(), new List<string> { "Подраздел " + d.docNumber, d.docName, d.count.ToString() });
                            }
                            else
                            {
                                sectionName = d.sectionName;
                                tab.Add("head" + (tab.Count + 1).ToString(), new List<string> { "Раздел " + d.sectionNumber, d.sectionName, doc.Where(x => x.SectionID == d.SectionID && x.HeadsID == d.HeadsID).Sum(x => x.count).ToString() });
                                tab.Add((tab.Count + 1).ToString(), new List<string> { "Подраздел " + d.docNumber, d.docName, d.count.ToString() });
                            }
                        }
                        else
                        {
                            headName = d.Name;
                            tab.Add("head" + (tab.Count + 1).ToString(), new List<string> { "Глава " + d.Number, d.Name, doc.Where(x => x.HeadsID == d.HeadsID).Sum(x => x.count).ToString() });
                            sectionName = d.sectionName;
                            tab.Add("head" + (tab.Count + 1).ToString(), new List<string> { "Раздел " + d.sectionNumber, d.sectionName, doc.Where(x => x.SectionID == d.SectionID && x.HeadsID == d.HeadsID).Sum(x => x.count).ToString() });
                            tab.Add((tab.Count + 1).ToString(), new List<string> { "Подраздел " + d.docNumber, d.docName, d.count.ToString() });
                        }
                    }
                }
            }
            else
            {
                var list =await _context.Registration.Include(x=>x.DocRegistry).AsNoTracking()
                .Where(x => x.DocRegistry.Selected == true && x.GettingDate.Value.Date >= startDate && x.GettingDate.Value.Date <= endDate && (RegType == 0 ? x.TypeReg != null : x.TypeReg == RegType) && x.DocNo != null && x.Deleted != true)
                .GroupBy(x => new { x.DocRegistry.Number, x.DocRegistry.RegName })
                .Select(x => new { regName = (x.Key.Number + " " + x.Key.RegName), count = x.Count() }).OrderBy(x => x.regName).ToListAsync();

                CountList = list.Sum(x => x.count).ToString();

                foreach (var d in list)
                {
                    tab.Add((tab.Count + 1).ToString(), new List<string> { (tab.Count + 1).ToString(), d.regName, d.count.ToString() });

                }

            }
            if (radioOfDocType == 1)
            {
                tab.Add("emptyRow" + (tab.Count + 1).ToString(), new List<string> { "", "", "" });
                tab.Add("startTwo" + (tab.Count + 1).ToString(), new List<string> { "Всего принято заявлений граждан", "", CountList });
                tab.Add("startTwo" + (tab.Count + 1).ToString(), new List<string> { "Направлено запросов в АС «ЖИЛПЛАТ»", "", "0" });
                tab.Add("startTwo" + (tab.Count + 1).ToString(), new List<string> { "Направлено запросов в МГА ГРиЗК", "", BRTI.ToString() });
                tab.Add("startTwo" + (tab.Count + 1).ToString(), new List<string> { "Направлено запросов в ОАО «АСБ Беларусбанк»", "", Bank.ToString() });
                tab.Add("startTwo" + (tab.Count + 1).ToString(), new List<string> { "Направлено запросов по электронной почте", "", ElPost.ToString() });
                tab.Add("startTwo" + (tab.Count + 1).ToString(), new List<string> { "Зарегистрировано письменных запросов в иные учреждения, организации", "", WrPost.ToString() });
                tab.Add("startTwo" + (tab.Count + 1).ToString(), new List<string> { "Обратилось через систему электронного управления очередью", "", "" });
                tab.Add("startTwo" + (tab.Count + 1).ToString(), new List<string> { "Обращение, поступившее по электронной почте", "", GettingEl.ToString() });
                tab.Add("startTwo" + (tab.Count + 1).ToString(), new List<string> { "Направлено уведомлений ", "", notification });
                tab.Add("startTwo" + (tab.Count + 1).ToString(), new List<string> { "Начальник отдела", "", User.Identity.Name });
                tab.Add("startTwo" + (tab.Count + 1).ToString(), new List<string> { "", "", DateTime.Now.ToString("d") });


                //Ширина столбцов
                tab.Add("width", new List<string> { "15", "70", "14" });
            }


            Dictionary<string, string> findesReplaces = new Dictionary<string, string>() {
                { "Area", _context.Settings.OrderBy(x=>x.ID).FirstOrDefault().Name },
                { "Naim", dictionaryList.typeReports[regT] },
                { "StartDate", startDate.ToString("d")},
                { "EndDate", endDate.ToString("d") },
                { "Itogo", CountList},
                //{ "Zp_KOMPLAT", countKomplat.ToString()},
                { "Zp_BRTI", BRTI.ToString()},
                { "Zp_ASB",  Bank.ToString()},
                { "Zp_e_mail", ElPost.ToString()},
                { "Zp_pochta", WrPost.ToString()},
                { "Za_internet", GettingEl.ToString()},
                { "Worker", User.Identity.Name},
                { "dtpCurrDate", DateTime.Now.Day.ToString()},
                { "dtpMonth", DateTime.Now.Month.ToString()},
                { "dtpYear", DateTime.Now.Year.ToString()},
                { "notification_count", notification}
            };

            createDocument createDoc = new createDocument();
            if (radioOfDocType == 0)
                urlNew = createDoc.createWord(urlNew, findesReplaces, tab);
            if (radioOfDocType == 1)
                urlNew = createDoc.createExcel(urlNew, tab, true);

            return Json(urlNew);
        }

        /// <summary>
        /// Реестр для детского сада
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<IActionResult> reportGarden(DateTime startDate, DateTime endDate)
        {
            string url = new getUrlFile().urlFileGeneral();
            string urlNew = new getUrlFile().urlFile();

            if (!System.IO.File.Exists(url + "Template/Отчет66.docx"))
                return BadRequest();
            urlNew += "Reports/" + DateTime.Now.ToString("yyyy_dd_MM_HH_mm_ss") + "_" + DateTime.Now.Ticks.ToString() + "_Отчет66.docx";
            System.IO.File.Copy((url + "Template/Отчет66.docx"), urlNew);


            if (urlNew == "")
                return BadRequest();
            Dictionary<string, List<string>> tab = new Dictionary<string, List<string>>();

            var list = await _context.Registration.Include(x=>x.DocRegistry).Include(p => p.Family).AsNoTracking().Where(p => (p.DocRegistry.RegName == "Постановка на учет ребенка, нуждающегося в определении в учреждение образования для получения дошкольного образования" || p.DocRegistry.Number == "6.6") && p.Deleted != true && p.GettingDate.Value.Date >= startDate && p.GettingDate.Value.Date <= endDate && p.DocNo != null).OrderBy(p => p.GettingDate).ToListAsync();
            if (list.Any())
            {
                string TypeRequestName = "";
                switch (list.OrderBy(x=>x.RegName).First().TypeReg)
                {
                    case 1: TypeRequestName = "Физических лиц"; break;
                    case 2: TypeRequestName = "Юридических лиц"; break;
                    case 3: TypeRequestName = "Административных жалоб"; break;
                    case 4: TypeRequestName = "Внутренних потребностей"; break;
                }



                foreach (var c in list)
                {
                    tab.Add((tab.Count + 1).ToString(), new List<string> {
                        c.DocNo.ToString(),
                        c.GettingDate.ToString(),
                        (c.LName + " " + c.FName + " " + c.MName),
                    (c.City + " " + c.Address + " " + c.Home + " " + c.Flat),
                    c.PhoneNo,
                    c.MobPhone,
                    c.Registrator,
                    c.Notes,
                    });
                    foreach (var f in c.Family)
                    {
                        tab.Add((tab.Count + 1).ToString(), new List<string> { "",f.NRotN, f.LName,
                    f.FName,
                    f.MName,
                    f.DOB!= null ? f.DOB.Value.ToString("d") : "",
                    "",
                    ""
                    });
                    }
                }



                Dictionary<string, string> findesReplaces = new Dictionary<string, string>() {
                { "Naim",_context.Settings.FirstOrDefault().Name },
                { "StartDate", startDate.ToString("d")},
                { "EndDate", endDate.ToString("d") },
                 { "ZA_L", TypeRequestName},
                { "Itogo_ZA", list.Count().ToString()}
            };

                createDocument createDoc = new createDocument();

                urlNew = createDoc.createWord(urlNew, findesReplaces, tab);
            }

            return Json(urlNew);
        }

        /// <summary>
        /// Отчет по 110
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<IActionResult> reportHandred(DateTime startDate, DateTime endDate)
        {
            string urlNew = new getUrlFile().urlFile();


            urlNew += "Reports/" + DateTime.Now.ToString("yyyy_dd_MM_HH_mm_ss") + "_" + DateTime.Now.Ticks.ToString() + "110.xlsx";


            var list = await _context.Solutions.Include(x => x.Registration).ThenInclude(x => x.DocRegistry).ThenInclude(x=>x.Sections).AsNoTracking().Where(x => x.Registration.GettingDate.Value.Date >= startDate && x.Registration.GettingDate.Value.Date <= endDate && x.Registration.Deleted != true
                           && x.Registration.DocRegistry.Heads.Number == 110
                           && x.solution == "Положительно").Select(x=>x.Registration).OrderBy(x => x.DocRegistry.Sections.Number).ThenBy(x=>x.DocRegistry.Num).ThenBy(x=>x.GettingDate).ToListAsync();

            var sectNum = list.Select(x => x.DocRegistry.Number).Distinct().ToList();

            Dictionary<string, List<string>> tab = new Dictionary<string, List<string>>();
            tab.Add("NoBorder" + (tab.Count + 1).ToString(), new List<string> {
                _context.Settings.OrderBy(x=>x.ID).FirstOrDefault().Name+ " отчёт о приеме уведомлений о видах экономической деятельности за период с "+startDate.ToString("d")+" по "+endDate.ToString("d")
            });

            foreach (var s in sectNum)
            {
                //пустая строка как разделитель
                tab.Add("emptyRow"+(tab.Count + 1).ToString(), new List<string> {"","",""});

                tab.Add("head" + (tab.Count + 1).ToString(), new List<string> {
                list.Select(x=>new {mame = x.DocRegistry.Number+" "+x.DocRegistry.Sections.Name +" "+x.DocRegistry.RegName, number = x.DocRegistry.Number }).FirstOrDefault(x=>x.number==s).mame
                });

                tab.Add("head" + (tab.Count + 1).ToString(), new List<string> {
                "Организация"
                ,"Дата регистрации"
                ,"Дата направления уведомления по почте"
                });

                foreach (var p in list.Where(x => x.DocRegistry.Number == s).ToList())
                {
                    tab.Add((tab.Count + 1).ToString(), new List<string> {
                        p.OrgName
                        ,p.GettingDate.Value.ToString("d")
                        ,p.Notes
                        });
                }

                tab.Add((tab.Count + 1).ToString(), new List<string> {
                "Итого: "+list.Count(x => x.DocRegistry.Number == s)
                ,""
                ,""
                });

            }

            tab.Add("emptyRow" + (tab.Count + 1).ToString(), new List<string> { "", "", "" });

            tab.Add((tab.Count + 1).ToString(), new List<string> {
                DateTime.Now.ToString("d")
                ,""
                ,""
                });

            //Ширина столбцов
            tab.Add("width", new List<string> {"35","35","35"});

            createDocument createDoc = new createDocument();

            urlNew = createDoc.createExcel(urlNew, tab, true);


            return Json(urlNew);
        }

        /// <summary>
        /// Отчет по количеству принятых решений
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="TypeOfReportLists"></param>
        /// <param name="TypeOfReportListSolutions"></param>
        /// <returns></returns>
        public async Task<IActionResult> reportCountSolution(DateTime startDate, DateTime endDate, int TypeOfReportLists, string TypeOfReportListSolutions)
        {
            string url = new getUrlFile().urlFileGeneral();
            string urlNew = new getUrlFile().urlFile();
           
                if (!System.IO.File.Exists(url + "Template/Отчет740.docx"))
                    return BadRequest();
                urlNew += "Reports/" + DateTime.Now.ToString("yyyy_dd_MM_HH_mm_ss") + "_" + DateTime.Now.Ticks.ToString() + "_Отчет740.docx";
                System.IO.File.Copy(url + "Template/Отчет740.docx", urlNew);

            var list = await _context.Solutions.Include(x=>x.Registration).ThenInclude(x=>x.DocRegistry).ThenInclude(x=>x.Heads).ThenInclude(x => x.Sections).AsNoTracking().Where(x =>
                ((TypeOfReportLists == 12|| TypeOfReportLists == 9) ? x.Registration.GettingDate.Value.Date >= startDate && x.Registration.GettingDate.Value.Date <= endDate : x.dateOfSolution.Value.Date >= startDate && x.dateOfSolution.Value.Date <= endDate)
                && x.Registration.Deleted != true
                && x.Registration.TypeReg != 3
                && (TypeOfReportLists<=10 || x.Registration.DocRegistry.Regulation740 == true)
                && (TypeOfReportListSolutions == "Все" || x.solution == TypeOfReportListSolutions)
                && ((TypeOfReportLists == 11 || TypeOfReportLists == 12)
                || (Convert.ToDouble(x.Registration.DocRegistry.Sections.Number) < 99 && x.Registration.DocRegistry.Heads.Number < 99))
                ).GroupBy(x => new
                {
                    headNumber = x.Registration.DocRegistry.Heads.Number,
                    headName = x.Registration.DocRegistry.Heads.Name,
                    headId = x.Registration.DocRegistry.Heads.HedID,
                    secNumber = x.Registration.DocRegistry.Sections.Number,
                    secName = x.Registration.DocRegistry.Sections.Name,
                    secId = x.Registration.DocRegistry.Sections.SectionID,
                    docNumber = x.Registration.DocRegistry.Number,
                    docName = x.Registration.DocRegistry.RegName,
                    docIP = x.Registration.DocRegistry.IP
                }).Select(x => new
                {
                    x.Key.headNumber,
                    x.Key.headName,
                    x.Key.headId,
                    x.Key.secNumber,
                    x.Key.secName,
                    x.Key.secId,
                    x.Key.docNumber,
                    x.Key.docName,
                    x.Key.docIP,
                    count = x.Count()
                }).OrderBy(x => x.docIP).ThenBy(x => x.headNumber).ThenBy(x => x.secNumber).ThenBy(x => x.secName).ThenBy(x => x.docNumber).ThenBy(x => x.count).ToListAsync();

            
            Dictionary<string, List<string>> tab = new Dictionary<string, List<string>>();
            

            foreach (var docIP in list.Select(x => x.docIP).Distinct().ToList())
            {
                string headName = "";
                string sectionName = "";
                if (list.Any(x => x.docIP == docIP))
                {
                    if(!docIP)
                        tab.Add((tab.Count + 1).ToString(), new List<string> { "", "ФИЗ. ЛИЦА", list.Where(x => x.docIP == false).Sum(x=>x.count).ToString() });
                    else
                        tab.Add((tab.Count + 1).ToString(), new List<string> { "", "Юр. ЛИЦА", list.Where(x => x.docIP == true).Sum(x => x.count).ToString() });
                }
                    
                foreach (var d in list.Where(x => x.docIP == docIP).ToList())
                {
                    if (d.headName == headName)
                    {
                        if (d.secName == sectionName)
                        {
                            tab.Add((tab.Count + 1).ToString(), new List<string> { "Процедура " + d.docNumber, d.docName, d.count.ToString() });
                        }
                        else
                        {
                            sectionName = d.secName;
                            tab.Add("head" + (tab.Count + 1).ToString(), new List<string> { "Раздел " + d.headName, d.secName, list.Where(x => x.secId == d.secId && x.headId == d.headId).Sum(x => x.count).ToString() });
                            tab.Add((tab.Count + 1).ToString(), new List<string> { "Процедура " + d.docNumber, d.docName, d.count.ToString() });
                        }
                    }
                    else
                    {
                        headName = d.headName;
                        tab.Add("head" + (tab.Count + 1).ToString(), new List<string> { "Глава " + d.headNumber, d.headName, list.Where(x => x.headId == d.headId).Sum(x => x.count).ToString() });
                        sectionName = d.secName;
                        tab.Add("head" + (tab.Count + 1).ToString(), new List<string> { "Раздел " + d.secNumber, d.secName, list.Where(x => x.secId == d.secId && x.headId == d.headId).Sum(x => x.count).ToString() });
                        tab.Add((tab.Count + 1).ToString(), new List<string> { "Процедура " + d.docNumber, d.docName, d.count.ToString() });
                    }
                }
            }

            string headTitle = "";

            switch (TypeOfReportLists)
            {
                case 1:
                    headTitle = "Отчет по количеству принятых заявлений граждан ";
                    break;
                case 2:
                    headTitle = "Отчет по количеству принятых заявлений юридических лиц и индивидуальных предпринимателей ";
                    break;
                case 3:
                    headTitle = "Отчет по количеству зарегестрированных заявлений в разделе внутренние потребности ";
                    break;
                case 4:
                    headTitle = "Отчет по количеству принятых административных жалоб граждан ";
                    break;
                case 5:
                    headTitle = "Отчет по количеству принятых административных жалоб юридических лиц и индивидуальных предпринимателей ";
                    break;
                case 11:
                    headTitle = "Отчет по принятым решениям в соответствии ПСМ 740 ";
                    break;
                case 10:
                    headTitle = "Отчет по количеству принятых решений ";
                    break;
                case 9:
                    headTitle = "Отчет по количеству принятых решений ";
                    break;
                default:
                    headTitle = "Отчет по общему количеству принятых заявлений ";
                    break;
            }

            Dictionary<string, string> findesReplaces = new Dictionary<string, string>() {
                { "Area", _context.Settings.OrderBy(x=>x.ID).FirstOrDefault().Name },
                { "Naim", headTitle + TypeOfReportListSolutions },
                { "StartDate", startDate.ToString("d")},
                { "EndDate", endDate.ToString("d") },
                { "Itogo", list.Sum(x=>x.count).ToString()},
                { "Worker", User.Identity.Name},
                { "dtpCurrDate", DateTime.Now.Day.ToString()},
                { "dtpMonth", DateTime.Now.Month.ToString()},
                { "dtpYear", DateTime.Now.Year.ToString()}
            };


            createDocument createDoc = new createDocument();

            urlNew = createDoc.createWord(urlNew, findesReplaces, tab);
            return Json(urlNew);
        }

        /// <summary>
        /// отчет о решениях
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="radioOfSolutionType"></param>
        /// <returns></returns>
        public async Task<IActionResult> reportSolution(DateTime startDate, DateTime endDate, int radioOfSolutionType)
        {            
            string urlNew = new getUrlFile().urlFile();


            urlNew += "Reports/" + DateTime.Now.ToString("yyyy_dd_MM_HH_mm_ss") + "_" + DateTime.Now.Ticks.ToString() + "solutions.xlsx";



            List<string> solutionsTypes = new List<string> { "Положительно", "Отрицательно", "Отказано в приёме", "Отказано в осуществлении", "Отозвано", "Переадресовано" };

            Dictionary<string, int> itogFiz = new();
            Dictionary<string, int> itogYr = new();

            Dictionary<string, List<string>> tab = null;

            createDocument createDoc = new createDocument();

            foreach (var typeSol in solutionsTypes)
            {
                tab = new();
                var doc = await _context.Solutions.Include(x=>x.Registration).ThenInclude(x => x.DocRegistry).ThenInclude(x => x.Heads).ThenInclude(x => x.Sections).AsNoTracking().Where(x =>
                    (radioOfSolutionType == 0 ? x.Registration.GettingDate.Value.Date >= startDate && x.Registration.GettingDate.Value.Date <= endDate : x.dateOfSolution.Value.Date >= startDate && x.dateOfSolution.Value.Date <= endDate)
                    && x.Registration.Deleted != true
                    && x.Registration.DocNo != null
                    && (x.Registration.TypeReg == 1 || x.Registration.TypeReg == 2 || x.Registration.TypeReg == 4 || x.Registration.TypeReg == 5)
                    && x.solution == typeSol
                    ).GroupBy(x => new
                    {
                        headNumber = x.Registration.DocRegistry.Heads.Number,
                        headName = x.Registration.DocRegistry.Heads.Name,
                        headId = x.Registration.DocRegistry.Heads.HedID,
                        secNumber = x.Registration.DocRegistry.Sections.Number,
                        secName = x.Registration.DocRegistry.Sections.Name,
                        secId = x.Registration.DocRegistry.Sections.SectionID,
                        docNumber = x.Registration.DocRegistry.Number,
                        docName = x.Registration.DocRegistry.RegName,
                        docIP = x.Registration.DocRegistry.IP
                    }).Select(x => new
                    {
                        x.Key.headNumber,
                        x.Key.headName,
                        x.Key.headId,
                        x.Key.secNumber,
                        x.Key.secName,
                        x.Key.secId,
                        x.Key.docNumber,
                        x.Key.docName,
                        x.Key.docIP,
                        count = x.Count()
                    }).OrderBy(x => x.docIP).ThenBy(x => x.headNumber).ThenBy(x => x.secNumber).ThenBy(x => x.secName).ThenBy(x => x.docNumber).ThenBy(x => x.count).ToListAsync();

               

                
                tab.Add("NoBorder" + (tab.Count + 1).ToString(), new List<string> {
                _context.Settings.OrderBy(x=>x.ID).FirstOrDefault().Name+ " отчёт о решении "+typeSol+" за период с "+startDate.ToString("d")+" по "+endDate.ToString("d")
            });
                
                foreach (var docIP in doc.Select(x => x.docIP).Distinct().ToList())
                {
                    tab.Add("emptyRow" + (tab.Count + 1).ToString(), new List<string> { "", "", "" });

                    string headName = "";
                    string sectionName = "";
                    if (doc.Any(x => x.docIP == docIP))
                    {
                        if (!docIP)
                        {
                            itogFiz.Add(typeSol, doc.Where(x => x.docIP == docIP).Sum(x => x.count));
                            tab.Add((tab.Count + 1).ToString(), new List<string> { "", "ФИЗ. ЛИЦА", "" });
                        }
                        else
                        {
                            itogYr.Add(typeSol, doc.Where(x => x.docIP == docIP).Sum(x => x.count));
                            tab.Add((tab.Count + 1).ToString(), new List<string> { "", "Юр. ЛИЦА", "" });
                        }
                    }

                    foreach (var d in doc.Where(x => x.docIP == docIP).ToList())
                    {
                        if (d.headName == headName)
                        {
                            if (d.secName == sectionName)
                            {
                                tab.Add((tab.Count + 1).ToString(), new List<string> { "Подраздел " + d.docNumber, d.docName, d.count.ToString() });
                            }
                            else
                            {
                                sectionName = d.secName;
                                tab.Add("head" + (tab.Count + 1).ToString(), new List<string> { "Раздел " + d.secNumber, d.secName, doc.Where(x => x.secId == d.secId && x.headId == d.headId).Sum(x => x.count).ToString() });
                                tab.Add((tab.Count + 1).ToString(), new List<string> { "Подраздел " + d.docNumber, d.docName, d.count.ToString() });
                            }
                        }
                        else
                        {
                            headName = d.headName;
                            tab.Add("head" + (tab.Count + 1).ToString(), new List<string> { "Глава " + d.headNumber, d.headName, doc.Where(x => x.headId == d.headId).Sum(x => x.count).ToString() });
                            sectionName = d.secName;
                            tab.Add("head" + (tab.Count + 1).ToString(), new List<string> { "Раздел " + d.secNumber, d.secName, doc.Where(x => x.secId == d.secId && x.headId == d.headId).Sum(x => x.count).ToString() });
                            tab.Add((tab.Count + 1).ToString(), new List<string> { "Подраздел " + d.docNumber, d.docName, d.count.ToString() });
                        }
                    }
                }

                tab.Add("emptyRow" + (tab.Count + 1).ToString(), new List<string> { "", "", "" });

                tab.Add((tab.Count + 1).ToString(), new List<string> {
                DateTime.Now.ToString("d")
                ,""
                ,""
                });

                //Ширина столбцов
                tab.Add("width", new List<string> { "15", "95", "15" });

                createDoc.createExcel(urlNew, tab, true, typeSol);
            }


            tab = new();

            tab.Add("NoBorder" + (tab.Count + 1).ToString(), new List<string> {
                _context.Settings.OrderBy(x => x.ID).FirstOrDefault().Name+ " Количество принятых решений за период с "+startDate.ToString("d")+" по "+endDate.ToString("d")
            });

            tab.Add("emptyRow" + (tab.Count + 1).ToString(), new List<string> { });

            if (itogFiz.Any())
                tab.Add("head" + (tab.Count + 1).ToString(), new List<string> { "Физ. лица" });
            foreach (var i in itogFiz)
            {
                tab.Add((tab.Count + 1).ToString(), new List<string> { i.Key, i.Value.ToString() });
            }

            if (itogFiz.Any())
                tab.Add((tab.Count + 1).ToString(), new List<string> { "Итого:", itogFiz.Sum(x => x.Value).ToString() });

            if (itogYr.Any())
                tab.Add("head" + (tab.Count + 1).ToString(), new List<string> { "Юр. лица" });
            foreach (var i in itogYr)
            {
                tab.Add((tab.Count + 1).ToString(), new List<string> { i.Key, i.Value.ToString() });
            }

            if (itogYr.Any())
                tab.Add((tab.Count + 1).ToString(), new List<string> { "Итого:", itogYr.Sum(x => x.Value).ToString() });


            tab.Add((tab.Count + 1).ToString(), new List<string> {"Общее количество:", (itogFiz.Sum(x=>x.Value)+itogYr.Sum(x=>x.Value)).ToString()});

            //Ширина столбцов
            tab.Add("width", new List<string> { "70", "25" });

            createDoc.createExcel(urlNew, tab, true, "Итого");

            return Json(urlNew);
        }
    }
}
