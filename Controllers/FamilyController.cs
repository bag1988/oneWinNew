using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using oneWin.Data;
using Microsoft.EntityFrameworkCore;
using oneWin.Models.baseModel;
using System.Net;
using oneWin.Service;
using oneWin.Models;
using System.Security.Principal;
using System.IO;
using System.Web;
using System.Text;
using System.Globalization;
using oneWin.OfficeCreate;
using Newtonsoft.Json;

namespace oneWin.Controllers
{
    public class FamilyController : Controller
    {
        private oneWinDbContext _context;


        public FamilyController(oneWinDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// главная состав семьи
        /// </summary>
        /// <param name="regId"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(Guid regId)
        {
            if (!await _context.Registration.AnyAsync(x => x.RegistrationID == regId))
                return BadRequest("Нет такой записи!");

            if (!await _context.InfoFlat.AnyAsync(x => x.Registration_Id == regId))
                return View(new infoFlatModel() { Registration_Id = regId });

            return View(await _context.InfoFlat.FirstAsync(x => x.Registration_Id == regId));
        }

        /// <summary>
        /// Главная. Добавить infoFlatModel
        /// </summary>
        /// <param name="flat"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Index(infoFlatModel flat)
        {
            if (!ModelState.IsValid)
                return View(flat);
            if (flat.Id == Guid.Empty)
            {
                flat.Id = Guid.NewGuid();
                _context.InfoFlat.Add(flat);
            }
            else
                _context.InfoFlat.Update(flat);

            await _context.SaveChangesAsync();
            ViewBag.message = "Сохранено!";
            return View(flat);
        }


        /// <summary>
        /// частичное представление отобразить состав семьи
        /// </summary>
        /// <param name="regId"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult loadFamily(Guid regId)
        {
            return PartialView(regId);
        }

        /// <summary>
        /// добавить состав семьи
        /// </summary>
        /// <param name="regId"></param>
        /// <param name="idFamily"></param>
        /// <returns></returns>
        public async Task<IActionResult> Create(Guid regId, Guid idFamily)
        {
            if (!await _context.Registration.AnyAsync(x => x.RegistrationID == regId))
                return BadRequest("Нет такой записи!");

            familyModel family = null;

            if (idFamily != Guid.Empty)
            {
                if (!await _context.Family.AnyAsync(x => x.FamilyID == idFamily))
                    return BadRequest("Нет такой записи!");

                family = await _context.Family.FindAsync(idFamily);
            }
            else
                family = new familyModel() { RegistrationID = regId };

            return View(family);
        }


        /// <summary>
        /// добавить состав семьи
        /// </summary>
        /// <param name="regId"></param>
        /// <param name="family"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(Guid regId, familyModel family)
        {
            if (!await _context.Registration.AnyAsync(x => x.RegistrationID == regId))
                return BadRequest("Нет такой записи!");
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            family.FName = ti.ToTitleCase(family.FName);
            family.LName = ti.ToTitleCase(family.LName);
            family.MName = family.MName == null ? family.MName : ti.ToTitleCase(family.MName);
            family.PassportNo = family.PassportNo!=null?family.PassportNo.ToUpper():null;
            family.PersonalNo = family.PersonalNo!=null?family.PersonalNo.ToUpper():null;
            if (family.FamilyID != Guid.Empty)
            {
                _context.Family.Update(family);
            }
            else
                await _context.Family.AddAsync(family);

            await _context.SaveChangesAsync();

            return Redirect("index?regId=" + regId);

            //ViewBag.message = "Сохранено!";
            //return View(family);
        }

        /// <summary>
        /// Удалить состав семьи
        /// </summary>
        /// <param name="idFamily"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> deleteFamily(Guid idFamily)
        {
            if (!await _context.Family.AnyAsync(x => x.FamilyID == idFamily))
                return BadRequest("Нет такой записи!");

            _context.Family.Remove(_context.Family.Find(idFamily));

            await _context.SaveChangesAsync();

            return Ok();
        }


        /// <summary>
        /// Перенос в запросы
        /// </summary>
        /// <param name="regId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> TransferInRequest(Guid regId)
        {
            if (!await _context.Family.AnyAsync(x => x.RegistrationID == regId))
                return BadRequest("Нет записей для передачи!");

            await _context.MSG.AddRangeAsync(_context.Family.Where(x => x.RegistrationID == regId).Select(x => new msgModel()
            {
                RegistrationId = regId,
                LName = x.LName,
                FName = x.FName,
                MName = x.MName,
                City = x.City,
                Address = x.Address,
                AddressDate = x.AddressDate,
                Home = x.Home,
                Flat = x.Flat,
                DOB = x.DOB,
                DocType = "1",
                TypeMSG = false,
                InterDoc = 1,
                IsSend = false,
                PersonalNo = x.PersonalNo,
                DocIssueDate = x.PassIssuerDate,
                DocNo = x.PassportNo,
                Summ = 0,
                DocIssuer = x.PassIssuer,
                DateofCreatingRequest = DateTime.Now,
                Sent = Guid.NewGuid()
            }));

            await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Главная страница запросов
        /// </summary>
        /// <param name="regId"></param>
        /// <returns></returns>
        public async Task<IActionResult> Transfer(Guid regId)
        {
            if (!await _context.Registration.AnyAsync(x => x.RegistrationID == regId))
                return BadRequest("Нет такой записи!");

            return View(regId);
        }

        /// <summary>
        /// Добавить запрос
        /// </summary>
        /// <param name="regId"></param>
        /// <param name="idMsg"></param>
        /// <returns></returns>
        public async Task<IActionResult> addTransfer(Guid regId, Guid? idMsg = null)
        {
            if (!await _context.Registration.AnyAsync(x => x.RegistrationID == regId))
                return BadRequest("Нет такой записи!");

            if ((idMsg != Guid.Empty || idMsg != null) && await _context.MSG.AnyAsync(x => x.MsgId == idMsg))
            {
                return View(await _context.MSG.Include(x => x.orgsZapr).OrderBy(x=>x.LName).FirstOrDefaultAsync(x => x.MsgId == idMsg));
            }

            var reg = await _context.Registration.FindAsync(regId);

            msgModel msg = new msgModel()
            {
                RegistrationId = regId,
                LName = reg.LName,
                FName = reg.FName,
                MName = reg.MName,
                DocNo = reg.PassportNo,
                PersonalNo = reg.PersonalNo,
                DocIssuer = reg.PassIssuer,
                DocIssueDate = reg.PassIssuerDate,
                DOB = GetDateOfBorn(reg.PersonalNo) != null ? Convert.ToDateTime(GetDateOfBorn(reg.PersonalNo)) : null,
                City = reg.City,
                Address = reg.Address,
                Home = reg.Home,
                Flat = reg.Flat
            };

            
            return View(msg);
        }

        /// <summary>
        /// Добавить запрос
        /// </summary>
        /// <param name="regId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> addTransfer(Guid regId, msgModel msg)
        {
            if (!await _context.Registration.AnyAsync(x => x.RegistrationID == regId))
                return BadRequest("Нет такой записи!");

            msg.orgsZapr = null;

            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            msg.FName = ti.ToTitleCase(msg.FName);
            msg.LName = ti.ToTitleCase(msg.LName);
            msg.MName = msg.MName == null ? msg.MName : ti.ToTitleCase(msg.MName);
            msg.PersonalNo = msg.PersonalNo != null ? msg.PersonalNo.ToUpper() : null;
            msg.DocNo = msg.DocNo != null ? msg.DocNo.ToUpper() : null;
            //запрос(false) согласование(true)
            msg.TypeMSG = false;
            if (msg.DOB == null)
                msg.DOB = GetDateOfBorn(msg.PersonalNo) != null ? Convert.ToDateTime(GetDateOfBorn(msg.PersonalNo)) : null;
            msg.IsSend = false;
            msg.InterDoc = 1;
            msg.DateofCreatingRequest = DateTime.Now;
            if (msg.MsgId != Guid.Empty)
            {
                _context.MSG.Update(msg);
            }
            else
                await _context.MSG.AddAsync(msg);

            await _context.SaveChangesAsync();

            return Redirect("Transfer?regId=" + regId);

            //ViewBag.message = "Сохранено!";

            //return View(msg);
        }


        /// <summary>
        /// Копировать запрос
        /// </summary>
        /// <param name="idMsg"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> copyTransfer(Guid idMsg)
        {
            if (!await _context.MSG.AnyAsync(x => x.MsgId == idMsg))
                return BadRequest("Нет такой записи!");
            var oldMsg = await _context.MSG.FindAsync(idMsg);
            oldMsg.MsgId = Guid.NewGuid();
            await _context.MSG.AddAsync(oldMsg);
            await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Удалить запрос
        /// </summary>
        /// <param name="idMsg"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> deleteTransfer(Guid idMsg)
        {
            if (!await _context.MSG.AnyAsync(x => x.MsgId == idMsg))
                return BadRequest("Нет такой записи!");

            _context.MSG.Remove(_context.MSG.Find(idMsg));

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public IActionResult loadTransfer(Guid regId)
        {
            return PartialView(regId);
        }

        [HttpPost]
        public async Task<IActionResult> createRequest(Guid regId)
        {
            if (!await _context.Registration.AnyAsync(x => x.RegistrationID == regId))
                return BadRequest("Нет такой записи!");


            var msgList = await _context.MSG.Include(x => x.zaprDoc).Include(x => x.orgsZapr).Include(x => x.Registration).Where(x => x.RegistrationId == regId).ToListAsync();

            string message = "";

            foreach (var msg in msgList)
            {
                message += msg.PayNo + " " + msg.LName + " " + msg.FName + " " + msg.MName + " " + await sendRequest(msg) + "<br/>";
            }



            return Ok(message);
        }

        /// <summary>
        /// Отправка одного запроса
        /// </summary>
        /// <param name="idMsg"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> sendOneRequest(Guid idMsg)
        {
            if (!await _context.MSG.AnyAsync(x => x.MsgId == idMsg))
                return BadRequest("Нет такой записи!");

            var msg = await _context.MSG.Include(x => x.zaprDoc).Include(x => x.orgsZapr).Include(x => x.Registration).FirstAsync(x => x.MsgId == idMsg);

            return Ok(await sendRequest(msg));
        }

        /// <summary>
        /// Отправка запроса
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private async Task<string> sendRequest(msgModel msg)
        {
            string message = JsonConvert.SerializeObject(new { message = "Ошибка" });
            bool sendRequest = false;

            if (msg.zaprDoc.HTTPZapr)
            {
                if (msg.File != null)
                {
                    message = JsonConvert.SerializeObject(new { file = msg.File });
                    sendRequest = true;
                }
                else
                {
                    message = JsonConvert.SerializeObject(new { message = "Не отправлено в БРТИ" }); 
                }
            }
            if (msg.zaprDoc.e_mailzapr)
            {
                if (msg.OrganisationID != null && msg.orgsZapr != null && msg.orgsZapr.e_mail != null)
                {
                    var setting = await _context.Settings.FirstAsync();
                    string textMessage = "Запрос по обращению N " + msg.Registration.DocNo + " от " + msg.Registration.GettingDate.Value.ToString("d");
                    string subjectMail = "Запрос от " + setting.Name + " по обращению N " + msg.Registration.DocNo.Value;
                    string emailFrom = setting.MailLoginForNotification;
                    string passFrom = setting.MailPassForNotification;
                    string nameFrom = setting.Name;
                    string emailTo = msg.orgsZapr.e_mail;
                    string nameTo = msg.orgsZapr.Name;
                    sendEmail sendE = new();
                    var boolEmail = await sendE.SendEmailAsync(subjectMail, textMessage, emailFrom, passFrom, nameFrom, emailTo, nameTo, msg.File);

                    if (boolEmail)
                        message = JsonConvert.SerializeObject(new { message = "Сообщение email отправлено!" }); 
                    else
                        message = JsonConvert.SerializeObject(new { message = "Сообщение email не отправлено!" }); 

                    sendRequest = boolEmail;
                }
                else
                    message = JsonConvert.SerializeObject(new { message = "Сообщение email не может быть отправлено!" }); 
            }

            if (msg.zaprDoc.bankzapr)
            {
                var doc = msg.zaprDoc;

                var setting = await _context.Settings.FirstAsync();

                string nameFile = doc.BankName.Replace("[ID]", (msg.Registration.OrderNo + "_" + msg.PayNo));


                string paramsBank = doc.BankParams;

                paramsBank = paramsBank.Replace("[LName]", msg.LName);
                paramsBank = paramsBank.Replace("[FName]", msg.FName);
                paramsBank = paramsBank.Replace("[MName]", msg.MName ?? String.Empty);
                paramsBank = paramsBank.Replace("[DOB]", msg.DOB != null ? msg.DOB.Value.ToString("d") : "");
                if (msg.DocType == "1" || msg.DocType == "8")
                {
                    paramsBank = paramsBank.Replace("[PersonalNo]", msg.PersonalNo);
                    paramsBank = paramsBank.Replace("[PassportSeries]", msg.DocNo.Length > 2 ? msg.DocNo.Substring(0, 2) : msg.DocNo);
                    paramsBank = paramsBank.Replace("[PassportNumber]", msg.DocNo.Length > 2 ? msg.DocNo.Substring(2, msg.DocNo.Length - 2) : msg.DocNo);
                    paramsBank = paramsBank.Replace("[DocIssuer]", msg.DocIssuer);
                    paramsBank = paramsBank.Replace("[DocIssueDate]", msg.DocIssueDate != null ? msg.DocIssueDate.Value.ToString("d") : "");
                    if (msg.DocType == "1")
                        paramsBank = paramsBank.Replace("[C01]", "паспорт гражданина РБ");
                    if (msg.DocType == "8")
                        paramsBank = paramsBank.Replace("[C01]", "вид на жительство в РБ");
                }
                else
                {
                    paramsBank = paramsBank.Replace("[PersonalNo]", String.Empty);
                    paramsBank = paramsBank.Replace("[PassportSeries]", String.Empty);
                    paramsBank = paramsBank.Replace("[PassportNumber]", String.Empty);
                    paramsBank = paramsBank.Replace("[DocIssuer]", String.Empty);
                    paramsBank = paramsBank.Replace("[DocIssueDate]", String.Empty);
                    paramsBank = paramsBank.Replace("[C01]", String.Empty);

                }

                paramsBank = paramsBank.Replace("[City]", msg.City);
                paramsBank = paramsBank.Replace("[Street]", GetStreet(msg.Address));
                paramsBank = paramsBank.Replace("[Tip]", GetTip(msg.Address));
                paramsBank = paramsBank.Replace("[House]", House(msg.Home));
                paramsBank = paramsBank.Replace("[Ind]", Ind(msg.Home));
                paramsBank = paramsBank.Replace("[Korp]", Korp(msg.Home));
                paramsBank = paramsBank.Replace("[Flat]", Flat(msg.Flat));
                paramsBank = paramsBank.Replace("[Index]", GetIndex(msg.Flat));
                paramsBank = paramsBank.Replace("[Address]", msg.City + msg.Address + "д." + msg.Home + "кв." + msg.Flat);
                paramsBank = paramsBank.Replace("[Bank]", msg.Bank);
                paramsBank = paramsBank.Replace("[GettingDate]", msg.Registration.GettingDate != null ? msg.Registration.GettingDate.Value.ToString("d") : "");
                paramsBank = paramsBank.Replace("[OrderNo]", msg.Registration.OrderNo.ToString());
                paramsBank = paramsBank.Replace("<01>1", "<A01>1");
                paramsBank = paramsBank.Replace("[ID]", msg.Registration.OrderNo.ToString());
                paramsBank = paramsBank.Replace("[Now]", DateTime.Now.ToString("d"));
                paramsBank = paramsBank.Replace("[Phone]", setting.Phone);
                paramsBank = paramsBank.Replace("[Worker]", msg.Registration.Registrator);
                paramsBank = paramsBank.Replace("[DogNo]", msg.DogNo);

                if (setting.ValueArea != null)
                    paramsBank = paramsBank.Replace("[kod]", setting.ValueArea.Value.ToString());



                using (ImpersonateUser wi = new ImpersonateUser("usermgaon", _context.Settings.First().PathGP, "1812mgaon"))
                {

#pragma warning disable CA1416 // Проверка совместимости платформы
                    WindowsIdentity.RunImpersonated(wi.Identity.AccessToken, () =>
                    {
                        using (var writer = new StreamWriter(System.IO.File.Create(nameFile)))
                        {
                            writer.WriteLine(paramsBank);
                        }
                        //System.IO.File.Delete(nameFile);

                    });
#pragma warning restore CA1416 // Проверка совместимости платформы
                }
                message = JsonConvert.SerializeObject(new { message = "Отправлено в банк!" }); 
                sendRequest = true;
            }

            if (sendRequest)
            {
                msg.IsSend = true;
                _context.MSG.Update(msg);
                msg.Registration.Proceedings += "\n" + msg.zaprDoc.Name + " " + DateTime.Now.ToString("d");
            }
            else
                msg.Registration.Proceedings += "\nЗапрос не отправлен\n" + msg.zaprDoc.Name + " " + DateTime.Now.ToString("d");

            _context.Registration.Update(msg.Registration);
            await _context.SaveChangesAsync();

            return message;
        }


        public async Task<IActionResult> generateTransfer(Guid regId, bool fullFamily = false, bool calculate = false)
        {
            var msgList = await _context.MSG.Include(x => x.zaprDoc).Include(x => x.orgsZapr).Include(x => x.Registration)
               .Where(x => x.RegistrationId == regId).ToListAsync();
            if (calculate)
                await calculateSumm(msgList.Where(x => x.zaprDoc != null && x.zaprDoc.HTTPZapr).OrderByDescending(x => x.Summ).ToList());

            await generateFile(msgList.Where(x =>x.zaprDoc!=null && x.zaprDoc.e_mailzapr).ToList(), fullFamily);

            await generateFile(msgList.Where(x => x.zaprDoc != null && x.zaprDoc.postzapr).ToList(), fullFamily);

            await generateRequest(msgList.Where(x => x.zaprDoc != null && x.zaprDoc.HTTPZapr).OrderByDescending(x => x.Summ).ToList());

            return Ok();
        }

        /// <summary>
        /// Формируем запрос
        /// </summary>
        /// <param name="regId"></param>
        /// <returns></returns>
        public async Task<IActionResult> generateRequest(List<msgModel> msgList)
        {
            if (!msgList.Any())
                return Ok();

            string DocType = "", DocNo = "", DocIssuer = "", DocIssueDate = "", PersonalNo = "", LName = "", FName = "", MName = "", UlTip = "", City = "", PhoneNO = "",
                    Street = "", Houses = "", Korps = "", Inds = "", Flats = "", Indexs = "", DOB = "", AddressDate = "";

            var setting = await _context.Settings.FirstAsync();

            string HTTP = msgList.First().zaprDoc.HTTP;
            string Params = msgList.First().zaprDoc.Params;


            DocType = string.Join("||", msgList.Select(x => HttpUtility.UrlEncode(x.DocType, Encoding.GetEncoding(1251))));
            DocNo = string.Join("||", msgList.Select(x => HttpUtility.UrlEncode(x.DocNo, Encoding.GetEncoding(1251))));
            DocIssuer = string.Join("||", msgList.Select(x => HttpUtility.UrlEncode(x.DocIssuer, Encoding.GetEncoding(1251))));
            DocIssueDate = string.Join("||", msgList.Select(x => HttpUtility.UrlEncode((x.DocIssueDate != null ? x.DocIssueDate.Value.ToString("d") : ""), Encoding.GetEncoding(1251))));
            PersonalNo = string.Join("||", msgList.Select(x => HttpUtility.UrlEncode(x.PersonalNo, Encoding.GetEncoding(1251))));
            LName = string.Join("||", msgList.Select(x => HttpUtility.UrlEncode(x.LName, Encoding.GetEncoding(1251))));
            FName = string.Join("||", msgList.Select(x => HttpUtility.UrlEncode(x.FName, Encoding.GetEncoding(1251))));
            MName = string.Join("||", msgList.Select(x => HttpUtility.UrlEncode(x.MName, Encoding.GetEncoding(1251))));
            UlTip = string.Join("||", msgList.Join(_context.RVC_SULICTIP, r => GetTip(r.Address), x => x.NAME, (x, r) => new { UlTip = r.ULTIP }).Select(x => x.UlTip));
            City = string.Join("||", msgList.Select(x => HttpUtility.UrlEncode(x.City, Encoding.GetEncoding(1251))));
            Street = string.Join("||", msgList.Select(x => HttpUtility.UrlEncode(GetStreet(x.Address), Encoding.GetEncoding(1251))));
            Houses = string.Join("||", msgList.Select(x => HttpUtility.UrlEncode(House(x.Home), Encoding.GetEncoding(1251))));
            Korps = string.Join("||", msgList.Select(x => HttpUtility.UrlEncode(Korp(x.Home), Encoding.GetEncoding(1251))));
            Inds = string.Join("||", msgList.Select(x => HttpUtility.UrlEncode(Ind(x.Home), Encoding.GetEncoding(1251))));
            Flats = string.Join("||", msgList.Select(x => HttpUtility.UrlEncode(Flat(x.Flat), Encoding.GetEncoding(1251))));
            Indexs = string.Join("||", msgList.Select(x => HttpUtility.UrlEncode(GetIndex(x.Flat), Encoding.GetEncoding(1251))));
            DOB = string.Join("||", msgList.Select(x => HttpUtility.UrlEncode((x.DOB != null ? x.DOB.Value.ToString("d") : ""), Encoding.GetEncoding(1251))));
            AddressDate = string.Join("||", msgList.Select(x => HttpUtility.UrlEncode((x.AddressDate != null ? x.AddressDate.Value.ToString("d") : ""), Encoding.GetEncoding(1251))));
            PhoneNO = string.Join("||", msgList.Select(x => String.Empty));

            Params = Params.Replace("[DocType]", DocType);
            Params = Params.Replace("[DocNo]", DocNo);
            Params = Params.Replace("[DocIssuer]", DocIssuer);
            Params = Params.Replace("[DocIssueDate]", DocIssueDate);
            Params = Params.Replace("[PersonalNo]", PersonalNo);
            Params = Params.Replace("[LName]", LName);
            Params = Params.Replace("[FName]", FName);
            Params = Params.Replace("[MName]", MName);
            Params = Params.Replace("[Tip]", UlTip);
            Params = Params.Replace("[City]", City);
            Params = Params.Replace("[Street]", Street);
            Params = Params.Replace("[House]", Houses);
            Params = Params.Replace("[Korp]", Korps);
            Params = Params.Replace("[Ind]", Inds);
            Params = Params.Replace("[Flat]", Flats);
            Params = Params.Replace("[Index]", Indexs);
            Params = Params.Replace("[DOB]", DOB);
            Params = Params.Replace("[AddressDate]", AddressDate);
            Params = Params.Replace("[PhoneNo]", PhoneNO);
            Params = Params.Replace("Param=", "");

            if (msgList.Any(x => x.Summ > 0))
            {
                //HTTP = HTTP.Replace("[Comment]", HttpUtility.UrlEncode(msgList.FirstOrDefault(x => x.Summ > 0).Notes, Encoding.GetEncoding(1251)));
                HTTP = HTTP.Replace("[Comment]", string.Join("||", msgList.Where(x => x.Summ > 0).Select(x => HttpUtility.UrlEncode(x.Notes, Encoding.GetEncoding(1251)))));
                HTTP = HTTP.Replace("[Summa]", msgList.Sum(x => x.Summ).ToString().Replace(".", ","));
                HTTP = HTTP.Replace("[PayNo]", string.Join("||", msgList.Where(x => x.Summ > 0).Select(x => x.PayNo)));
                // HTTP = HTTP.Replace("[PayNo]", msgList.FirstOrDefault(x => x.Summ > 0).PayNo);
            }
            else
            {
                HTTP = HTTP.Replace("[Comment]", "");
                HTTP = HTTP.Replace("[Summa]", "");
                HTTP = HTTP.Replace("[PayNo]", "");
            }


            //указать пароль из настроек программы
            HTTP = HTTP.Replace("[User]", setting.LoginBRTI);
            HTTP = HTTP.Replace("[Pass]", setting.PassBrti);
            HTTP = HTTP.Replace("[Now]", DateTime.Now.Date.ToString("d"));
            HTTP = HTTP.Replace("URL=", "");
            HTTP = HTTP.Replace("[ID]", msgList.First().Registration.DocNo.ToString());
            HTTP = HTTP + Params;
            HTTP = HTTP.Replace("[OrderNo]", msgList.First().Registration.OrderNo.ToString());

            foreach (var row in msgList)
            {
                row.File = HTTP;
            }

            _context.MSG.UpdateRange(msgList);

            await _context.SaveChangesAsync();

            return Ok();
        }

        public async Task<IActionResult> calculateSumm(List<msgModel> msgList)
        {
            if (!msgList.Any())
                return Ok();
            foreach (var msg in msgList)
            {
                msg.Summ = msg.zaprDoc.Summ;
            }


            _context.MSG.UpdateRange(msgList);

            await _context.SaveChangesAsync();

            return Ok();
        }


        public async Task<IActionResult> generateFile(List<msgModel> msgList, bool fullFamily = false)
        {
            if (!msgList.Any())
                return Ok();
            getUrlFile getUrl = new();

            string dirName = "DocsForRequests/";
            string urlSave = "";

            string templateDoc = getUrl.urlFile() + "Template/";

            var setting = await _context.Settings.FirstAsync();
            createDocument doc = new();

            Dictionary<string, string> replaseDictionary = new();
            Dictionary<string, List<string>> tab = new();

            var msgAll = await _context.MSG.Where(x => x.RegistrationId == msgList.First().RegistrationId).ToListAsync();

            foreach (var msg in msgList)
            {
                urlSave = getUrl.urlFile() + dirName + msg.MsgId.ToString() + "_" + msg.zaprDoc.File.Replace(' ', '_').Replace(',', '_');



                if (!System.IO.File.Exists(templateDoc + msg.zaprDoc.File))
                    continue;
                if (System.IO.File.Exists(urlSave))
                {
                    System.IO.File.Delete(urlSave);
                }
                if (System.IO.File.Exists(msg.File))
                {
                    System.IO.File.Delete(msg.File);
                }
                System.IO.File.Copy(templateDoc + msg.zaprDoc.File, urlSave);

                replaseDictionary = new();
                replaseDictionary.Add("LName", msg.LName);
                replaseDictionary.Add("FName", msg.FName);
                replaseDictionary.Add("MName", msg.MName ?? String.Empty);
                replaseDictionary.Add("PDocNo", msg.DocNo);
                replaseDictionary.Add("DocNo", msg.Registration.DocNo.ToString());
                replaseDictionary.Add("RegName", msg.Registration.RegName);
                replaseDictionary.Add("Organiz", msg.Registration.OrgName);
                replaseDictionary.Add("P_PersonalNo", msg.PersonalNo);
                replaseDictionary.Add("P_PassIssueDate", msg.DocIssueDate != null ? msg.DocIssueDate.Value.ToString("d") : String.Empty);
                replaseDictionary.Add("P_PassIssuer", msg.DocIssuer);
                replaseDictionary.Add("PhoneNoM", msg.Registration.MobPhone);
                replaseDictionary.Add("NoDog", msg.DogNo);
                replaseDictionary.Add("ZaNotes", msg.Registration.Notes);
                replaseDictionary.Add("ZpNotes", msg.Notes);
                replaseDictionary.Add("NameZp", msg.zaprDoc.Name);
                replaseDictionary.Add("PassIssuer", msg.DocIssuer);
                replaseDictionary.Add("PassIssueDate", msg.DocIssueDate != null ? msg.DocIssueDate.Value.ToString("d") : "");
                replaseDictionary.Add("DateDog", msg.DogDate != null ? msg.DogDate.Value.ToString("d") : "");
                replaseDictionary.Add("DocIssueDate", msg.DocIssueDate != null ? msg.DocIssueDate.Value.ToString("d") : "");
                replaseDictionary.Add("DocIssuer", string.Format("{0:d}", msg.DocIssuer.ToString()));
                replaseDictionary.Add("Worker", msg.Registration.Registrator);
                replaseDictionary.Add("Phone", setting.Phone);
                replaseDictionary.Add("Naim", setting.Name);
                replaseDictionary.Add("DocType", dictionaryList.typeDoc[msg.DocType]);
                replaseDictionary.Add("DOB", msg.DOB != null ? msg.DOB.Value.ToString("d") : "");
                if (msg.orgsZapr != null)
                {
                    replaseDictionary.Add("NameOrg", msg.orgsZapr.Name);
                    replaseDictionary.Add("AddressOrg", msg.orgsZapr.PostAddress);
                }
                replaseDictionary.Add("OrderNo", msg.Registration.OrderNo.ToString());
                replaseDictionary.Add("bank", msg.Bank);
                replaseDictionary.Add("Address", msg.City + "," + msg.Address + ",д." + msg.Home + ",кв." + msg.Flat);
                replaseDictionary.Add("GettingDate", msg.Registration.GettingDate != null ? msg.Registration.GettingDate.Value.ToString("d") : "");
                replaseDictionary.Add("dtpDay", DateTime.Now.Day.ToString());
                replaseDictionary.Add("dtpMonth", DateTime.Now.Month.ToString("MMMM", CultureInfo.CreateSpecificCulture("ru-RU")));
                replaseDictionary.Add("dtpYear", DateTime.Now.Year.ToString());

                replaseDictionary.Add("FullName", string.Join(", ", msgList.Where(x => x.DocNo == msg.DocNo).Select(x => x.LName + " " + x.FName + (x.MName != null ? " " + x.MName : ""))));

                if (fullFamily)
                {
                    tab = new();
                    foreach (var m in msgAll.Where(x => x.PayNo == msg.PayNo))
                    {
                        tab.Add((tab.Count + 1).ToString(), new List<string>() {
                        m.LName + " " + m.FName + " " + m.MName
                        ,m.DOB != null ? m.DOB.Value.ToString("d") : String.Empty
                        ,m.City + ", " + m.Address + ", д." + m.Home + ", кв." + m.Flat
                        ,dictionaryList.typeDoc[m.DocType]
                        ,m.DocNo
                        ,m.PersonalNo
                        ,m.DocIssuer
                        ,m.DocIssueDate != null ? m.DocIssueDate.Value.ToString("d") : String.Empty
                        });
                    }
                    urlSave = doc.createWord(urlSave, replaseDictionary, tab, true);
                }
                else
                    urlSave = doc.createWord(urlSave, replaseDictionary, null);

                msg.File = urlSave;

            }

            _context.MSG.UpdateRange(msgList);
            await _context.SaveChangesAsync();


            return Ok();
        }


        private string GetStreet(string address)
        {
            if (string.IsNullOrEmpty(address))
                return "";
            string[] streets = address.Split(' ');
            string street = "";
            for (int i = 0; i < streets.Count() - 1; i++)
                street += streets[i];
            return street;
        }

        private string GetIndex(string flat)
        {
            string ind = "";
            if (!string.IsNullOrEmpty(flat))
            {
                for (int i = 0; i < flat.Length; i++)
                {
                    if (flat[i] < '0' || flat[i] > '9')
                        ind += flat[i];
                }
            }
            return ind;
        }

        private string Flat(string flat)
        {
            string ind = "";
            if (!string.IsNullOrEmpty(flat))
            {
                for (int i = 0; i < flat.Length; i++)
                {
                    if (flat[i] >= '0' && flat[i] <= '9')
                        ind += flat[i];
                }
            }
            return ind;
        }

        private string Korp(string home)
        {
            if (string.IsNullOrEmpty(home))
                return "";
            string[] korps = home.Split('к');
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

        private string House(string home)
        {
            if (string.IsNullOrEmpty(home))
                return "";
            string[] korps = home.Split('к');
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
                for (int i = 0; i < home.Length; i++)
                {
                    if (home[i] >= '0' && home[i] <= '9')
                        korp += home[i];
                }
                return korp;
            }
        }

        private string Ind(string home)
        {
            if (string.IsNullOrEmpty(home))
                return "";
            string[] korps = home.Split('к');
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
                for (int i = 0; i < home.Length; i++)
                {
                    if (home[i] < '0' || home[i] > '9')
                        ind += home[i];
                }
                return ind;
            }
        }

        private string GetTip(string address)
        {
            if (string.IsNullOrEmpty(address))
                return "";
            string[] tipName = address.Split(' ');
            return tipName[tipName.Count() - 1];
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
                else return null;
            }
            return null;
        }

        
    }
}
