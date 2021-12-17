using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using oneWin.Data;
using oneWin.Models;
using oneWin.OfficeCreate;
using oneWin.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Controllers
{   
    public class AnalyticsController : Controller
    {
        private oneWinDbContext _context;

        public AnalyticsController(oneWinDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Количество принятых заявлений в час
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="ProcedureName"></param>
        /// <param name="RegistratorName"></param>
        /// <returns></returns>
        public IActionResult AnalyticsColHour(DateTime startDate, DateTime endDate, string ProcedureName, string RegistratorName)
        {            
            string urlNew = new getUrlFile().urlFile();


            urlNew += "Reports/" + DateTime.Now.ToString("yyyy_dd_MM_HH_mm_ss") + "_" + DateTime.Now.Ticks.ToString() + "AnalyticsColHour.xlsx";



            Dictionary<string, List<string>> tab = null;

            createDocument createDoc = new createDocument();

            
                tab = new();

            var doc = _context.Registration.Where(x => x.GettingDate.Value.Date >= startDate
                    && x.GettingDate.Value.Date <= endDate
                    && x.Deleted != true
                    && x.TypeReg != 3
                    && (ProcedureName == null || x.Number == ProcedureName)
                    && (RegistratorName == null || x.Registrator.Contains(RegistratorName)
                    ))
                .GroupBy(x => new { x.GettingDate.Value.Date, x.GettingDate.Value.Hour })
                .Select(x => new { x.Key.Date, x.Key.Hour, count = x.Count() }).OrderBy(x => x.Date).ThenBy(x => x.Hour).ToList();


            tab.Add("head" + (tab.Count + 1).ToString(), doc.Select(x => x.Date.ToString("d")).Distinct().ToList());

            tab["head" + (tab.Count).ToString()].Insert(0, "Итого");

            tab["head" + (tab.Count).ToString()].Insert(0, "Часы/дата");

            List<string> dateList = new();

            for (int i = 8; i <= 19; i++)
            {

                dateList.Add(i + ":00 - " + (i + 1) + ":00");

                dateList.Add(doc.Where(x => x.Hour >= i && x.Hour < (i + 1)).Sum(x => x.count).ToString());
                foreach (var t in doc.Select(x => x.Date).Distinct().ToList())
                {
                    dateList.Add(doc.Where(x => x.Hour >=i && x.Hour < (i + 1) && x.Date == t.Date).Sum(x => x.count).ToString());
                }
                tab.Add((tab.Count + 1).ToString(), dateList);
                dateList = new();
            }

            tab.Add("head" + (tab.Count + 1).ToString(), doc.GroupBy(x => new { x.Date }).Select(x => x.Sum(s=>s.count).ToString()).ToList());
            tab["head" + (tab.Count).ToString()].Insert(0, doc.GroupBy(x => new { x.Date }).Select(x => x.Sum(s => s.count)).Sum().ToString());
            tab["head" + (tab.Count).ToString()].Insert(0, "Итого");


            tab.Add("width", new List<string> { "16" });

            createDoc.createExcel(urlNew, tab, true, "Итого");

            return Json(urlNew);
        }

        /// <summary>
        /// Аналитика по главам
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="DateList">Выборка по дате</param>
        /// <param name="Person">физ(0) или юр(1) лица</param>
        /// <param name="Chapter">Глава</param>
        /// <param name="ListQuestion">По админ. процедурам и дополнительным вопросам</param>
        /// <param name="radioOfDocTime">Формирование отчета по</param>
        /// <returns></returns>
        public IActionResult AnalyticsChapter(DateTime startDate, DateTime endDate, int DateList, int Person, string Chapter, int ListQuestion, int radioOfDocTime)
        {
            string urlNew = new getUrlFile().urlFile();

            urlNew += "Reports/" + DateTime.Now.ToString("yyyy_dd_MM_HH_mm_ss") + "_" + DateTime.Now.Ticks.ToString() + "AnalyticsChapter.xlsx";



            Dictionary<string, List<string>> tab = null;

            createDocument createDoc = new createDocument();


            tab = new();




            var doc = _context.Registration.AsNoTracking()
                .Where(x =>
            (DateList == 0 ? x.GettingDate.Value.Date >= startDate && x.GettingDate.Value.Date <= endDate : x.MustBeReady.Value.Date >= startDate && x.MustBeReady.Value.Date <= endDate)
                    && x.TypeReg != 3
                    && x.Deleted != true
                    && (Chapter == null || x.DocRegistry.Heads.Name.Contains(Chapter))
                    && (Person == 2 || x.DocRegistry.IP == (Person == 1 || false))                   
                    && (ListQuestion == 1 ? (Convert.ToDouble(x.DocRegistry.Sections.Number) >= 99 && x.DocRegistry.Heads.Number >= 99) : ListQuestion == 0 ? (Convert.ToDouble(x.DocRegistry.Sections.Number) < 99 && x.DocRegistry.Heads.Number < 99) : true)
                    )
                .OrderBy(x => (DateList == 0 ? x.GettingDate : x.MustBeReady)).ThenBy(x => x.DocRegistry.IP).ThenBy(x => x.DocRegistry.Heads.Number).ThenBy(x => x.DocRegistry.Sections.Number).ThenBy(x => x.DocRegistry.Sections.Name).ThenBy(x => x.DocRegistry.RegName)
                .Select(x => new
                {
                    headNumber = (int?)x.DocRegistry.Heads.Number,
                    headName = x.DocRegistry.Heads.Name,
                    headId = x.DocRegistry.HeadsID,
                    secNumber = x.DocRegistry.Sections.Number,
                    secName = x.DocRegistry.Sections.Name,
                    secId = x.DocRegistry.SectionID,
                    docNumber = x.DocRegistry.Number,
                    docName = x.DocRegistry.RegName,
                    docIP =(bool?)x.DocRegistry.IP,
                    docId =  (Guid?)x.DocRegistry.RegID,
                    date = (DateList == 0 ? radioOfDocTime == 1 ? x.GettingDate.Value.Date.ToString("MMMM/yyyy") : radioOfDocTime == 2 ? x.GettingDate.Value.Date.ToString("yyyy") : x.GettingDate.Value.Date.ToString("d")
                    : radioOfDocTime == 1 ? x.MustBeReady.Value.Date.ToString("MMMM/yyyy") : radioOfDocTime == 2 ? x.MustBeReady.Value.Date.ToString("yyyy") : x.MustBeReady.Value.Date.ToString("d")).ToString()
                }).ToList()
                .GroupBy(x => new
                 {
                     x.headNumber,
                     x.headName,
                     x.headId,
                     x.secNumber,
                     x.secName,
                     x.secId,
                     x.docNumber,
                     x.docName,
                     x.docIP,
                     x.docId,
                     x.date
                 }).ToList();


            tab.Add("NoBorder" + (tab.Count + 1).ToString(), new List<string> { _context.Settings.FirstOrDefault().Name + " Аналитика по "+(DateList == 1 ? "поданным заявлениям" : "принятым решениям")+" за период с " + startDate.ToString("d") + " по " + endDate.ToString("d")  });
            tab.Add("emptyRow" + (tab.Count + 1).ToString(), new List<string> { });
            tab.Add("head" + (tab.Count + 1).ToString(), doc.Select(x => x.Key.date).Distinct().ToList());
            tab["head" + (tab.Count).ToString()].Insert(0, "Процедура/дата");

            List<string> dateList = new();


            var procId = _context.Registration.Where(x => x.DocRegistry.Number != null && (ListQuestion == 1 ? (Convert.ToDouble(x.DocRegistry.Sections.Number) >= 99 && x.DocRegistry.Heads.Number >= 99) : ListQuestion == 0 ? (Convert.ToDouble(x.DocRegistry.Sections.Number) < 99 && x.DocRegistry.Heads.Number < 99) : true) && (Person == 2 || x.DocRegistry.IP == (Person == 1 || false)) && x.TypeReg != 3 && x.Deleted != true && (Chapter == null || x.DocRegistry.Heads.Name.Contains(Chapter))).Select(x => new
            {
                headNumber = x.DocRegistry.Heads.Number,
                headName = x.DocRegistry.Heads.Name,
                secNumber = x.DocRegistry.Sections.Number,
                secName = x.DocRegistry.Sections.Name,
                docNumber = x.DocRegistry.Number,
                docName = x.DocRegistry.RegName,
                docIP = x.DocRegistry.IP,
                docId = x.DocRegistry.RegID,
                headId = x.DocRegistry.HeadsID,
                secId = x.DocRegistry.SectionID
            }).AsEnumerable()
                .GroupBy(x => new
                {
                    x.docIP,
                    x.headNumber,
                    x.secNumber,
                    x.docNumber,
                    x.headName,
                    x.secName,
                    x.docName,
                    x.docId,
                    x.headId,
                    x.secId
                })
                .OrderBy(x => x.Key.docIP).ThenBy(x => x.Key.headNumber).ThenBy(x => x.Key.secNumber).ThenBy(x => x.Key.docNumber).ThenBy(x => x.Key.headName).ThenBy(x => x.Key.secName).ThenBy(x => x.Key.docName).ToList();

            bool[] docIpArray = new bool[] { false, true };

            foreach (var docIP in docIpArray)
            {
                string headName = "";
                string sectionName = "";
                if (doc.Any(x => x.Key.docIP == docIP))
                {
                    if (!docIP)
                        tab.Add((tab.Count + 1).ToString(), new List<string> { "ФИЗ. ЛИЦА" });
                    else
                        tab.Add((tab.Count + 1).ToString(), new List<string> { "Юр. ЛИЦА" });
                }
                foreach (var d in procId.Where(x => x.Key.docIP == docIP))
                {

                    if (d.Key.headName == headName)
                    {
                        if (d.Key.secName == sectionName)
                        {
                            dateList.Add("Процедура " + d.Key.docNumber + " " + d.Key.docName);

                            foreach (var t in doc.Select(x => x.Key.date).Distinct().ToList())
                            {
                                dateList.Add(doc.Where(x => x.Key.docId == d.Key.docId && x.Key.date == t).Sum(x => x.Count()) > 0 ? doc.Where(x => x.Key.docId == d.Key.docId && x.Key.date == t).Sum(x => x.Count()).ToString() : "");
                            }
                        }
                        else
                        {
                            sectionName = d.Key.secName;
                            dateList.Add("Раздел " + d.Key.secNumber + " " + d.Key.secName);

                            foreach (var t in doc.Select(x => x.Key.date).Distinct().ToList())
                            {
                                dateList.Add(doc.Where(x => x.Key.secId == d.Key.secId && x.Key.date == t).Sum(x => x.Count()) > 0 ? doc.Where(x => x.Key.secId == d.Key.secId && x.Key.date == t).Sum(x => x.Count()).ToString() : "");
                            }

                            tab.Add("head" + (tab.Count + 1).ToString(), dateList);
                            dateList = new();

                            dateList.Add("Процедура " + d.Key.docNumber + " " + d.Key.docName);

                            foreach (var t in doc.Select(x => x.Key.date).Distinct().ToList())
                            {
                                dateList.Add(doc.Where(x => x.Key.docId == d.Key.docId && x.Key.date == t).Sum(x => x.Count()) > 0 ? doc.Where(x => x.Key.docId == d.Key.docId && x.Key.date == t).Sum(x => x.Count()).ToString() : "");
                            }
                        }
                    }
                    else
                    {
                        headName = d.Key.headName;

                        dateList.Add("Глава " + d.Key.headNumber + " " + d.Key.headName);

                        foreach (var t in doc.Select(x => x.Key.date).Distinct().ToList())
                        {
                            dateList.Add(doc.Where(x => x.Key.headId == d.Key.headId && x.Key.date == t).Sum(x => x.Count()) > 0 ? doc.Where(x => x.Key.headId == d.Key.headId && x.Key.date == t).Sum(x => x.Count()).ToString() : "");
                        }

                        tab.Add("head" + (tab.Count + 1).ToString(), dateList);
                        dateList = new();

                        sectionName = d.Key.secName;
                        dateList.Add("Раздел " + d.Key.secNumber + " " + d.Key.secName);

                        foreach (var t in doc.Select(x => x.Key.date).Distinct().ToList())
                        {
                            dateList.Add(doc.Where(x => x.Key.secId == d.Key.secId && x.Key.date == t).Sum(x => x.Count()) > 0 ? doc.Where(x => x.Key.secId == d.Key.secId && x.Key.date == t).Sum(x => x.Count()).ToString() : "");
                        }

                        tab.Add("head" + (tab.Count + 1).ToString(), dateList);
                        dateList = new();

                        dateList.Add("Процедура " + d.Key.docNumber + " " + d.Key.docName);

                        foreach (var t in doc.Select(x => x.Key.date).Distinct().ToList())
                        {
                            dateList.Add(doc.Where(x => x.Key.docId == d.Key.docId && x.Key.date == t).Sum(x => x.Count()) > 0 ? doc.Where(x => x.Key.docId == d.Key.docId && x.Key.date == t).Sum(x => x.Count()).ToString() : "");
                        }

                    }
                    tab.Add((tab.Count + 1).ToString(), dateList);
                    dateList = new();
                }

            }
            tab.Add("width", new List<string> { "130" });

            createDoc.createExcel(urlNew, tab, true, "Итого");

            return Json(urlNew);
        }

        /// <summary>
        /// Аналитика по решениям
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public IActionResult AnalyticsDecision(DateTime startDate, DateTime endDate)
        {            
            string urlNew = new getUrlFile().urlFile();


            urlNew += "Reports/" + DateTime.Now.ToString("yyyy_dd_MM_HH_mm_ss") + "_" + DateTime.Now.Ticks.ToString() + "AnalyticsDecision.xlsx";

            DateTime startSolution = Convert.ToDateTime("01/01/1900");
            DateTime endSolution = Convert.ToDateTime("03/02/1900");

            var list = _context.Solutions.Where(x => x.Registration.GettingDate.Value.Date >= startDate 
            && x.Registration.GettingDate.Value.Date <= endDate
            && x.dateOfSolution.Value.Date <= endSolution.Date
            && x.Registration.Deleted != true
            && !x.Registration.Number.Contains("99") 
            && !x.Registration.Number.Contains("201") 
            && !x.Registration.Number.Contains("110")
            && x.Registration.TypeReg != 3 
            && x.Registration.TypeReg != 6 
            && x.Registration.TypeReg != 7
            && x.Registration.State>=3)
                .Select(x=>new
                {
                    x.Registration.FName,
                    x.Registration.MName,
                    x.Registration.LName,
                    x.Registration.GettingDate,
                    x.Registration.DocNo,
                    x.Registration.Number,
                    x.solution
                }).OrderBy(x=>x.GettingDate).ToList();

            Dictionary<string, List<string>> tab = new Dictionary<string, List<string>>();
            tab.Add("NoBorder" + (tab.Count + 1).ToString(), new List<string> {
                _context.Settings.FirstOrDefault().Name+ " Аналитика по решениям за период с "+startDate.ToString("d")+" по "+endDate.ToString("d")
            });

            tab.Add("emptyRow" + (tab.Count + 1).ToString(), new List<string> {});

            tab.Add("head" + (tab.Count + 1).ToString(), new List<string> {"Имя", "Отчество","Фамилия","Дата приема","Номер рег.карты","Процедура","Решение"});

            foreach (var s in list)
            {
                tab.Add((tab.Count + 1).ToString(), new List<string> {s.FName, s.MName, s.LName, s.GettingDate.ToString(), s.DocNo.ToString(), s.Number, s.solution});
            }

            tab.Add("emptyRow" + (tab.Count + 1).ToString(), new List<string> { "", "", "" });

            tab.Add("NoBorder"+(tab.Count + 1).ToString(), new List<string> {
                "Итого: "+list.Count().ToString()});

            ////Ширина столбцов
            tab.Add("width", new List<string> { "15", "20", "20", "20", "15", "15", "15" });

            createDocument createDoc = new createDocument();

            urlNew = createDoc.createExcel(urlNew, tab, true);


            return Json(urlNew);
        }
    }
}
