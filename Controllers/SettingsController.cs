using Microsoft.AspNetCore.Mvc;
using oneWin.Data;
using oneWin.Models;
using oneWin.Models.baseModel;
using oneWin.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Reflection;
using System.Net;
using System.IO;
using System.Xml.Linq;
using System.Security.Principal;
using System.Data.OleDb;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Data.Odbc;
using System.Net.Http;

namespace oneWin.Controllers
{
    public class SettingsController : Controller
    {
        private oneWinDbContext _context;
        public SettingsController(oneWinDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Settings.FirstOrDefault());
        }

        [HttpPost]
        public async Task<IActionResult> Index(settingModel doc)
        {
            if (ModelState.IsValid)
            {

                doc.ValueArea = _context.Areas.First(x => x.Id == doc.Areas_Id).Number;
                if (doc.ID == 0)
                {
                    await _context.Settings.AddAsync(doc);
                }
                else
                {
                    _context.Settings.Update(doc);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("index");
            }
            return View(doc);
        }


        public IActionResult loadGP()
        {
            return View();
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы", Justification = "<Ожидание>")]
        public async Task<IActionResult> loadGP(loadGPModel gpModel)
        {
            if (gpModel != null)
            {
                DateTime start = gpModel.StartDateLoad;
                string pathGP = _context.Settings.First().PathGP;
                int count = 0;
                for (int i = 0; i < (gpModel.EndDateLoad.Day - start.Day + 1); i++)
                {
                    using (ImpersonateUser wi = new ImpersonateUser("name", pathGP, "password"))
                    {

                        await WindowsIdentity.RunImpersonated(wi.Identity.AccessToken, async () =>
                          {
                              try
                              {

                                  //получаем файл на стороне жилищной политики
                                  string nameFile = start.Year.ToString() + start.ToString("MM") + start.ToString("dd") + ".xml";
                                string f = pathGP + "/" + nameFile;
                                  if (System.IO.File.Exists(f))
                                  {                                      
                                      XmlDocument xdoc = new XmlDocument();
                                      xdoc.Load(f);
                                      var xRoot = xdoc.DocumentElement;
                                      foreach (XmlNode node in xRoot)
                                      {                                          
                                          string NumberSolutions = "", DateSolutions = "", ResultType = "", CaseNumber = "", OrderNo = "", LName = "", FName = "", MName = "", VidUchDelo = "";
                                          foreach (XmlNode childNode in node.ChildNodes)
                                          {
                                              if (childNode.Name == "VidUchDelo")
                                                  VidUchDelo = childNode.InnerText;
                                              if (childNode.Name == "LName")
                                                  LName = childNode.InnerText;
                                              if (childNode.Name == "FName")
                                                  FName = childNode.InnerText;
                                              if (childNode.Name == "MName")
                                                  MName = childNode.InnerText;
                                              if (childNode.Name == "NumberSolutions")
                                                  NumberSolutions = childNode.InnerText;
                                              if (childNode.Name == "DateSolutions")
                                                  DateSolutions = childNode.InnerText;
                                              if (childNode.Name == "ResultType")
                                                  ResultType = childNode.InnerText;
                                              if (childNode.Name == "CaseNumber")
                                                  CaseNumber = childNode.InnerText;
                                              if (childNode.Name == "OrderNo")
                                                  OrderNo = childNode.InnerText;
                                          }
                                          int orderNo = 0;
                                          if (Int32.TryParse(OrderNo, out orderNo))
                                          {
                                              registrationModel row = null;
                                              if (orderNo != 0)
                                                  row = await _context.Registration.FirstOrDefaultAsync(p => p.OrderNo == orderNo);
                                              else
                                              {                                                  
                                                  var d = await _context.Registration.Include(x=>x.DocRegistry).Where(p => p.LName.Contains(LName) && p.FName.Contains(FName) && p.MName.Contains(MName)).ToListAsync();
                                                  //ViewBag.message += d.Count().ToString() + "\n";
                                                  if (d.Any())
                                                  {
                                                      if (d.Any(x => x.RegID!=null)&& d.Where(x=>x.RegID!=null&&x.DocRegistry.NameGP!=null).Any(x=>x.DocRegistry.NameGP.Contains(VidUchDelo)))
                                                      {                                                          
                                                          row = d.Where(x=>x.RegID!=null && x.DocRegistry.NameGP != null).First(x => x.DocRegistry.NameGP.Contains(VidUchDelo));
                                                      }
                                                      else
                                                          ViewBag.message += "Запись не было обработана, ошибка в базе, нет данных " + LName + " " + FName + " " + MName + " не найден параметр " + VidUchDelo + "\n";
                                                  }
                                                  else
                                                      ViewBag.message += "Запись не было обработана, нет записи " + LName + " " + FName + " " + MName + "\n";
                                              }                                                  
                                              if (row != null)
                                              {
                                                  var tempSol = await _context.Solutions.FirstAsync(x => x.RegistrationId == row.RegistrationID);
                                                  if (row.State == 2)
                                                  {
                                                      row.NamberSolutions = NumberSolutions;
                                                      row.CaseNamber = CaseNumber;
                                                      row.State = 4;
                                                      row.IssueDate = gpModel.IssueDate;
                                                      row.EvaluationNotification = gpModel.EvaluationNotification;
                                                      row.ReturnInDeptDate = gpModel.ReturnInDeptDate;
                                                      row.DateSsolutions = Convert.ToDateTime(DateSolutions);
                                                      tempSol.dateOfSolution = Convert.ToDateTime(DateSolutions);
                                                      tempSol.solutionNumber = NumberSolutions;
                                                      
                                                      switch (ResultType)
                                                      {
                                                          case "0":
                                                              tempSol.solution = "Отрицательно";
                                                              break;
                                                          case "1":
                                                              tempSol.solution = "Положительно";
                                                              break;
                                                          default:
                                                              tempSol.solution = ResultType;
                                                              break;
                                                      }
                                                      _context.Solutions.Update(tempSol);
                                                      _context.Registration.Update(row);
                                                      
                                                      count++;
                                                  }

                                              }
                                              
                                          }
                                      }
                                      await _context.SaveChangesAsync();
                                  }
                              }
                              catch (System.Exception e)
                              {
                                  ViewBag.message += e.Message + "\n";
                              }
                          });
                    }                   
                    start = start.AddDays(1);
                }
                ViewBag.message += "Данные получены! Кол-во записей: " + count+"\n";
            }
            return View(gpModel);
        }

        public IActionResult sendGP()
        {
            return View();
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы", Justification = "<Ожидание>")]
        public async Task<IActionResult> sendGP(DateTime starDate, DateTime endDate, string selectFor)
        {
            if (starDate != new DateTime() && endDate != new DateTime())
            {
                if (selectFor == "site")
                {
                    int count = 0;
                    XDocument xdoc = XDocument.Load("wwwroot/Data/INFO.XML");
                    var xelem = xdoc.Element("xml");
                    var list = xelem.Elements();
                    var rows = list.Last();
                    rows.RemoveAll();
                    var listReg = await _context.Registration.Where(p => p.GettingDate >= starDate && p.GettingDate <= endDate && p.Deleted != true).ToListAsync();
                    foreach (var c in listReg)
                    {
                        try
                        {
                            XElement row = new XElement(XName.Get("row", "#RowsetSchema"));
                            XAttribute region = new XAttribute("Region", _context.Settings.First().ValueArea);
                            XAttribute LName = new XAttribute("LName", c.LName);
                            XAttribute FName = new XAttribute("FName", c.FName);
                            XAttribute MName = new XAttribute("MName", c.MName = c.MName ?? String.Empty);
                            XAttribute GettingDate;
                            if (c.GettingDate != null)
                                GettingDate = new XAttribute("GettingDate", c.GettingDate);
                            else
                                GettingDate = new XAttribute("GettingDate", "");
                            XAttribute ReturnInDeptDate;
                            if (c.ReturnInDeptDate != null)
                                ReturnInDeptDate = new XAttribute("ReturnInDeptDate", c.ReturnInDeptDate);
                            else
                                ReturnInDeptDate = new XAttribute("ReturnInDeptDate", "");
                            XAttribute State = new XAttribute("State", c.State);
                            XAttribute DocNo = new XAttribute("DocNo", c.DocNo);

                            XAttribute RegistrationID = new XAttribute("RegistrationID",
                                "{" + c.RegistrationID.ToString().ToUpper() + "}");
                            XAttribute RegID = new XAttribute("RegID", "{" + c.RegID.ToString().ToUpper() + "}");
                            row.Add(region);
                            row.Add(LName);
                            row.Add(FName);
                            row.Add(MName);
                            row.Add(GettingDate);
                            row.Add(ReturnInDeptDate);
                            row.Add(State);
                            row.Add(DocNo);
                            row.Add(RegistrationID);
                            row.Add(RegID);
                            rows.Add(row);
                            count++;
                        }
                        catch (System.Exception exception)
                        {
                            ViewBag.message += exception.Message;
                            continue;
                        }
                    }


                    using (ImpersonateUser wi = new ImpersonateUser("name", _context.Settings.First().PathSite, "password"))
                    {

                       await WindowsIdentity.RunImpersonated(wi.Identity.AccessToken, async () =>
                        {
                            try
                            {
                                var setting = await _context.Settings.FirstOrDefaultAsync();
                                using (var writer = new StreamWriter(setting.PathSite + "//INFO.XML"))
                                {
                                    xdoc.Save(writer);
                                    ViewBag.message += "Данные отправлены! Кол-во записей: " + count;
                                }
                            }
                            catch (System.Exception ex)
                            {
                                ViewBag.message += ex.Message;
                            }
                        });
                    }

                    return View();
                }
                if (selectFor == "gp")
                {
                    TransferGP(starDate, endDate);
                    return View();

                }
            }
            ViewBag.message += "Данные НЕ отправлены!";
            return View();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы", Justification = "<Ожидание>")]
        protected async void TransferGP(DateTime StartDate, DateTime EndDate)
        {
            
            using (OleDbConnection conn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=wwwroot\Data; Extended Properties=DBASE 5.0;"))
            {
                conn.Open();
                var list =_context.Registration.Include(c=>c.DocRegistry).ThenInclude(x=>x.Sections).Where(p =>
                            p.OutDeptDate >= StartDate && p.OutDeptDate <= EndDate && p.State == 2 && p.Deleted != true &&
                            p.DocRegistry.GP == true).ToList();
                OleDbCommand commandApplicant = new OleDbCommand();
                OleDbCommand command = new OleDbCommand();
                Int32 countOfExceptions = 0;
                foreach (var c in list)
                {
                    try
                    {
                        string regname = c.DocRegistry.Number + c.DocRegistry.Sections.Name + c.DocRegistry.RegName;
                        if (regname.Length > 100)
                            regname = regname.Substring(0, 100);
                        var flats = _context.InfoFlat.Where(p => p.Registration_Id == c.RegistrationID);
                        infoFlatModel flat = new();
                        if (flats.Count() > 0)
                            flat = flats.First();
                        else
                            flat = null;
                        string DROD = "";
                        if (c.PersonalNo.Length >= 7)
                            DROD = c.PersonalNo[1].ToString() + c.PersonalNo[2].ToString() + "." +
                                   c.PersonalNo[3].ToString() + c.PersonalNo[4].ToString() + "." +
                                   c.PersonalNo[5].ToString() + c.PersonalNo[6].ToString();
                        DateTime DRODdate;
                        if (DateTime.TryParse(DROD, out DRODdate))
                            DROD = "'" + DRODdate.ToString("d") + "'";
                        else
                            DROD = "null";
                        string CodeUl = KodUl(c);
                        string street = GetStreet(c);
                        string tip = UlTIp(c);
                        string ntip = NTIP(c);
                        string home = House(c);
                        string IndHome = Ind(c);
                        string KorpHome = Korp(c);
                        string Flats = Flat(c);
                        string flatInd = Index(c);
                        commandApplicant =
                            new OleDbCommand(
                                "INSERT INTO obmen751 ( FNAME, MNAME, LNAME, KODUL, STREET, TIP, NTIP, HAUSE, IND, KORP, FLAT, [INDEX], PHONENO, PASSPORTNO, PASSISDATE," +
                                "PASSISSUER, PERSONALNO, GETDATE, GETTIME, OUTDATE, ORDERNO, REGNAME, DROD, NPRAV, PLO, PLG, KOLB )" +
                                "VALUES('"
                                + c.FName + "','"
                                + (c.MName ?? "отсутствует") + "','"
                                + c.LName + "',"
                                + KodUl(c) + ",'"
                                + GetStreet(c) + "','"
                                + NTIP(c) + "','" +
                                UlTIp(c) + "','"
                                + House(c) + "','"
                                + Ind(c) + "','"
                                + Korp(c) + "','"
                                + Flat(c) + "','"
                                + Index(c) + "','"
                                + c.PhoneNo + "','"
                                + c.PassportNo + "',"
                                + (c.PassIssuerDate != null ? "'" + c.PassIssuerDate.Value.ToString("d") + "'" : "null") +
                                ",'"
                                + (c.PassIssuer.Length > 50 ? c.PassIssuer.Substring(0, 50) : c.PassIssuer) + "','"
                                + c.PersonalNo + "','"
                                + (c.GettingDate != null ? c.GettingDate.Value.ToString("d") : "") + "','" +
                                (c.GettingDate != null ? c.GettingDate.Value.ToString("t") : "") + "','" +
                                (c.OutDeptDate != null ? c.OutDeptDate.Value.ToString("d") : "") + "',"
                                + c.OrderNo + ",'"
                                + regname + "'," + DROD + ",'"
                                + (flat != null ? flat.Pravo : "") + "',"
                                + (flat != null ? "'" + flat.totalArea.ToString().Replace(".",",") + "'" : "'0'") + ","
                                + (flat != null ? "'" + flat.livingSpace.ToString().Replace(".", ",") + "'" : "'0'") + ","
                                + (flat != null ? "'" + flat.personNumber.ToString() + "'" : "'0'") + ")", conn);
                        commandApplicant.ExecuteNonQuery();

                }
                    catch (System.Exception ex)
                {
                    ViewBag.message += "№ " + c.DocNo + " " + c.LName + " НЕ внесен" + ex.Message +
                                   commandApplicant.CommandText + "\n";
                    countOfExceptions++;
                }
                foreach (var f in _context.Family.Where(x=>x.RegistrationID== c.RegistrationID).ToList())
                    {
                        try
                        {
                            command =
                                new OleDbCommand(
                                    "INSERT INTO obmen753 ( FNAME, MNAME, LNAME, DROD, NROTN, PASSPORTNO, PASSISDATE, PASSISSUER, PERSONALNO, ORDERNO ) " +
                                    "VALUES('" + f.FName + "','" + f.MName + "','" + f.LName + "','" +
                                    (f.DOB != null ? f.DOB.Value.ToString("d") : "") + "','" + f.NRotN + "',"
                                    + (f.PassportNo != "" ? "'" + f.PassportNo + "'" : "null") + ","
                                    +
                                    (f.PassIssuerDate != null
                                        ? "'" + f.PassIssuerDate.Value.ToString("d") + "'"
                                        : "null")
                                    + ","
                                    + (!string.IsNullOrEmpty(f.PassIssuer) ? "'" + (f.PassIssuer.Length > 50 ? f.PassIssuer.Substring(0, 50) : f.PassIssuer) + "'" : "null")
                                    + ","
                                    + (f.PersonalNo != "" ? "'" + f.PersonalNo + "'" : "null") +
                                    "," + c.OrderNo + ")", conn);
                            command.ExecuteNonQuery();
                        }
                        catch (System.Exception ex)
                        {
                            ViewBag.message += f.LName + " " + f.FName + " " + f.MName + " Заявитель : " + c.LName + "\n" +
                                           ex.Message + command.CommandText + "\n";
                        }
                    }
                }
                ViewBag.message += "Выгружено " + (list.Count() - countOfExceptions) + " зап.";

               
                string path = _context.Settings.First().PathGP;

                using (ImpersonateUser wi = new ImpersonateUser("name", path, "password"))
                {

                    await WindowsIdentity.RunImpersonated(wi.Identity.AccessToken, async () =>
                    {
                        try
                        {
                            await Task.Run(() =>
                            {
                                new FileInfo(path + @"\obmen751.dbf").Delete();
                                new FileInfo(path + @"\obmen753.dbf").Delete();
                                new FileInfo("wwwroot/Data/obmen751.dbf").CopyTo(path + @"\obmen751.dbf");
                                new FileInfo("wwwroot/Data/obmen753.dbf").CopyTo(path + @"\obmen753.dbf");
                            });
                        }
                        catch (System.Exception ex)
                        {
                            ViewBag.message += ex.Message;
                        }
                    });
                }

                OleDbCommand ClearFamaly = new OleDbCommand("DELETE FROM obmen751", conn);
                ClearFamaly.ExecuteNonQuery();
                ClearFamaly = new OleDbCommand("DELETE FROM obmen753", conn);
                ClearFamaly.ExecuteNonQuery();
                conn.Close();
            }
        }

        private string GetStreet(registrationModel row)
        {

            string[] streets = row.Address.Split(' ');
            string street = "";
            for (int i = 0; i < streets.Count() - 1; i++)
                street += streets[i];
            return street;
        }
        /// <summary>
        /// Тип улицы
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private string UlTIp(registrationModel row)
        {
            string[] tipName = row.Address.Split(' ');
            return tipName[tipName.Count() - 1];
        }
        /// <summary>
        /// Корпус
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private string Korp(registrationModel row)
        {
            string[] korps = row.Home.Split('к');
            if (korps.Count() > 1)
            {
                string korp = "";
                for (int i = 0; i < korps[1].Length; i++)
                {
                    if (korps[1][i] >= '0' && korps[1][i] <= '9')
                        korp += korps[1][i];
                }
                return korp;
            }
            return "";
        }
        /// <summary>
        /// Дом
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private string House(registrationModel row)
        {
            string[] korps = row.Home.Split('к');
            if (korps.Count() > 1)
            {
                string korp = "";
                for (int i = 0; i < korps[0].Length; i++)
                {
                    if (korps[0][i] >= '0' && korps[0][i] <= '9')
                        korp += korps[0][i];
                }
                return korp;
            }
            else
            {
                string korp = "";
                for (int i = 0; i < row.Home.Length; i++)
                {
                    if (row.Home[i] >= '0' && row.Home[i] <= '9')
                        korp += row.Home[i];
                }
                return korp;
            }
        }
        /// <summary>
        /// Индекс дома
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private string Ind(registrationModel row)
        {
            string[] korps = row.Home.Split('к');
            if (korps.Count() > 1)
            {
                string ind = "";
                for (int i = 0; i < korps[1].Length; i++)
                {
                    if (korps[1][i] < '0' || korps[1][i] > '9')
                        ind += korps[1][i];
                }
                return ind;
            }
            else
            {
                string ind = "";
                for (int i = 0; i < row.Home.Length; i++)
                {
                    if (row.Home[i] < '0' || row.Home[i] > '9')
                        ind += row.Home[i];
                }
                return ind;
            }
        }
        /// <summary>
        /// Индекс квартиры
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private string Index(registrationModel row)
        {
            string ind = "";
            if (string.IsNullOrEmpty(row.Flat))
                return ind;
            for (int i = 0; i < row.Flat.Length; i++)
            {
                if (row.Flat[i] < '0' || row.Flat[i] > '9')
                    ind += row.Flat[i];
            }
            return ind;
        }
        /// <summary>
        /// Номер квартиры
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private string Flat(registrationModel row)
        {
            string ind = "";
            if (string.IsNullOrEmpty(row.Flat))
                return ind;
            for (int i = 0; i < row.Flat.Length; i++)
            {
                if (row.Flat[i] >= '0' && row.Flat[i] <= '9')
                    ind += row.Flat[i];
            }
            return ind;
        }
        /// <summary>
        /// Код улицы
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private string KodUl(registrationModel row)
        {
            var uls = _context.RVC_SULIC.Where(p => p.NAME == row.Address);
            if (uls.Count() > 0)
            {
                var ul = uls.First();
                if (ul != null)
                    return ul.KODUL.ToString();
            }
            return "0";
        }
        /// <summary>
        /// Код типа улицы
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private string NTIP(registrationModel row)
        {
            var uls = _context.RVC_SULIC.Where(p => p.NAME == row.Address);
            if (uls.Count() > 0)
            {
                var ul = uls.First();
                if (ul != null)
                    return ul.ULTIP.ToString();
            }
            return "0";
        }

    }


}
