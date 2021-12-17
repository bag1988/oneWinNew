using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using oneWin.Data;
using oneWin.Models;
using oneWin.Models.baseModel;
using oneWin.OfficeCreate;
using oneWin.Service;

namespace oneWin.Controllers
{
    public class CreateController : Controller
    {
        private oneWinDbContext _context;
        private AppDbContext _acontext;
        private WorkingDay _w;
        private viewTable _v;

        public CreateController(oneWinDbContext context, WorkingDay w, viewTable v, AppDbContext acontext)
        {
            _acontext = acontext;
            _context = context;
            _w = w;
            _v = v;
        }
        public async Task<IActionResult> Index(Guid? regId = null, string data = null)
        {
            createStatement registration = new();
            int typePerson = 0;
            var queryListCookie = _v.stringToDictionary(Request.Cookies["queryList"]);
            if (queryListCookie.ContainsKey("typePerson"))
            {
                int.TryParse(queryListCookie["typePerson"], out typePerson);
            }
            //ид-карта есифул
            if (!string.IsNullOrEmpty(data) && Request.Cookies.ContainsKey("codeVerifier") && !string.IsNullOrEmpty(Request.Cookies["codeVerifier"]))
            {
                
                registration = await loginCallback(data, Request.Cookies["codeVerifier"]);
                if (typePerson != 0)
                {
                    registration.TypeReg = typePerson;
                }
                return View(registration);
            }

            //копируем заявление
            if (regId != null)
            {
                if (!await _context.Registration.AnyAsync(x => x.RegistrationID == regId))
                    return NoContent();
                registrationModel reg = await _context.Registration.Include(x => x.DocRegistry).Include(x => x.sol).FirstAsync(x => x.RegistrationID == regId);
                reg.MustBeReady = null;                

                foreach (var props in reg.GetType().GetProperties())
                {
                    if (registration.GetType().GetProperties().Any(x => x.Name == props.Name))
                    {
                        props.SetValue(registration, props.GetValue(reg), null);
                    }
                }
                registration.NameDocDop = await _context.SelectDocs.AsNoTracking().Where(x => x.RegId == regId).Select(x => (Guid)x.DocId).ToListAsync();
                registration.NameZpDop = await _context.SelectZaprDocs.AsNoTracking().Where(x => x.RegId == regId).Select(x => (Guid)x.ZaprDocId).ToListAsync();
                return View(registration);
            }

            //физ или юр лицо
            if (typePerson != 0)
            {                
                registration.TypeReg = typePerson;
                return View(registration);
            }
            return View(registration);
        }
        [HttpPost]
        public async Task<IActionResult> Index(createStatement doc, ICollection<IFormFile> fileUrl = null)
        {
            if (ModelState.IsValid)
            {
                doc.Registrator = User.Identity.Name;
                doc.GettingDate = DateTime.Now;
                doc.State = 1;
                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
                doc.FName = ti.ToTitleCase(doc.FName);
                doc.LName = ti.ToTitleCase(doc.LName);
                doc.MName = doc.MName == null ? string.Empty : ti.ToTitleCase(doc.MName);
                doc.PerformerName = _context.Performers.FindAsync(doc.Organiz).Result.FIO;               
                doc.PassportNo = doc.PassportNo != null ? doc.PassportNo.ToUpper() : null;
                doc.PersonalNo = doc.PersonalNo != null ? doc.PersonalNo.ToUpper() : string.Empty;
                doc.Flat = doc.Flat == null ? string.Empty : doc.Flat;
                doc.Notes = doc.Notes == null ? string.Empty : doc.Notes;
                doc.MobPhone = doc.MobPhone == null ? string.Empty : doc.MobPhone;
                doc.PhoneNo = doc.PhoneNo != null ? doc.PhoneNo : string.Empty;
                doc.Proceedings = doc.Proceedings == null ? string.Empty : doc.Proceedings;
                doc.e_mail = doc.e_mail == null ? string.Empty : doc.e_mail;
                _context.Registration.Add(doc);

                await _context.SaveChangesAsync();
                await _context.Solutions.AddAsync(new solutionModel { Id = Guid.NewGuid(), RegistrationId = doc.RegistrationID, solutionNumber="" });

                if (fileUrl != null)
                {
                    await saveAttachFile(doc.RegistrationID, fileUrl);
                }

                if (doc.NameDocDop != null)
                {
                    if (await _context.SelectDocs.AnyAsync(x => x.RegId == doc.RegistrationID))
                    {
                        var selDoc = await _context.SelectDocs.Where(x => x.RegId == doc.RegistrationID).ToListAsync();

                        _context.SelectDocs.RemoveRange(selDoc.Where(x => !doc.NameDocDop.Contains((Guid)x.DocId)));

                        doc.NameDocDop = doc.NameDocDop.Where(x => !selDoc.Select(s => s.DocId).Contains(x)).ToList();

                    }
                    await _context.SelectDocs.AddRangeAsync(doc.NameDocDop.Select(x => new selectDocsModel { Id = Guid.NewGuid(), RegId = doc.RegistrationID, DocId = x }));
                }

                //проверка выбраны ли все документы для запроса
                int countZaprDoc = await _context.BufOrgsZApr.CountAsync(p => p.RegID == doc.RegID);
                bool FullZaprDoc = false;
                //int countSelectZaprDoc = await _context.SelectZaprDocs.CountAsync(p => p.RegId == doc.RegistrationID);
                if (doc.NameZpDop != null && doc.NameZpDop.Count == countZaprDoc && !doc.extendSrok)
                    FullZaprDoc = true;
                if (countZaprDoc == 0)
                    FullZaprDoc = true;
                if (doc.NameZpDop != null)
                {
                    if (await _context.SelectZaprDocs.AnyAsync(x => x.RegId == doc.RegistrationID))
                    {
                        var selDoc = await _context.SelectZaprDocs.Where(x => x.RegId == doc.RegistrationID).ToListAsync();

                        _context.SelectZaprDocs.RemoveRange(selDoc.Where(x => !doc.NameZpDop.Contains((Guid)x.ZaprDocId)));

                        doc.NameZpDop = doc.NameZpDop.Where(x => !selDoc.Select(s => s.ZaprDocId).Contains(x)).ToList();

                    }

                    await _context.SelectZaprDocs.AddRangeAsync(doc.NameZpDop.Select(x => new selectZaprDocsModel { Id = Guid.NewGuid(), RegId = doc.RegistrationID, ZaprDocId = x }));
                }

                await _context.SaveChangesAsync();

                var d = await _context.DocRegistry.FindAsync(doc.RegID);
                if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday && d.IssueTerms.Value == 0)
                {
                    doc.MustBeReady = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 0, 0);
                }
                else
                {                    
                    int i = 0;
                    if (!string.IsNullOrEmpty(d.TypeIssue) && d.TypeIssue.Replace(" ", "") != "")
                        i = Convert.ToInt32(d.TypeIssue.Replace(" ", ""));
                    
                    int count = d.IssueZapr.Value;
                    int typeSrok = d.TypeIssueZapr.Value;

                    if (((count == 0 && d.Issue == 1) || FullZaprDoc) && !doc.extendSrok)
                        doc.MustBeReady = await _w.GetDay(d.IssueTerms.Value, i/*doc.TypeIssue*/, doc.GettingDate.Value);
                    else
                    {
                        if (doc.extendSrok)
                        {
                            if (count == 0 && (new int[] { 3, 7 }).Contains(typeSrok))
                                count = 1;
                            else if (count == 0)
                            {
                                count = d.IssueTerms.Value;
                                typeSrok = i;
                            }
                        }
                        doc.MustBeReady = await _w.GetDay(count, typeSrok, doc.GettingDate.Value);
                    }

                }

                var NumberOrder = await _context.Registration.CountAsync(x => x.OrderNo <= doc.OrderNo && x.GettingDate.Value.Year == doc.GettingDate.Value.Year && x.TypeReg == doc.TypeReg);

                //if (doc.TypeReg == 1)
                //    NumberOrder += _context.Settings.OrderBy(x=>x.ID).First().StartDocNoFis != null ? _context.Settings.OrderBy(x => x.ID).First().StartDocNoFis.Value : 0;
                //if (doc.TypeReg == 2)
                //    NumberOrder += _context.Settings.OrderBy(x => x.ID).First().StartDocNoYur != null ? _context.Settings.OrderBy(x => x.ID).First().StartDocNoYur.Value : 0;
                doc.DocNo = NumberOrder;


                _context.Registration.Update(doc);
                await _context.SaveChangesAsync();
                await generateDocument(doc.RegistrationID);

                return RedirectToAction("edit", new { regId = doc.RegistrationID });
            }

            return View(doc);
        }
        public IActionResult logout()
        {
            var s = logoutEsiful();
            if (string.IsNullOrEmpty(s))
                return Redirect("/create/Index");            
            return Redirect(s);
        }

        [HttpPost]
        public async Task<IActionResult> deleteAttachFile(Guid idFile)
        {
            if (!await _context.AttachedFile.AnyAsync(x => x.Id == idFile))
            {
                return BadRequest("Файл не найдено!");
            }

            var attachFile = await _context.AttachedFile.FindAsync(idFile);

            if (System.IO.File.Exists(attachFile.Url))
            {
                System.IO.File.Delete(attachFile.Url);
            }
            _context.AttachedFile.Remove(attachFile);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> saveAttachFile(Guid registration, ICollection<IFormFile> fileUrl)
        {
            if (!await _context.Registration.AnyAsync(x => x.RegistrationID == registration))
            {
                return BadRequest("Заявление не найдено!");
            }
            getUrlFile getUrl = new getUrlFile();
            string nameDir = "AttachedFiles/";
            string nameFile = "";
            string urlFile = "";
            string exeFile = "";
            Guid idAttach = Guid.Empty;
            foreach (var f in fileUrl)
            {
                idAttach = Guid.NewGuid();
                exeFile = Path.GetExtension(f.FileName);
                nameFile = Regex.Replace(f.FileName.Replace(exeFile, ""), @"\W", "_");
                urlFile = getUrl.urlFile() + nameDir + idAttach.ToString().Replace("-", "_") + "_" + nameFile + exeFile;

                using (var stream = new FileStream(urlFile, FileMode.Create))
                {
                    await f.CopyToAsync(stream);
                }

                await _context.AttachedFile.AddAsync(new attachedFileModel { Id = idAttach, AttachingDateTime = DateTime.Now, RegistrationId = registration, Name = nameFile, Url = urlFile });
            }
            await _context.SaveChangesAsync();

            return Ok();
        }

        public async Task<IActionResult> Edit(Guid regId, string message=null)
        {
            if(message!=null)
                ViewBag.message = message;
            if (!await _context.Registration.AnyAsync(x => x.RegistrationID == regId))
                return NoContent();
            registrationModel reg = await _context.Registration.Include(x => x.DocRegistry).Include(x => x.sol).FirstAsync(x => x.RegistrationID == regId);

            createStatement registration = new();

            foreach (var props in reg.GetType().GetProperties())
            {
                if (registration.GetType().GetProperties().Any(x => x.Name == props.Name))
                {
                    props.SetValue(registration, props.GetValue(reg), null);
                }
            }
            registration.NameDocDop = await _context.SelectDocs.AsNoTracking().Where(x => x.RegId == regId).Select(x => (Guid)x.DocId).ToListAsync();
            registration.NameZpDop = await _context.SelectZaprDocs.AsNoTracking().Where(x => x.RegId == regId).Select(x => (Guid)x.ZaprDocId).ToListAsync();
            return View(registration);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(createStatement doc, bool? generateDoc = false)
        {
            if (ModelState.IsValid)
            {
                await SaveEdit(doc);
                if(Message!="")
                {
                    ViewBag.message = Message;
                    return View(doc);
                }

                if((bool)generateDoc)
                {
                   await generateDocument(doc.RegistrationID);
                }

                return Redirect("/create/edit?regId=" + doc.RegistrationID+"&message=Сохранено!");
            }
            return View(doc);
        }

        private string Message = "";
        public async Task<createStatement> SaveEdit(createStatement doc)
        {

            var NameZpDop = doc.NameZpDop;
            var NameDocDop = doc.NameDocDop;

            var oldDoc = await _context.Registration.Include(x => x.suspend).Include(x => x.DocRegistry).Include(x => x.sol).FirstAsync(x => x.RegistrationID == doc.RegistrationID);
            if (User.IsInRole("registrator2") && User.Identity.Name != oldDoc.Registrator)
            {
                Message = "Вы можете редактировать только свои записи!";
                foreach (var props in oldDoc.GetType().GetProperties())
                {
                    if (doc.GetType().GetProperties().Any(x => x.Name == props.Name))
                    {
                        props.SetValue(doc, props.GetValue(oldDoc), null);
                    }
                }
                return doc;
            }
            if (oldDoc.State == 4 && (User.IsInRole("registrator2") || User.IsInRole("registrator3")))
            {
                Message = "Вы не можете редактировать эту запись!";
                foreach (var props in oldDoc.GetType().GetProperties())
                {
                    if (doc.GetType().GetProperties().Any(x => x.Name == props.Name))
                    {
                        props.SetValue(doc, props.GetValue(oldDoc), null);
                    }
                }
                return doc;
            }
            if (oldDoc.DocRegistry==null)
            {
                Message = "Процедура не найдена!!!";                
                return doc;
            }
            oldDoc.StatementForm = doc.StatementForm;
            oldDoc.OrgName = doc.OrgName;
            oldDoc.Organiz = doc.Organiz;
            oldDoc.PerformerName = _context.Performers.FindAsync(doc.Organiz).Result.FIO;
            oldDoc.RegID = doc.RegID;
            oldDoc.Number = doc.Number;
            oldDoc.RegName = doc.RegName;
            oldDoc.KolList = doc.KolList;
            oldDoc.KolListPril = doc.KolListPril;
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            oldDoc.FName = ti.ToTitleCase(doc.FName);
            oldDoc.LName = ti.ToTitleCase(doc.LName);
            oldDoc.MName = doc.MName == null ? string.Empty : ti.ToTitleCase(doc.MName);
            oldDoc.PhoneNo = doc.PhoneNo != null ? doc.PhoneNo : string.Empty;
            oldDoc.PassportNo = doc.PassportNo != null ? doc.PassportNo.ToUpper() : null;
            oldDoc.PersonalNo = doc.PersonalNo != null ? doc.PersonalNo.ToUpper() : string.Empty;
            oldDoc.PassIssuerDate = doc.PassIssuerDate;
            oldDoc.PassIssuer = doc.PassIssuer;
            oldDoc.City = doc.City;
            oldDoc.Address = doc.Address;
            oldDoc.Home = doc.Home;
            oldDoc.NameDoc_Dop = doc.NameDoc_Dop;
            oldDoc.NameZp_Dop = doc.NameZp_Dop;
            oldDoc.LoListCase = doc.LoListCase;
            oldDoc.Flat = doc.Flat == null ? string.Empty : doc.Flat;
            oldDoc.Notes = doc.Notes == null ? string.Empty : doc.Notes;
            oldDoc.MobPhone = doc.MobPhone == null ? string.Empty : doc.MobPhone;
            oldDoc.Proceedings = doc.Proceedings == null ? string.Empty : doc.Proceedings;
            oldDoc.e_mail = doc.e_mail == null ? string.Empty : doc.e_mail;
            oldDoc.Vid = doc.Vid;

            if (doc.NameDocDop != null)
            {
                if (await _context.SelectDocs.AnyAsync(x => x.RegId == doc.RegistrationID))
                {
                    var selDoc = await _context.SelectDocs.Where(x => x.RegId == doc.RegistrationID).ToListAsync();

                    _context.SelectDocs.RemoveRange(selDoc.Where(x => !doc.NameDocDop.Contains((Guid)x.DocId)));

                    doc.NameDocDop = doc.NameDocDop.Where(x => !selDoc.Select(s => s.DocId).Contains(x)).ToList();

                }
                await _context.SelectDocs.AddRangeAsync(doc.NameDocDop.Select(x => new selectDocsModel { Id = Guid.NewGuid(), RegId = doc.RegistrationID, DocId = x }));
            }

            //проверка выбраны ли все документы для запроса
            int countZaprDoc = await _context.BufOrgsZApr.CountAsync(p => p.RegID == doc.RegID);
            bool FullZaprDoc = false;
            //int countSelectZaprDoc = await _context.SelectZaprDocs.CountAsync(p => p.RegId == doc.RegistrationID);
            if (doc.NameZpDop != null && doc.NameZpDop.Count == countZaprDoc && !doc.extendSrok)
                FullZaprDoc = true;
            if (countZaprDoc == 0)
                FullZaprDoc = true;
            if (doc.NameZpDop != null)
            {
                if (await _context.SelectZaprDocs.AnyAsync(x => x.RegId == doc.RegistrationID))
                {
                    var selDoc = await _context.SelectZaprDocs.Where(x => x.RegId == doc.RegistrationID).ToListAsync();

                    _context.SelectZaprDocs.RemoveRange(selDoc.Where(x => !doc.NameZpDop.Contains((Guid)x.ZaprDocId)));

                    doc.NameZpDop = doc.NameZpDop.Where(x => !selDoc.Select(s => s.ZaprDocId).Contains(x)).ToList();
                }

                await _context.SelectZaprDocs.AddRangeAsync(doc.NameZpDop.Select(x => new selectZaprDocsModel { Id = Guid.NewGuid(), RegId = doc.RegistrationID, ZaprDocId = x }));
            }

            await _context.SaveChangesAsync();

            if (!oldDoc.suspend.Any(x => x.beginDate != null))//если не было остановки, то дату исполнения можно изменить.
            {
                int TypeIssue = 0;               
                if (oldDoc.GettingDate.Value.DayOfWeek == DayOfWeek.Saturday && oldDoc.DocRegistry.IssueTerms.Value == 0)
                {
                    oldDoc.MustBeReady = oldDoc.GettingDate.Value.Date.AddHours(23);
                }
                else
                {                   
                    if (!string.IsNullOrEmpty(oldDoc.DocRegistry.TypeIssue) && oldDoc.DocRegistry.TypeIssue.Replace(" ", "") != "")
                        TypeIssue = Convert.ToInt32(oldDoc.DocRegistry.TypeIssue.Replace(" ", ""));

                    int count = oldDoc.DocRegistry.IssueZapr.Value;
                    int typeSrok = oldDoc.DocRegistry.TypeIssueZapr.Value;

                    if (((count == 0 && oldDoc.DocRegistry.Issue == 1) || FullZaprDoc) && !doc.extendSrok)
                        oldDoc.MustBeReady = await _w.GetDay(oldDoc.DocRegistry.IssueTerms.Value, TypeIssue/*doc.TypeIssue*/, oldDoc.GettingDate.Value);
                    else
                    {
                        if (doc.extendSrok)
                        {
                            if (count == 0 && (new int[] { 3, 7 }).Contains(typeSrok))
                                count = 1;
                            else if (count == 0)
                            {
                                count = oldDoc.DocRegistry.IssueTerms.Value;
                                typeSrok = TypeIssue;
                            }
                        }
                        oldDoc.MustBeReady = await _w.GetDay(count, typeSrok, oldDoc.GettingDate.Value);
                    }

                }
            }

            if (oldDoc.DocNo == null)
            {
                var NumberOrder = await _context.Registration.CountAsync(x => x.OrderNo <= oldDoc.OrderNo && x.GettingDate.Value.Year == oldDoc.GettingDate.Value.Year && x.TypeReg == oldDoc.TypeReg);

                if (oldDoc.TypeReg == 1)
                    NumberOrder += _context.Settings.OrderBy(x => x.ID).First().StartDocNoFis != null ? _context.Settings.OrderBy(x => x.ID).First().StartDocNoFis.Value : 0;
                if (oldDoc.TypeReg == 2)
                    NumberOrder += _context.Settings.OrderBy(x => x.ID).First().StartDocNoYur != null ? _context.Settings.OrderBy(x => x.ID).First().StartDocNoYur.Value : 0;
                oldDoc.DocNo = NumberOrder;
            }

            _context.Registration.Update(oldDoc);
            await _context.SaveChangesAsync();

            foreach (var props in oldDoc.GetType().GetProperties())
            {
                if (doc.GetType().GetProperties().Any(x => x.Name == props.Name))
                {
                    props.SetValue(doc, props.GetValue(oldDoc), null);
                }
            }

            doc.NameDocDop = NameDocDop;
            doc.NameZpDop = NameZpDop;

            return doc;
        }

        [HttpPost]
        public async Task<bool> generateDocument(Guid regId)
        {
            if (await _context.Registration.AnyAsync(x => x.RegistrationID == regId))
            {
                getUrlFile getUrl = new getUrlFile();

                string nameDirTemp = getUrl.urlFile() + "TemplateZayav/";
                string nameDirNewAp = getUrl.urlFile() + "WordsForRegisrtations/";
                string nameTempRK = getUrl.urlFileGeneral() + "Template/Регистрационная_карта.docx";
                var doc = await _context.Registration.Include(x => x.DocRegistry).FirstAsync(x => x.RegistrationID == regId);

                if (doc.UrlZayav != null)
                {
                    if (System.IO.File.Exists(doc.UrlZayav))
                    {
                        System.IO.File.Delete(doc.UrlZayav);
                    }
                }


                string urlRk = nameDirNewAp + doc.RegistrationID.ToString().Replace("-", "_") + "_РК.docx";

                if (System.IO.File.Exists(urlRk))
                {
                    System.IO.File.Delete(urlRk);
                }

                doc.URLKartReg = null;
                doc.UrlZayav = null;
                if (System.IO.File.Exists(nameTempRK))
                {
                    System.IO.File.Copy(nameTempRK, urlRk);
                    doc.URLKartReg = await createRk(urlRk, doc);
                }

                string nameTempZayv = doc.DocRegistry.URL;
                if (nameTempZayv != null)
                {
                    urlRk = nameDirNewAp + doc.RegistrationID.ToString() + "_" + nameTempZayv.Replace(' ', '_').Replace(',', '_');


                    if (System.IO.File.Exists(urlRk))
                    {
                        System.IO.File.Delete(urlRk);
                    }

                    if (System.IO.File.Exists(nameDirTemp + nameTempZayv))
                    {
                        System.IO.File.Copy(nameDirTemp + nameTempZayv, urlRk);
                        doc.UrlZayav = await createRk(urlRk, doc);
                    }
                }
                _context.Registration.Update(doc);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        /// <summary>
        /// создаем регистрационную карту word
        /// </summary>
        /// <param name="url"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        private async Task<string> createRk(string url, registrationModel doc)
        {
            createDocument createWord = new();
            return createWord.createWord(url, await replaceDictionary(doc));
        }

        /// <summary>
        /// словарь значений для перезаписи
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        private async Task<Dictionary<string, string>> replaceDictionary(registrationModel registration)
        {
            var setting = await _context.Settings.OrderBy(x=>x.ID).FirstAsync();

            performerModel performerName = new();
            infoFlatModel infoFlat = await _context.InfoFlat.OrderBy(x=>x.Id).FirstOrDefaultAsync(x => x.Registration_Id == registration.RegistrationID);
            if (registration.Organiz != null)
            {
                performerName = await _context.Performers.Include(x => x.Department).ThenInclude(x => x.Curators).OrderBy(x=>x.Id).FirstOrDefaultAsync(x => x.Id == registration.Organiz);
            }
            var sol = await _context.Solutions.OrderBy(x=>x.Id).FirstOrDefaultAsync(x => x.RegistrationId == registration.RegistrationID);
            var f = await _context.Family.OrderBy(x=>x.LName).FirstOrDefaultAsync(x => x.RegistrationID == registration.RegistrationID);
            if (f == null)
                f = new();
            string srokZapr = "";

            if (registration.DocRegistry.IssueZapr != null && registration.DocRegistry.IssueZapr > 0)
            {
                var l = await new getList(_context).isssueList();
                var t = await new getList(_context).TypeIssueList();
                srokZapr = " (" + l.First(x=>x.Value==registration.DocRegistry.Issue.ToString()).Text + " " + registration.DocRegistry.IssueZapr + " " + t.First(x => x.Value == registration.DocRegistry.TypeIssueZapr.ToString()).Text + ")";
            }

            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {                
                { "LNameOfFamily", f.LName},
                { "MNameOfFamily", f.MName},
                { "FNameOfFamily", f.FName},
                { "NRotN", f.NRotN},
                { "DOBOfFamily", f.DOB != null ? f.DOB.Value.ToString("d") : ""},
                {"DocNo", registration.DocNo.ToString()},
                {"LName", registration.LName},
                {"MName", registration.MName},
                {"FName", registration.FName},
                {"Organiz", registration.OrgName != null ? registration.OrgName.Replace("\"", "&quot;") : ""},
                {"Isp_address", performerName.Address!=null?performerName.Address:""},
                {"Address",(registration.City!=null? registration.City.Replace("\"", "&quot;"):"") + ", " + registration.Address + ", д." + registration.Home + ", кв." + registration.Flat},
                {"PhoneNoM", (registration.MobPhone != "" ? ", моб.тел. " + registration.MobPhone : "") +(registration.e_mail != "" ? ", e-mail " + registration.e_mail : "")},
                {"DeptPhoneNo", performerName.Phone!=null?performerName.Phone.Replace(" ", ""):""},
                {"PhoneNo_M", registration.MobPhone},
                {"PhoneNO_M", registration.MobPhone},
                {"PhoneNo", registration.PhoneNo},
                {"PhoneNO", registration.PhoneNo},
                {"StatementForm", registration.StatementForm},
                {"GettingDate", registration.GettingDate.Value.ToShortDateString()},
                {"kollist", registration.KolList != null ? registration.KolList.ToString() : ""}, //?
                {"kolListPril", registration.KolListPril != null ? registration.KolListPril.ToString() : ""}, //?
                {"RegName", Char.IsUpper(registration.DocRegistry.RegName.Trim()[0])? registration.DocRegistry.Number + " " + registration.DocRegistry.RegName.Replace("\"", "&quot;") :registration.DocRegistry.Number+ " " + _context.Sections.Find(registration.DocRegistry.SectionID).Name.Replace("\"", "&quot;") + " " + registration.DocRegistry.RegName.Replace("\"", "&quot;")},
                {"DeptName",performerName.FIO!=null? performerName.FIO.Replace("\"", "&quot;"):""},
                {"Proceedings", registration.Proceedings!=null? registration.Proceedings.Replace("\"", "&quot;"):""},
                {"LongReadyDate",registration.DocRegistry.IssueTerms!=0? ("Максимальный срок получения итогового документа - " + ((registration.GettingDate.Value.Date>=new DateTime(2017, 7, 15).Date&&registration.MustBeReady.Value.Date!=registration.GettingDate.Value.Date)? _w.GetDay(1, 2, registration.MustBeReady.Value).Result.Date.ToString("d"):registration.MustBeReady.Value.Date.ToString("d"))+srokZapr):""},
                { "MustBeReadyDate", registration.MustBeReady.Value.ToShortDateString()+ srokZapr},
                {"DateSsolutions",sol.dateOfSolution != null ? sol.dateOfSolution.Value.ToShortDateString() : ""},
                {"DateRech",sol.dateOfSolution != null ? sol.dateOfSolution.Value.ToShortDateString() : ""},
                {"PassIssueDate", registration.PassIssuerDate != null ? registration.PassIssuerDate.Value.ToString("d") : ""},
                {"PassIssuer", registration.PassIssuer != null ? registration.PassIssuer.ToString() : ""},
                {"NamberSolutions", sol.solutionNumber},
                {"NRech", sol.solutionNumber},
                {"ResultType", sol.solution},
                {"RezRech", sol.solution},
                {"OutDeptDate",registration.OutDeptDate != null ? registration.OutDeptDate.Value.ToShortDateString() : ""},
                {"IssueDate", registration.IssueDate != null ? registration.IssueDate.Value.ToShortDateString() : ""},
                {"EvaluationNotificati", registration.EvaluationNotification}, // Имя поля в ворде имеет ограничение по количеству символов 
                {"CaseNamber", registration.CaseNamber},
                {"LoListCase", registration.LoListCase != null ? registration.LoListCase.ToString() : ""},
                {"NotificationEmail", !String.IsNullOrEmpty(registration.e_mail) ? "Сообщение о рассмотрении вашего заявления придет на адрес электронной почты " +registration.e_mail: ""},
                {"Isp_dol",performerName.Title != null ? performerName.Title.Replace("\"", "&quot;") : ""},
                {"Isp_tel", performerName.Phone},
                {"Isp_prim",performerName.Notes!=null? performerName.Notes.Replace("\"", "&quot;"):""},
                {"Isp_kab",performerName.Cabinet},
                {"Isp",performerName.FIO!=null?performerName.FIO.Replace("\"", "&quot;"):""},
                {"Phone_ОО",setting.Phone},
                {"Worker",registration.Registrator!=null? registration.Registrator.Replace("\"", "&quot;"):""},
                {"FIO", registration.LName + " " + registration.FName + " " + registration.MName},
                {"cbStreet",registration.Address!=null? registration.Address.Replace("\"", "&quot;"):""},
                {"cbHouse", registration.Home},
                {"cbFlat", registration.Flat},
                {"dptDay", registration.GettingDate != null ? registration.GettingDate.Value.Day.ToString() : ""},
                {"dptMonth", registration.GettingDate != null ? registration.GettingDate.Value.Month.ToString() : ""},
                {"dptYear", registration.GettingDate != null ? registration.GettingDate.Value.Year.ToString() : ""},
                {"PassportNo", registration.PassportNo != null ? registration.PassportNo.ToString() : ""},
                {"PersonalNo", registration.PersonalNo != null ? registration.PersonalNo.ToString() : ""},
                {"dtpDOB", GetDateOfBorn(registration.PersonalNo)},// Получаем дату рождения из личного номера паспорта
                {"DOB", GetDateOfBorn(registration.PersonalNo)},
                {"NPrav", infoFlat != null ? infoFlat.Pravo : ""},
                {"PlO", infoFlat != null ? infoFlat.totalArea.ToString() : ""},
                {"Notes", registration.Notes != null ? registration.Notes.Replace("\"", "&quot;") : ""},
                {"SRechUv", registration.EvaluationNotification},
                {"NDeloArhiv", registration.CaseNamber},
                {"LDeloArhiv", registration.LoListCase.ToString()}, //?
                {"HodRa",registration.Proceedings!=null? registration.Proceedings.Replace("\"", "&quot;"):""},
                {"FPZa", registration.StatementForm},
                {"FIOKur",performerName.Department.Curators.FIO!=null? performerName.Department.Curators.FIO.Replace("\"", "&quot;"):""},
                {"Otd_isp",performerName.Department.Name!=null? performerName.Department.Name.Replace("\"", "&quot;"):""},
                {"dtpCurrDate", DateTime.Now.ToString("d")},
                {"dtpday", DateTime.Now.Day.ToString("d")},
                {"dtpmonth", DateTime.Now.Month.ToString("d")},
                {"dtpyear", DateTime.Now.Year.ToString("d")},
                {"dtpMonth", DateTime.Now.Month.ToString("d")},
                {"dtpYear", DateTime.Now.Year.ToString("d")},
                {"NameDoc_ZA", _context.SelectDocs.Any(x=>x.RegId==registration.RegistrationID)? String.Join("\n", _context.SelectDocs.Where(x=>x.RegId==registration.RegistrationID).Select(x=> x.docs.Name)):"" },
                {"NumberOfLivingPersons",infoFlat != null ? infoFlat.personNumber.ToString():"" },
                {"e_mail", registration.e_mail },
                {"AdministrationName",setting.Name!=null? setting.Name.Replace("\"", "&quot;") :""},
                {"AdministrationAdres", _context.Curators.FirstOrDefault(x=>x.Areas.Number==setting.ValueArea).Address },
                {"Name_Zp", _context.SelectZaprDocs.Any(x=>x.RegId==registration.RegistrationID)? String.Join(";", _context.SelectZaprDocs.Where(x=>x.RegId==registration.RegistrationID).Select(x=> x.zaprDoc.Name)):""},
                {"NameDoc_Dop", registration.NameDoc_Dop!=null? registration.NameDoc_Dop.Replace("\"", "&quot;"):""},
                {"NameZp_Dop", registration.NameZp_Dop!=null? registration.NameZp_Dop.Replace("\"", "&quot;"):""},
                { "dtpTime",registration.GettingDate != null? registration.GettingDate.Value.ToLongTimeString(): String.Empty }
            };

            String familyMembersData = String.Empty;
            var family = await _context.Family.Where(x => x.RegistrationID == registration.RegistrationID).ToListAsync();
            int i = 2;
            foreach (var c in family)
            {
                familyMembersData += String.Format(i+". – {0} {1} {2} {3} {4}",
                    c.NRotN != null ? c.NRotN.Trim() : String.Empty,
                    c.LName != null ? c.LName.Trim() : String.Empty,
                    c.FName != null ? c.FName.Trim() : String.Empty,
                    c.MName != null ? c.MName.Trim() : String.Empty,
                    (c.DOB != null) ? (c.DOB.Value.ToString("d") + " г.р.") : String.Empty) + "<w:br />";
                i++;
            }
            dictionary.Add("CHL_S", familyMembersData);
            return dictionary;
        }

        /// <summary>
        /// Получаем дату рождения из личного номера паспорта
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        string GetDateOfBorn(string PersonalNo)
        {
            if (!String.IsNullOrEmpty(PersonalNo) && !String.IsNullOrWhiteSpace(PersonalNo) && PersonalNo.Length > 6)
            {
                Boolean isPersonalNoCorrect = (Char.IsDigit(PersonalNo, 0) &&
                                               Char.IsDigit(PersonalNo, 1) &&
                                               Char.IsDigit(PersonalNo, 2) &&
                                               Char.IsDigit(PersonalNo, 3) &&
                                               Char.IsDigit(PersonalNo, 4) &&
                                               Char.IsDigit(PersonalNo, 5) &&
                                               Char.IsDigit(PersonalNo, 6)) ? true : false;
                Boolean isValidTypeOfPersonalNo = (PersonalNo[0] != '0' &&
                                                   PersonalNo[0] != '7' &&
                                                   PersonalNo[0] != '8' &&
                                                   PersonalNo[0] != '9') ? true : false;
                if (isPersonalNoCorrect && isValidTypeOfPersonalNo)
                {
                    if (PersonalNo[0] == '1' || PersonalNo[0] == '2')
                        return String.Format("{0}{1}.{2}{3}.18{4}{5}", PersonalNo[1],
                            PersonalNo[2], PersonalNo[3], PersonalNo[4],
                            PersonalNo[5], PersonalNo[6]);

                    if (PersonalNo[0] == '3' || PersonalNo[0] == '4')
                        return String.Format("{0}{1}.{2}{3}.19{4}{5}", PersonalNo[1],
                            PersonalNo[2], PersonalNo[3], PersonalNo[4],
                            PersonalNo[5], PersonalNo[6]);

                    if (PersonalNo[0] == '5' || PersonalNo[0] == '6')
                        return String.Format("{0}{1}.{2}{3}.20{4}{5}", PersonalNo[1],
                            PersonalNo[2], PersonalNo[3], PersonalNo[4],
                            PersonalNo[5], PersonalNo[6]);
                }
                else return "ВВЕДИТЕ ДАТУ РОЖДЕНИЯ В РУЧНУЮ";
            }
            return "";
        }

        /// <summary>
        /// загружаем список процедур
        /// </summary>
        /// <param name="legalStr"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult viewAdminProc(string legalStr = "false")
        {
            getAdminProc proc = new getAdminProc(_context);
            return PartialView(proc.getList(legalStr, true));
        }

        /// <summary>
        /// загружаем информацию по процедуре
        /// </summary>
        /// <param name="RegId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> getAdminProcInfo(Guid RegId)
        {
            if (!await _context.DocRegistry.AnyAsync(x => x.RegID == RegId))
                return BadRequest("Нет такой процедуры");
            return PartialView(await _context.DocRegistry.FindAsync(RegId));
        }

        /// <summary>
        /// загружаем список документов для процедуры
        /// </summary>
        /// <param name="RegId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult getDocForProc(Guid RegId)
        {
            return PartialView(new docZapr { regId = RegId });
        }

        /// <summary>
        /// загружаем список запросов для процедуры
        /// </summary>
        /// <param name="RegId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult getZaprForProc(Guid RegId)
        {
            return PartialView(new docZapr { regId = RegId });

        }

        /// <summary>
        /// загружаем список запросов для процедуры
        /// </summary>
        /// <param name="RegId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult loadAttachFile(Guid RegId)
        {
            return PartialView(RegId);
        }

        [HttpPost]
        public IActionResult loginEsiful()
        {
            string data = JsonConvert.SerializeObject(new
            {
                settings = settingsForEsiful()
            });

            string respons = callEsiful("http://192.168.209.205:13000/api/v2/login", data);

            dynamic s = JsonConvert.DeserializeObject(respons, typeof(object));
            if (s == null)
                return BadRequest("нет ответа от сервера!!!");
            return Json(JsonConvert.SerializeObject(new { signed = s.signed_data_to_check_in_cp, codeVerifer = s.code_verifier, enveloped = s.enveloped_and_signed_auth_url }));

        }

        [HttpPost]
        public IActionResult Bauth()
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult addSession(string codeVerifier)
        {
            Response.Cookies.Append("codeVerifier", codeVerifier);
            return Ok();
        }

        public async Task<createStatement> loginCallback(string dataEsiful, string codeVerifer)
        {
            createStatement doc = new();
            string data = JsonConvert.SerializeObject(new
            {
                @params = dataEsiful,
                code_verifier = codeVerifer,
                settings = settingsForEsiful()
            });

            string respons = callEsiful("http://192.168.209.205:13000/api/v2/login_callback", data);

            dynamic s = JsonConvert.DeserializeObject(respons, typeof(object));
            if (s == null)
                return doc;

            Response.Cookies.Append("id_token", s.id_token.Value);

            if (s.userinfo == null)
                return doc;

            string adres = s.userinfo.residence_place != null ? s.userinfo.residence_place.full_address.Value : "";
            if (adres != "")
            {
                await _acontext.fullAdres.AddAsync(new Models.generalModel.fullAdresModel { fullAdres = JsonConvert.SerializeObject(s.userinfo) });
                await _acontext.SaveChangesAsync();
            }
            ViewBag.FullAdress = adres;
            string[] arrAdres = adres.Split(", ");


            doc.FName = s.userinfo.givenName != null ? s.userinfo.name.Value : "";
            doc.LName = s.userinfo.givenName != null ? s.userinfo.surname.Value : "";
            doc.MName = s.userinfo.givenName != null ? s.userinfo.givenName.Value : "";
            doc.PersonalNo = s.userinfo.givenName != null ? s.userinfo.serialNumber.Value : "";

            if (arrAdres.Any())
            {
                var city = arrAdres[0];
                var street = arrAdres[1] != null ? (arrAdres[1].TrimStart(' ').TrimEnd(' ').Split(" ")[1] + " " + arrAdres[1].TrimStart(' ').TrimEnd(' ').Split(" ")[0]) : "";
                var ndom = arrAdres[2] != null ? arrAdres[2].TrimStart(' ').TrimEnd(' ').Split(" ")[1] : "";
                var flat = arrAdres[3] != null ? arrAdres[3].TrimStart(' ').TrimEnd(' ').Split(" ")[1] : "";

                doc.City = city;
                doc.Address = street;
                doc.Home = ndom;
                doc.Flat = flat;
            }

            logoutEsiful();

            return doc;


        }

        public string logoutEsiful()
        {
            try
            {

                if (Request.Cookies.ContainsKey("id_token"))
                {
                    string urlLogout = "https://esiful.gov.by/session/end?id_token_hint=" + Request.Cookies["id_token"].ToString() + "&post_logout_redirect_uri=" + urlRedirect;

                    string data = JsonConvert.SerializeObject(new
                    {
                        id_token = Request.Cookies["id_token"].ToString(),
                        authority = "https://esiful.gov.by",
                        post_logout_redirect_uri = urlRedirect
                    });

                    callEsiful("http://192.168.209.205:13000/api/v2/logout", data);


                    return urlLogout;
                }
            }
            finally
            {
                Response.Cookies.Delete("id_token");
                Response.Cookies.Delete("codeVerifier");
            }
            return "";
        }

        [HttpPost]
        public string callEsiful(string urlCall, string dataCall)
        {
            string respons = "";
            Uri url = new Uri(urlCall);
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "POST"; // для отправки используется метод Post

            httpWebRequest.ContentType = "application/json";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(dataCall);
            }


            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                respons = streamReader.ReadToEnd();
            }

            return respons;
        }

        public object settingsForEsiful()
        {
            return new
            {
                authority = "https://esiful.gov.by",
                client_id = "Qo03MvBlGHkfETKGO209WZBK5QuHXBu63_x3ZsC21OU",
                grant_type = "authorization_code",
                response_type = "code",
                scope = "openid profile offline_access residence_place",
                post_logout_redirect_uri = urlRedirect,
                redirect_uri = urlRedirect,
                prompt = "consent",
                state = urlRedirect
            };
        }

        public string urlRedirect
        {
            get
            {
                //return "http://192.168.209.232/create";
                return "http://" + Request.Host + "/create";
            }
        }

    }
}
