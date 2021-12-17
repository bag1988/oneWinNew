using DocumentFormat.OpenXml.Wordprocessing;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using oneWin.Data;
using oneWin.Models;
using oneWin.Models.baseModel;
using oneWin.OfficeCreate;
using oneWin.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
namespace oneWin.Controllers
{
    public class HomeController : Controller
    {
        private readonly oneWinDbContext _context;

        private viewTable _createTable;
        private WorkingDay _w;

        public HomeController(oneWinDbContext context, viewTable createTable, WorkingDay w)
        {
            _context = context;
            _createTable = createTable;
            _w = w;
        }


        public async Task<IActionResult> Index()
        {
            var queryList = Request.Query;
            string otdel = "";

            if (queryList != null)
            {
                if (queryList.ContainsKey("otdel"))
                {
                    otdel = queryList["otdel"];
                }                
            }

            if (otdel != "")
            {
                if (User.IsInRole("administrator"))
                {
                    Response.Cookies.Append("otdel", otdel);
                    if (Request.GetTypedHeaders().Referer != null && Request.GetTypedHeaders().Referer.AbsolutePath != Request.Path.Value)
                        return Redirect(Request.GetTypedHeaders().Referer.ToString());
                }
            }            
            return View(await _createTable.createTable(queryList.Where(x => x.Key != "otdel").ToDictionary(x => x.Key, x => x.Value.ToString())));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public async Task<IActionResult> viewChildTable(string quertyString) => PartialView("_viewTable", await _createTable.createTable(quertyString));



        /// <summary>
        /// Отправка записей в отдел
        /// </summary>
        /// <param name="arrayProc">Список записей(GuidId)</param>
        /// <param name="dateSend"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> sendToOtdel(Guid[] arrayProc, DateTime dateSend)
        {
            if (arrayProc.Length == 0)
                return BadRequest("Вы не выбрали ни одной записи для совершения операции!");
            string errorMesssage = "";
            string registrator = User.Identity.Name;
            int countSend = 0;
            foreach (var g in arrayProc)
            {
                if (_context.Registration.Any(x => x.RegistrationID == g))
                {
                    var reg = _context.Registration.OrderBy(x=>x.GettingDate).First(x => x.RegistrationID == g);

                    if (User.IsInRole("registrator2") && reg.Registrator != registrator)
                    {
                        errorMesssage += "Вы не можете редактировать запись с номером: " + reg.DocNo + "!!!<br/>";
                        continue;
                    }

                    if (dateSend.AddDays(1).Date <= reg.GettingDate.Value.Date)
                    {
                        errorMesssage += "Дата передачи в отдел не может быть раньше даты обращения. Запись №: " + reg.DocNo + "!!!<br/>";
                        continue;
                    }
                    if (reg.OutDeptDate != null)
                    {
                        errorMesssage += "Запись №: " + reg.DocNo + " уже передана в отдел!!!<br/>";
                        continue;
                    }


                    reg.State = 2;
                    reg.OutDeptDate = dateSend;
                    _context.Registration.Update(reg);
                    await _context.SaveChangesAsync();
                    countSend++;
                }
                else
                {
                    errorMesssage += "Записи с номером: " + g + " не существует!!!<br/>";
                }
            }
            errorMesssage += "Отправлено в отдел: " + countSend + " записей.<br/>";
            return Ok(errorMesssage);
        }

        /// <summary>
        /// Исправление дата отправки в отдел
        /// </summary>
        /// <param name="arrayProc"></param>
        /// <param name="dateSend"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> replaceDateToOtdel(Guid[] arrayProc, DateTime dateSend)
        {
            if (arrayProc.Length == 0)
                return BadRequest("Вы не выбрали ни одной записи для совершения операции!");
            string errorMesssage = "";
            string registrator = User.Identity.Name;
            int countSend = 0;
            DateTime dateSsol = new DateTime(1990, 1, 1);
            foreach (var g in arrayProc)
            {
                if (_context.Registration.Any(x => x.RegistrationID == g))
                {
                    var reg = _context.Registration.OrderBy(x => x.GettingDate).First(x => x.RegistrationID == g);

                    if (User.IsInRole("registrator2") && reg.Registrator != registrator)
                    {
                        errorMesssage += "Вы не можете редактировать запись с номером: " + reg.DocNo + "!!!<br/>";
                        continue;
                    }
                    if (reg.OutDeptDate == null)
                    {
                        errorMesssage += "Запись №: " + reg.DocNo + " не была передана в отдел!!!<br/>";
                        continue;
                    }
                    if ((reg.GettingDate == null || dateSend.Date.CompareTo(reg.GettingDate.Value.Date) >= 0)
                        && (dateSend.Date.CompareTo(DateTime.Now.Date) <= 0)
                        && (reg.ReturnInDeptDate == null || dateSend.Date.CompareTo(reg.ReturnInDeptDate.Value.Date) <= 0)
                        && (reg.IssueDate == null || dateSend.Date.CompareTo(reg.IssueDate.Value.Date) <= 0)
                        //&& (reg.MustBeReady == null || dateSend.Date.CompareTo(reg.MustBeReady.Value.Date) <= 0)
                        && ((reg.DateSsolutions == null || (reg.DateSsolutions.Value.Date.CompareTo(dateSsol.Date) < 0)) || dateSend.Date.CompareTo(reg.DateSsolutions.Value.Date) <= 0)
                        )
                    {
                        reg.State = 2;
                        reg.OutDeptDate = dateSend;
                        _context.Registration.Update(reg);
                        await _context.SaveChangesAsync();
                        countSend++;
                        continue;

                    }
                    errorMesssage += ("Сравните дату отправления в отдел записи № " + reg.DocNo + " с остальными датами<br/>");
                }
                else
                {
                    errorMesssage += "Записи с номером: " + g + " не существует!!!<br/>";
                }
            }
            errorMesssage += "Исправленно: " + countSend + " записей.<br/>";
            return Ok(errorMesssage);
        }


        /// <summary>
        /// возврат из отдела и отправка уведомления на почту
        /// </summary>
        /// <param name="arrayProc"></param>
        /// <param name="dateSend"></param>
        /// <param name="dateDecision"></param>
        /// <param name="decisionNumber"></param>
        /// <param name="issueResolved"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> returnFromDept(Guid[] arrayProc, DateTime dateSend, DateTime? dateDecision = null, string decisionNumber = null, string issueResolved = null)
        {
            if (arrayProc.Length == 0)
                return BadRequest("Вы не выбрали ни одной записи для совершения операции!");
            DateTime dateSsol = new DateTime(1990, 1, 1);
            if (dateDecision != null && dateDecision.Value.Date <= dateSsol)
                return BadRequest("Проверьте правильность введенной информации!");
            string errorMesssage = "";
            string registrator = User.Identity.Name;
            int countSend = 0;
            int countSendEmail = 0;

            foreach (var g in arrayProc)
            {
                if (_context.Registration.Any(x => x.RegistrationID == g))
                {
                    var reg = _context.Registration.OrderBy(x => x.GettingDate).First(x => x.RegistrationID == g);

                    if (User.IsInRole("registrator2") && reg.Registrator != registrator)
                    {
                        errorMesssage += "Вы не можете редактировать запись с номером: " + reg.DocNo + "!!!<br/>";
                        continue;
                    }
                    if (reg.State == 1)
                    {
                        errorMesssage += "Дело № " + reg.DocNo + " еще не передавалось в отдел.!!!<br/>";
                        continue;
                    }
                    if (reg.State == 4)
                    {
                        errorMesssage += "Дело № " + reg.DocNo + " уже выдано заявителю!!!<br/>";
                        continue;
                    }
                    if (reg.State == 3 || reg.ReturnInDeptDate != null)
                    {
                        errorMesssage += "Дело № " + reg.DocNo + " уже возвращено из отдел!!!<br/>";
                        continue;
                    }
                    if (dateSend.Date.CompareTo(DateTime.Now.Date) > 0)
                    {
                        errorMesssage += "Дата возврата из отдела не может быть позже текущей даты. Дело № " + reg.DocNo + "!!!<br/>";
                        continue;
                    }
                    if (dateDecision != null && dateDecision.Value.Date.CompareTo(DateTime.Now.Date) > 0)
                    {
                        errorMesssage += "Дата принятия решения не может быть позже текущей даты. Дело № " + reg.DocNo + "!!!<br/>";
                        continue;
                    }
                    if (dateDecision != null && dateSend.Date.CompareTo(dateDecision.Value.Date) < 0)
                    {
                        errorMesssage += "Дата возврата из отдела не может быть раньше даты принятия решения. Дело № " + reg.DocNo + "!!!<br/>";
                        continue;
                    }

                    if (dateDecision != null && dateDecision.Value.Date.CompareTo(reg.OutDeptDate.Value.Date) < 0)
                    {
                        errorMesssage += "Дата принятия решения не может быть раньше даты передачи в отдел. Дело № " + reg.DocNo + "!!!<br/>";
                        continue;
                    }
                    if (dateDecision != null && dateDecision.Value.Date.CompareTo(reg.GettingDate.Value.Date) < 0)
                    {
                        errorMesssage += "Дата принятия решения не может быть раньше даты регистрации. Дело № " + reg.DocNo + "!!!<br/>";
                        continue;
                    }
                    if (dateSend.Date.CompareTo(reg.OutDeptDate.Value.Date) < 0)
                    {
                        errorMesssage += "Дата возврата из отдела не может быть раньше даты передачи в отдел. Дело № " + reg.DocNo + "!!!<br/>";
                        continue;
                    }
                    if (dateSend.Date.CompareTo(reg.GettingDate.Value.Date) < 0)
                    {
                        errorMesssage += "Дата возврата из отдела не может быть раньше даты регистрации. Дело № " + reg.DocNo + "!!!<br/>";
                        continue;
                    }

                    reg.State = 3;
                    reg.ReturnInDeptDate = dateSend;
                    if (reg.DateSsolutions == null)
                        reg.DateSsolutions = dateDecision;
                    _context.Registration.Update(reg);

                    var sol = await _context.Solutions.OrderBy(x=>x.Id).FirstAsync(x => x.RegistrationId == reg.RegistrationID);

                    sol.dateOfSolution = dateDecision;
                    sol.solutionNumber = decisionNumber ?? "";
                    sol.solution = issueResolved;
                    _context.Solutions.Update(sol);

                    await _context.SaveChangesAsync();
                    countSend++;

                    if (await SendEmailAsync(reg))
                        countSendEmail++;
                }
                else
                {
                    errorMesssage += "Записи с номером: " + g + " не существует!!!<br/>";
                }
            }
            errorMesssage += "Возвращено из отдела: " + countSend + " записей. Выслано уведомлений " + countSendEmail + "<br/>";
            return Ok(errorMesssage);
        }


        /// <summary>
        /// Исправление даты возврата из отдела
        /// </summary>
        /// <param name="arrayProc"></param>
        /// <param name="dateSend"></param>
        /// <param name="dateDecision"></param>
        /// <param name="decisionNumber"></param>
        /// <param name="issueResolved"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> replaceDateFromOtdel(Guid[] arrayProc, DateTime? dateSend = null, DateTime? dateDecision = null, string decisionNumber = null, string issueResolved = null)
        {
            if (arrayProc.Length == 0)
                return BadRequest("Вы не выбрали ни одной записи для совершения операции!");
            DateTime dateSsol = new DateTime(1990, 1, 1);
            if (dateDecision != null && dateDecision.Value.Date <= dateSsol)
                return BadRequest("Проверьте правильность введенной информации!");
            string errorMesssage = "";
            string registrator = User.Identity.Name;
            int countSend = 0;
            foreach (var g in arrayProc)
            {
                if (_context.Registration.Any(x => x.RegistrationID == g))
                {
                    var reg = _context.Registration.OrderBy(x => x.GettingDate).First(x => x.RegistrationID == g);

                    if (User.IsInRole("registrator2") && reg.Registrator != registrator)
                    {
                        errorMesssage += "Вы не можете редактировать запись с номером: " + reg.DocNo + "!!!<br/>";
                        continue;
                    }
                    if (reg.ReturnInDeptDate == null)
                    {
                        errorMesssage += "Запись №: " + reg.DocNo + " не была передана из отдел!!!<br/>";
                        continue;
                    }
                    if ((dateSend == null || ((reg.GettingDate == null || dateSend.Value.Date.CompareTo(reg.GettingDate.Value.Date) >= 0)
                        && (dateSend.Value.Date.CompareTo(DateTime.Now.Date) <= 0)
                        && (reg.OutDeptDate == null || dateSend.Value.Date.CompareTo(reg.OutDeptDate.Value.Date) >= 0)
                        && (dateDecision == null || dateSend.Value.CompareTo(dateDecision) >= 0)
                        //&& ((reg.sol == null || (reg.sol.dateOfSolution==null||reg.sol.dateOfSolution.Value.CompareTo(dateSend) >= 0)))
                        //&& (reg.ReturnInDeptDate == null || dateSend.Date.CompareTo(reg.ReturnInDeptDate.Value.Date) <= 0)
                        && (reg.IssueDate == null || dateSend.Value.Date.CompareTo(reg.IssueDate.Value.Date) <= 0)
                        //&& (reg.MustBeReady == null || dateSend.Date.CompareTo(reg.MustBeReady.Value.Date) <= 0)
                        ))
                        && (dateDecision == null || (
                        (reg.GettingDate == null || dateDecision.Value.Date.CompareTo(reg.GettingDate.Value.Date) >= 0)
                        && (dateDecision.Value.Date.CompareTo(DateTime.Now) <= 0)
                        && (reg.OutDeptDate == null || dateDecision.Value.Date.CompareTo(reg.OutDeptDate.Value.Date) >= 0)
                        //&& ((reg.DateSsolutions == null || (reg.DateSsolutions.Value.CompareTo(dateSsol) < 0)) || dateDecision.Date.CompareTo(reg.DateSsolutions.Value.Date) <= 0)
                        //&& (reg.ReturnInDeptDate == null || dateDecision.Date.CompareTo(reg.ReturnInDeptDate.Value.Date) <= 0)
                        && (reg.IssueDate == null || dateDecision.Value.Date.CompareTo(reg.IssueDate.Value.Date) <= 0)))
                        )
                    {

                        if (dateSend != null)
                        {
                            reg.State = 4;
                            reg.ReturnInDeptDate = dateSend;
                        }
                        var sol = await _context.Solutions.OrderBy(x=>x.Id).FirstAsync(x => x.RegistrationId == reg.RegistrationID);

                        if (dateDecision != null)
                        {
                            if (reg.DateSsolutions == null)
                                reg.DateSsolutions = dateDecision;
                            sol.dateOfSolution = dateDecision;

                        }
                        sol.solutionNumber = decisionNumber ?? sol.solutionNumber;
                        sol.solution = issueResolved;

                        _context.Solutions.Update(sol);
                        _context.Registration.Update(reg);

                        await _context.SaveChangesAsync();
                        countSend++;
                        continue;

                    }
                    errorMesssage += ("Сравните введенные даты для записи № " + reg.DocNo + " с остальными датами<br/>");
                }
                else
                {
                    errorMesssage += "Записи с номером: " + g + " не существует!!!<br/>";
                }
            }
            errorMesssage += " Исправлены данные возвращения из отдела: " + countSend + " записей.<br/>";
            return Ok(errorMesssage);
        }

        /// <summary>
        /// возврат из отдела и выдача заявителю
        /// </summary>
        /// <param name="arrayProc">список записей</param>
        /// <param name="issueDate">дата выдачи</param>
        /// <param name="dateSend">дата возврата</param>
        /// <param name="dateDecision">дата решения</param>
        /// <param name="evaluationNotification">выдано на</param>
        /// <param name="caseNumber">дело №</param>
        /// <param name="decisionNumber">решение №</param>
        /// <param name="issueResolved">вопрос решен</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> returnFromDeptAndIssuance(Guid[] arrayProc, DateTime issueDate, DateTime? dateSend = null, DateTime? dateDecision = null, string evaluationNotification = null, string caseNumber = null, string decisionNumber = null, string issueResolved = null)
        {
            if (arrayProc.Length == 0)
                return BadRequest("Вы не выбрали ни одной записи для совершения операции!");
            DateTime dateSsol = new DateTime(1990, 1, 1);
            if (dateDecision != null && dateDecision.Value.Date <= dateSsol)
                return BadRequest("Проверьте правильность введенной информации!");
            string errorMesssage = "";
            string registrator = User.Identity.Name;
            int countSend = 0;
            foreach (var g in arrayProc)
            {
                if (_context.Registration.Any(x => x.RegistrationID == g))
                {
                    var reg = _context.Registration.OrderBy(x => x.GettingDate).First(x => x.RegistrationID == g);

                    if (User.IsInRole("registrator2") && reg.Registrator != registrator)
                    {
                        errorMesssage += "Вы не можете редактировать запись с номером: " + reg.DocNo + "!!!<br/>";
                        continue;
                    }
                    if (reg.IssueDate != null)
                    {
                        errorMesssage += "Запись №: " + reg.DocNo + " уже выдана заявителю!!!<br/>";
                        continue;
                    }
                    if ((dateSend == null || ((reg.GettingDate == null || dateSend.Value.Date.CompareTo(reg.GettingDate.Value.Date) >= 0)
                        && (dateSend.Value.Date.CompareTo(DateTime.Now.Date) <= 0)
                        && (reg.OutDeptDate == null || dateSend.Value.Date.CompareTo(reg.OutDeptDate.Value.Date) >= 0)
                        && (dateDecision == null || dateSend.Value.Date.CompareTo(dateDecision.Value.Date) >= 0)
                        && (issueDate.Date.CompareTo(dateSend.Value.Date) >= 0)
                        ))
                        &&(dateDecision == null || (dateDecision.Value.Date.CompareTo(reg.GettingDate.Value.Date) >= 0)
                        && (dateDecision.Value.Date.CompareTo(DateTime.Now.Date) <= 0)
                        && (issueDate.Date.CompareTo(dateDecision.Value.Date) >= 0)
                        && (reg.OutDeptDate == null || dateDecision.Value.Date.CompareTo(reg.OutDeptDate.Value.Date) >= 0)
                        )
                        && (reg.GettingDate == null || issueDate.Date.CompareTo(reg.GettingDate.Value.Date) >= 0)
                        && (reg.OutDeptDate == null || issueDate.Date.CompareTo(reg.OutDeptDate.Value.Date) >= 0)                        
                        && (issueDate.Date.CompareTo(DateTime.Now.Date) <= 0)
                        )
                    {

                        reg.IssueDate = issueDate;
                        reg.CaseNamber = caseNumber;
                        reg.EvaluationNotification = evaluationNotification;
                        if (dateSend != null)
                        {
                            reg.State = 4;
                            reg.ReturnInDeptDate = dateSend;

                        }
                        var sol = await _context.Solutions.OrderBy(x=>x.Id).FirstAsync(x => x.RegistrationId == reg.RegistrationID);

                        if (dateDecision != null)
                        {
                            if (reg.DateSsolutions == null)
                                reg.DateSsolutions = dateDecision;
                            sol.dateOfSolution = dateDecision;

                        }
                        sol.solutionNumber = decisionNumber ?? sol.solutionNumber;
                        reg.NamberSolutions = sol.solutionNumber;
                        sol.solution = issueResolved;

                        _context.Solutions.Update(sol);
                        _context.Registration.Update(reg);

                        await _context.SaveChangesAsync();
                        countSend++;
                        continue;

                    }
                    errorMesssage += ("Сравните введенные даты для записи № " + reg.DocNo + " с остальными датами<br/>");
                }
                else
                {
                    errorMesssage += "Записи с номером: " + g + " не существует!!!<br/>";
                }
            }
            errorMesssage += "Возвращено из отдела и выдано заявителю: " + countSend + " записей.<br/>";
            return Ok(errorMesssage);
        }


        /// <summary>
        /// выдача заявителю
        /// </summary>
        /// <param name="arrayProc"></param>
        /// <param name="issueDate"></param>
        /// <param name="evaluationNotification"></param>
        /// <param name="caseNumber"></param>
        /// <param name="issueResolved"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> issuanceApplicant(Guid[] arrayProc, DateTime issueDate, string evaluationNotification = null, string caseNumber = null, string issueResolved = null)
        {
            if (arrayProc.Length == 0)
                return BadRequest("Вы не выбрали ни одной записи для совершения операции!");
            if (issueDate == new DateTime())
            {
                return BadRequest("Введите дату выдачи!!!<br/>");
            }
            string errorMesssage = "";
            string registrator = User.Identity.Name;
            int countSend = 0;
            foreach (var g in arrayProc)
            {
                if (_context.Registration.Any(x => x.RegistrationID == g))
                {
                    var reg = _context.Registration.Include(x=>x.sol).OrderBy(x => x.GettingDate).First(x => x.RegistrationID == g);

                    if (User.IsInRole("registrator2") && reg.Registrator != registrator)
                    {
                        errorMesssage += "Вы не можете редактировать запись с номером: " + reg.DocNo + "!!!<br/>";
                        continue;
                    }
                    if (reg.IssueDate != null)
                    {
                        errorMesssage += "Запись №: " + reg.DocNo + " уже выдана заявителю!!!<br/>";
                        continue;
                    }
                    if (issueDate.Date.CompareTo(DateTime.Now.Date) > 0)
                    {
                        errorMesssage += "Дата выдачи заявителю не может быть позже текущей даты. Дело № " + reg.DocNo + "!!!<br/>";
                        continue;
                    }
                    if (issueDate.Date.CompareTo(reg.GettingDate.Value.Date) < 0)
                    {
                        errorMesssage += "Дата выдачи заявителю не может быть раньше даты регистрации. Дело № " + reg.DocNo + "!!!<br/>";
                        continue;
                    }
                    if (issueDate.Date.CompareTo(reg.OutDeptDate.Value.Date) < 0)
                    {
                        errorMesssage += "Дата выдачи заявителю не может быть раньше даты передачи в отдел. Дело № " + reg.DocNo + "!!!<br/>";
                        continue;
                    }
                    if (reg.sol==null ||(reg.sol.dateOfSolution==null|| issueDate.Date.CompareTo(reg.sol.dateOfSolution.Value.Date) < 0))
                    {
                        errorMesssage += "Дата выдачи заявителю не может быть раньше даты принятия решения. Дело № " + reg.DocNo + "!!!<br/>";
                        continue;
                    }


                    reg.IssueDate = issueDate;
                    if (reg.DateSsolutions == null)
                        reg.DateSsolutions = issueDate;
                    reg.CaseNamber = caseNumber;
                    reg.EvaluationNotification = evaluationNotification;
                    reg.State = 4;


                    var sol = await _context.Solutions.OrderBy(x=>x.Id).FirstAsync(x => x.RegistrationId == reg.RegistrationID);

                    if (string.IsNullOrEmpty(sol.solution) && !string.IsNullOrEmpty(issueResolved))
                    {
                        sol.dateOfSolution = issueDate;
                        sol.solution = issueResolved;
                    }

                    _context.Solutions.Update(sol);
                    _context.Registration.Update(reg);

                    await _context.SaveChangesAsync();
                    countSend++;
                    continue;

                }
                else
                {
                    errorMesssage += "Записи с номером: " + g + " не существует!!!<br/>";
                }
            }
            errorMesssage += "Выдано заявителям: " + countSend + " записей.<br/>";
            return Ok(errorMesssage);
        }


        /// <summary>
        /// исправление выдачи заявителю
        /// </summary>
        /// <param name="arrayProc"></param>
        /// <param name="issueDate"></param>
        /// <param name="evaluationNotification"></param>
        /// <param name="caseNumber"></param>
        /// <param name="issueResolved"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> replaceIssuanceApplicant(Guid[] arrayProc, DateTime issueDate, string evaluationNotification = null, string caseNumber = null, string issueResolved = null)
        {
            if (arrayProc.Length == 0)
                return BadRequest("Вы не выбрали ни одной записи для совершения операции!");
            if (issueDate == new DateTime())
            {
                return BadRequest("Введите дату выдачи!!!<br/>");
            }
            string errorMesssage = "";
            string registrator = User.Identity.Name;
            int countSend = 0;
            foreach (var g in arrayProc)
            {
                if (_context.Registration.Include(x=>x.sol).Any(x => x.RegistrationID == g))
                {
                    var reg = _context.Registration.OrderBy(x => x.GettingDate).First(x => x.RegistrationID == g);

                    if (User.IsInRole("registrator2") && reg.Registrator != registrator)
                    {
                        errorMesssage += "Вы не можете редактировать запись с номером: " + reg.DocNo + "!!!<br/>";
                        continue;
                    }
                    if ((reg.GettingDate == null || issueDate.Date.CompareTo(reg.GettingDate.Value.Date) >= 0)
                        && (issueDate.Date.CompareTo(DateTime.Now.Date) <= 0)
                        && (reg.OutDeptDate == null || issueDate.Date.CompareTo(reg.OutDeptDate.Value.Date) >= 0)
                         && (reg.ReturnInDeptDate == null || issueDate.Date.CompareTo(reg.ReturnInDeptDate.Value.Date) >= 0)
                          && (reg.sol == null ||(reg.sol.dateOfSolution==null || issueDate.Date.CompareTo(reg.sol.dateOfSolution.Value.Date) >= 0))
                        )
                    {

                        reg.IssueDate = issueDate;
                        if (reg.DateSsolutions == null)
                            reg.DateSsolutions = issueDate;
                        reg.EvaluationNotification = evaluationNotification;
                        //reg.State = 4;


                        if (!string.IsNullOrEmpty(caseNumber))
                        {
                            reg.CaseNamber = caseNumber;
                        }

                        var sol = await _context.Solutions.OrderBy(x=>x.Id).FirstAsync(x => x.RegistrationId == reg.RegistrationID);

                        sol.dateOfSolution = issueDate;

                        if (string.IsNullOrEmpty(sol.solution) && !string.IsNullOrEmpty(issueResolved))
                        {
                            sol.solution = issueResolved;
                        }

                        _context.Solutions.Update(sol);
                        _context.Registration.Update(reg);

                        await _context.SaveChangesAsync();
                        countSend++;
                        continue;

                    }
                    errorMesssage += ("Сравните введенные даты для записи № " + reg.DocNo + " с остальными датами<br/>");
                }
                else
                {
                    errorMesssage += "Записи с номером: " + g + " не существует!!!<br/>";
                }
            }
            errorMesssage += "Исправлены данные выдачи заявителям: " + countSend + " записей.<br/>";
            return Ok(errorMesssage);
        }

        /// <summary>
        /// Вернуть в поступившие
        /// </summary>
        /// <param name="arrayProc"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> returnToReceived(Guid[] arrayProc)
        {
            if (arrayProc.Length == 0)
                return BadRequest("Вы не выбрали ни одной записи для совершения операции!");
            string errorMesssage = "";
            string registrator = User.Identity.Name;
            int countSend = 0;
            foreach (var g in arrayProc)
            {
                if (_context.Registration.Any(x => x.RegistrationID == g))
                {
                    var reg = await _context.Registration.OrderBy(x=>x.GettingDate).FirstAsync(x => x.RegistrationID == g);

                    if (User.IsInRole("registrator2") && reg.Registrator != registrator)
                    {
                        errorMesssage += "Вы не можете редактировать запись с номером: " + reg.DocNo + "!!!<br/>";
                        continue;
                    }

                    reg.State = 1;
                    reg.OutDeptDate = null;
                    reg.ReturnInDeptDate = null;
                    reg.DateSsolutions = null;
                    reg.NamberSolutions = null;
                    reg.Deleted = null;
                    reg.CaseNamber = null;
                    reg.IssueDate = null;
                    reg.EvaluationNotification = null;



                    var sol = await _context.Solutions.OrderBy(x=>x.Id).FirstOrDefaultAsync(x => x.RegistrationId == reg.RegistrationID);

                    sol.solution = null;
                    sol.solutionNumber = null;
                    sol.dateOfSolution = null;

                    var sus = await _context.SuspendedDocRegistries.Where(x => x.RegistrationId == reg.RegistrationID).ToListAsync();

                    _context.SuspendedDocRegistries.RemoveRange(sus);
                    _context.Registration.Update(reg);
                    _context.Solutions.Update(sol);

                    await _context.SaveChangesAsync();

                    countSend++;
                }
                else
                {
                    errorMesssage += "Записи с номером: " + g + " не существует!!!<br/>";
                }
            }
            errorMesssage += "Возвращено в поступившие: " + countSend + " записей.<br/>";
            return Ok(errorMesssage);
        }

        /// <summary>
        /// приостановить
        /// </summary>
        /// <param name="arrayProc"></param>
        /// <param name="dateSend"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> stopStatement(Guid[] arrayProc, DateTime dateSend, string reason = null)
        {
            if (arrayProc.Length == 0)
                return BadRequest("Вы не выбрали ни одной записи для совершения операции!");
            string errorMesssage = "";
            string registrator = User.Identity.Name;
            int countSend = 0;
            foreach (var g in arrayProc)
            {
                if (_context.Registration.Any(x => x.RegistrationID == g))
                {
                    var reg = await _context.Registration.OrderBy(x=>x.GettingDate).FirstAsync(x => x.RegistrationID == g);

                    if (await _context.SuspendedDocRegistries.AnyAsync(x => x.RegistrationId == reg.RegistrationID && x.beginDate == null))
                    {
                        errorMesssage += "Запись с номером: " + reg.DocNo + " уже приостановлена!!!<br/>";
                        continue;
                    }

                    if (User.IsInRole("registrator2") && reg.Registrator != registrator)
                    {
                        errorMesssage += "Вы не можете редактировать запись с номером: " + reg.DocNo + "!!!<br/>";
                        continue;
                    }

                    if (reg.MustBeReady.Value.Date < dateSend.Date)
                    {
                        errorMesssage += "Дата приостановки не может быть позже срока готовности! Ошибка в деле №: " + reg.DocNo + "!!!<br/>";
                        continue;
                    }

                    if (reg.GettingDate.Value.Date > dateSend.Date)
                    {
                        errorMesssage += "Дата приостановки не может быть раньше даты подачи! Ошибка в деле №: " + reg.DocNo + "!!!<br/>";
                        continue;
                    }

                    reg.Proceedings = reg.Proceedings + "Приостановлено " + dateSend.ToShortDateString() + " " + reason;


                    SuspendedDocRegistrieModel suspended = new();

                    suspended.RegistrationId = reg.RegistrationID;
                    suspended.reason = reason;
                    suspended.stopDate = dateSend;

                    await _context.SuspendedDocRegistries.AddAsync(suspended);

                    _context.Registration.Update(reg);

                    await _context.SaveChangesAsync();

                    countSend++;
                }
                else
                {
                    errorMesssage += "Записи с номером: " + g + " не существует!!!<br/>";
                }
            }
            errorMesssage += "Приостановлено: " + countSend + " записей.<br/>";
            return Ok(errorMesssage);
        }


        /// <summary>
        /// возобновить
        /// </summary>
        /// <param name="arrayProc"></param>
        /// <param name="dateSend"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> resumeStatement(Guid[] arrayProc, DateTime dateSend)
        {
            if (arrayProc.Length == 0)
                return BadRequest("Вы не выбрали ни одной записи для совершения операции!");
            string errorMesssage = "";
            string registrator = User.Identity.Name;
            int countSend = 0;
            foreach (var g in arrayProc)
            {
                if (_context.Registration.Any(x => x.RegistrationID == g))
                {
                    var reg = await _context.Registration.Include(x=>x.DocRegistry).OrderBy(x=>x.GettingDate).FirstAsync(x => x.RegistrationID == g);

                    if (!await _context.SuspendedDocRegistries.AnyAsync(x => x.RegistrationId == reg.RegistrationID && x.beginDate == null))
                    {
                        errorMesssage += "Запись с номером: " + reg.DocNo + " не была приостановлена!!!<br/>";
                        continue;
                    }
                    var suspended = await _context.SuspendedDocRegistries.OrderBy(x=>x.Id).FirstAsync(x => x.RegistrationId == reg.RegistrationID && x.beginDate == null);


                    if (User.IsInRole("registrator2") && reg.Registrator != registrator)
                    {
                        errorMesssage += "Вы не можете редактировать запись с номером: " + reg.DocNo + "!!!<br/>";
                        continue;
                    }

                    if (suspended.stopDate.Value.Date > dateSend.Date)
                    {
                        errorMesssage += "Дата возобновления не может быть раньше даты приостановки! Ошибка в деле №: " + reg.DocNo + "!!!<br/>";
                        continue;
                    }


                    reg.Proceedings = reg.Proceedings + "Возобновлено " + dateSend.ToShortDateString();
                    suspended.beginDate = dateSend;


                    if(reg.DocRegistry.Issue.Value==1 || reg.DocRegistry.IssueZapr.Value==0)
                    {
                        TimeSpan t = suspended.beginDate.Value.Subtract(suspended.stopDate.Value);
                        reg.MustBeReady = await _w.GetDay(0, 1, reg.MustBeReady.Value.AddDays(t.Days));
                    }
                    else
                    {
                        reg.MustBeReady = await _w.GetDay(reg.DocRegistry.IssueZapr.Value, reg.DocRegistry.TypeIssueZapr.Value, dateSend);
                    }
                    _context.SuspendedDocRegistries.Update(suspended);

                    _context.Registration.Update(reg);

                    await _context.SaveChangesAsync();

                    countSend++;
                }
                else
                {
                    errorMesssage += "Записи с номером: " + g + " не существует!!!<br/>";
                }
            }
            errorMesssage += "Возобновлено: " + countSend + " записей.<br/>";
            return Ok(errorMesssage);
        }

        /// <summary>
        /// направить уведомления
        /// </summary>
        /// <param name="arrayProc"></param>
        /// <param name="dateSend"></param>
        /// <param name="registryNumber"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> sendNotification(Guid[] arrayProc, DateTime dateSend, string registryNumber = null)
        {
            if (arrayProc.Length == 0)
                return BadRequest("Вы не выбрали ни одной записи для совершения операции!");
            string errorMesssage = "";
            string registrator = User.Identity.Name;
            int countSend = 0;

            string url = new getUrlFile().urlFile();
            string urlNew = "";
            urlNew = url + "Registrys/" + DateTime.Now.ToString("yyyy_dd_MM_HH_mm_ss") + "_" + DateTime.Now.Ticks.ToString() + "_notification.docx";

            var area = _context.Settings.OrderBy(x=>x.ID).FirstOrDefault();
            createDocument createDoc = new createDocument();

            Dictionary<string, List<string>> tab = new();
            Dictionary<string, List<string>> tab1 = new();

            

            tab1.Add("paragraph" + (tab1.Count + 1).ToString(), new List<string> { "Реестр писем № " + registryNumber + " от " + dateSend.Date.ToString("d") + "?align=center" });
            tab1.Add("paragraph" + (tab1.Count + 1).ToString(), new List<string> { area.Name + "?align=center" });
            tab1.Add((tab1.Count + 1).ToString(), new List<string> { "Номер?align=center", "Рег. №?align=center", "Адресат?align=center", "Адрес получателя?align=center" });


            string areaAdress = _context.Curators.OrderBy(x=>x.FIO).FirstOrDefault(x => x.Areas.Number == area.ValueArea).Address;

            string headingName = "Минский городской\nисполнительный комитет\n" + area.Name + "\n" + areaAdress + "\n" + dateSend.Date.ToString("d");
            string shortAreaName = area.Name.Replace("Администрация ", "");
            string nameApplicant = "";
            string addressAplicant = "";

            foreach (var g in arrayProc)
            {
                if (_context.Registration.Any(x => x.RegistrationID == g))
                {
                    var reg = await _context.Registration.OrderBy(x=>x.GettingDate).FirstAsync(x => x.RegistrationID == g);
                    if (reg.State < 3)
                    {
                        errorMesssage += "Запись №: " + reg.DocNo + " еще не передана из отдела!!!<br/>";
                        continue;
                    }
                    if (dateSend.Date < reg.GettingDate.Value.Date)
                    {
                        errorMesssage += "Дата направления уведомления не может быть раньше даты подачи! Ошибка в деле №: " + reg.DocNo + "!!!<br/>";
                        continue;
                    }


                    if (tab.Count != 0)
                    {
                        tab.Add("paragraph" + (tab.Count + 1).ToString(), new List<string> { "\n\n\n\n\n\n\n" });
                    }



                    if ((new int[] { 2, 6, 8, 5 }).Contains((int)reg.TypeReg) && reg.OrgName!=null)
                        nameApplicant = reg.OrgName;
                    else
                        nameApplicant = reg.LName + " " + (reg.FName != null ? reg.FName : "") + " " + (reg.MName!=null ? reg.MName : "");
                    addressAplicant = reg.Address + ", д. " + reg.Home + ", кв. " + reg.Flat + ", " + reg.City;

                    tab.Add((tab.Count + 1).ToString(), new List<string> { headingName + "?size=24&align=center", nameApplicant + "\n" + addressAplicant + "?align=left" });

                    tab.Add("paragraph" + (tab.Count + 1).ToString(), new List<string> {area.Name+ " сообщает, что Ваше заявление от "+reg.GettingDate.Value.ToString("d")+" № "+reg.DocNo+
                        ", поданное в службу «одно окно» "+shortAreaName+", рассмотрено."});

                    if (shortAreaName == "Фрунзенского района г.Минска")
                        tab.Add("paragraph" + (tab.Count + 1).ToString(), new List<string> { "Итоговые документы Вы можете получить в службе «одно окно» " + shortAreaName + "(ул. Кальварийская, д.39) в будние дни с 8.00 до 20.00, в субботу с 9.00 до 13.00." });
                    else
                        tab.Add("paragraph" + (tab.Count + 1).ToString(), new List<string> { "Итоговые документы Вы можете получить в службе «одно окно» " + shortAreaName + " в будние дни с 8.00 до 20.00, в субботу с 9.00 до 13.00." });



                    tab1.Add((tab1.Count + 1).ToString(), new List<string> { (countSend + 1).ToString() + "?align=left", reg.DocNo.ToString() + "?align=left", nameApplicant + "?align=left", addressAplicant + "?align=left" });


                    reg.Proceedings = reg.Proceedings + " Направленно уведомление по почте " + dateSend.Date.ToString("d") + " " + (registryNumber == null ? "" : "Реестр № " + registryNumber);
                    reg.NotificationDate = dateSend;
                    reg.NotificationRegistryNumber = registryNumber;

                    _context.Registration.Update(reg);

                    await _context.SaveChangesAsync();

                    countSend++;
                }
                else
                {
                    errorMesssage += "Записи с номером: " + g + " не существует!!!<br/>";
                }
            }

            tab1.Add("paragraph" + (tab1.Count + 1).ToString(), new List<string> { "Всего направлено уведомлений: " + countSend });
            tab1.Add("paragraph" + (tab1.Count + 1).ToString(), new List<string> { "Подпись уполномоченного лица" });
            tab1.Add("paragraph" + (tab1.Count + 1).ToString(), new List<string> { DateTime.Now.ToString("f") + "?align=right" });
            tab1.Add("border", null);

            tab.Add("width", new List<string> { "50", "50" });

            urlNew = createDoc.createWord(urlNew, tab);

            errorMesssage += "Направленно уведемлений: " + countSend + " записей.<br/><a target=\"blank\" href=\"returnFile/?urlFile=" + urlNew + "\">Скачать уведомления</a>";

            urlNew = url + "Registrys/" + DateTime.Now.ToString("yyyy_dd_MM_HH_mm_ss") + "_" + DateTime.Now.Ticks.ToString() + "_registry.docx";

            urlNew = createDoc.createWord(urlNew, tab1);

            errorMesssage += "<br/><a target=\"blank\" href=\"returnFile/?urlFile=" + urlNew + "\">Скачать реестр</a>";

            return Ok(errorMesssage);
        }

         
        /// <summary>
        /// Удаление
        /// </summary>
        /// <param name="arrayProc"></param>
        /// <returns></returns>
       
        [HttpPost]
        public async Task<IActionResult> deleteStatement(Guid[] arrayProc)
        {
            if (arrayProc.Length == 0)
                return BadRequest("Вы не выбрали ни одной записи для совершения операции!");
            string errorMesssage = "";
            string registrator = User.Identity.Name;
            int countSend = 0;
            foreach (var g in arrayProc)
            {
                if (_context.Registration.Any(x => x.RegistrationID == g))
                {
                    var reg = await _context.Registration.OrderBy(x=>x.GettingDate).FirstAsync(x => x.RegistrationID == g);

                    if (User.IsInRole("registrator2") && reg.Registrator != registrator)
                    {
                        errorMesssage += "Вы не можете редактировать запись с номером: " + reg.DocNo + "!!!<br/>";
                        continue;
                    }

                    reg.Deleted = true;

                    _context.Registration.Update(reg);

                    await _context.SaveChangesAsync();

                    countSend++;
                }
                else
                {
                    errorMesssage += "Записи с номером: " + g + " не существует!!!<br/>";
                }
            }
            errorMesssage += "Удалено: " + countSend + " записей.<br/>";
            return Ok(errorMesssage);
        }

        /// <summary>
        /// отправка уведомления по email
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public async Task<bool> SendEmailAsync(registrationModel reg)
        {
            if (!_context.Settings.Any())
                return false;

            var setting = await _context.Settings.OrderBy(x=>x.ID).FirstAsync();

            if (string.IsNullOrEmpty(setting.MailLoginForNotification) || string.IsNullOrEmpty(setting.MailPassForNotification) || string.IsNullOrEmpty(reg.e_mail))
                return false;
            var sol = await _context.Solutions.OrderBy(x=>x.Id).FirstAsync(x => x.RegistrationId == reg.RegistrationID);
            string textMessage = "Ваше заявление №" + reg.DocNo + " от " + reg.GettingDate.Value.Date.ToString("d") + " рассмотрено." + "\nВопрос решен " + sol.solution + ". " +
                (sol.solutionNumber != null ?
                "Решение № " + sol.solutionNumber :
                "" + (sol.dateOfSolution != null ?
                " от " + sol.dateOfSolution.Value.Date.ToString("d")+"." :
                "")) + "\n Служба «Одно окно»." + "\nЭто письмо сгенерировано автоматически. Не отвечайте на это письмо.";

            string emailFrom = setting.MailLoginForNotification;
            string passFrom = setting.MailPassForNotification;
            string nameFrom = setting.Name;
            string emailTo = reg.e_mail;
            string nameTo = reg.LName + " " + reg.FName + " " + reg.MName;
            string subjectMail = "Уведомление о рассмотрении поданных документов в службе «Одно окно»";
            sendEmail sendE = new();

            await sendE.SendEmailAsync(subjectMail, textMessage, emailFrom, passFrom, nameFrom, emailTo, nameTo);
            reg.Proceedings = reg.Proceedings + "Уведомление отправлено по электронной почте " + DateTime.Now;
            _context.Registration.Update(reg);
            await _context.SaveChangesAsync();
            return true;
        }

        [HttpPost]
        public IActionResult setColumnTable(List<string> viewNameColumn)
        {
            if (viewNameColumn != null)
                Response.Cookies.Append("columnViewTable", string.Join("&", viewNameColumn), new CookieOptions() { Expires = DateTime.Now.AddDays(10) });
            return Ok();
        }

        //кнопка печати
        [HttpPost]
        public IActionResult print()
        {            
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> searchRequest(string lnameSearch = null)
        {
            if (lnameSearch == null)
                return BadRequest("Введите фамилию!!!");
            if (lnameSearch.Length<3)
                return BadRequest("Введите минимум 3 символа для поиска!!!");

            Dictionary<string, List<string>> findRequest = new();

            var msg = await _context.MSG.Include(x => x.Registration).AsNoTracking().Where(x => x.LName == lnameSearch).Select(x => x.Registration).ToListAsync();

            findRequest.Add(_context.Settings.OrderBy(x => x.ID).First().Name, msg.Select(x => "№ " + x.DocNo.ToString() + " дата обращения " + x.GettingDate.Value.ToString("d") + " " + x.LName + " " + x.FName + " " + x.MName).ToList());


            //foreach (var otdel in dictionaryList.otdelList)
            //{

            //    string connectStr = " + otdel.Key + "";
            //    var contextOptions = new DbContextOptionsBuilder<oneWinDbContext>().UseSqlServer(connectStr).Options;
            //    using (var fullContextBase = new oneWinDbContext(contextOptions))
            //    {
            //        var msg = await fullContextBase.MSG.Include(x => x.Registration).AsNoTracking().Where(x => x.LName==lnameSearch).Select(x=>x.Registration).ToListAsync();
            //        findRequest.Add(otdel.Value, msg.Select(x => "№ " + x.DocNo.ToString() + " дата обращения " + x.GettingDate.Value.ToString("d") + " " + x.LName + " " + x.FName + " " + x.MName).ToList());
            //    }
            //}


            return Json(findRequest);
        }


        public async Task<IActionResult> reportForPeriod(string typeReports = "report")
        {

            string reportSrok = "";
            string reportsStr = "";
            
            var queryListCookie = _createTable.stringToDictionary(Request.Cookies["queryList"]);

            

            if (queryListCookie.Count == 0)
                return BadRequest("Для создания отчета воспользуйтесь фильтром");
            if (!queryListCookie.ContainsKey("typePerson")||queryListCookie["typePerson"]=="0")
                return BadRequest("Для создания отчета выберите тип обращения(физ., юр. лицо и т.п.)");

            int typePerson = int.Parse(queryListCookie["typePerson"]);

            string fio = "ФИО";

            if ((new int[] { 2, 6, 8, 5 }).Contains(typePerson))
                fio = "Наименование организации";

            if (typeReports == "reportSrok" && !queryListCookie.ContainsKey("StartOutdated") && !queryListCookie.ContainsKey("EndOutdated"))
                return BadRequest("Для создания отчета о нарушении сроков, выберите дату нарушения сроков");

            if (typeReports == "reportToDate" && !queryListCookie.ContainsKey("PotentiallyOutdated"))
                return BadRequest("Для создания отчета о приближении сроков готовности, выберите дату приближения сроков");
            
            if (queryListCookie.ContainsKey("page"))
                queryListCookie["page"] = "1";

            List<registrationModel> reg = null;
            var tempReg = await _createTable.createTable(queryListCookie, true);
            if (tempReg.reg.Any())
                reg = await _context.Registration.Include(x => x.sol).Include(x => x.performer).ThenInclude(x => x.Department).Where(x => tempReg.reg.Select(x => x.RegistrationID).Contains(x.RegistrationID)).ToListAsync();

            if(reg==null)
                return BadRequest("Нет данных для отображения");

            string typeReg = dictionaryList.typeReg[reg.OrderBy(x => x.GettingDate).First().TypeReg.ToString()];
           

            string registrator = User.Identity.Name;

            if (typeReports == "reportSrok")
            {
                var typeSrok = "осуществления";
                if(queryListCookie.ContainsKey("sort"))
                {
                    switch(queryListCookie["sort"])
                    {
                        case "0": typeSrok = "передачи в отдел"; break;
                        case "1": typeSrok = "осуществления"; break;
                        case "2": typeSrok = "возврата из отдела"; break;
                        case "3": typeSrok = "выдачи"; break;
                        case "4": typeSrok = "исполнения"; break;
                    }
                }
                reportSrok = " о нарушении сроков " + typeSrok + " административных процедур";
            }
                

            reportsStr = " с " + reg.Min(x => x.GettingDate.Value).ToString("d") + " по " + reg.Max(x => x.GettingDate.Value).ToString("d");

            if (typeReports == "reportToDate")
                reportsStr = " о приближении сроков осуществления административных процедур до " + queryListCookie["PotentiallyOutdated"];

            string url = new getUrlFile().urlFile();
            string urlNew = "";
            urlNew = url + "Reports/" + DateTime.Now.ToString("yyyy_dd_MM_HH_mm_ss") + "_" + DateTime.Now.Ticks.ToString() + "_ОТЧЁТ_ЗА_ПЕРИОД.docx";

            var area = _context.Settings.OrderBy(x=>x.ID).FirstOrDefault();
            createDocument createDoc = new createDocument();

            Dictionary<string, List<string>> tab = new();

            tab.Add("paragraph" + (tab.Count + 1).ToString(), new List<string> { area.Name + " отчёт о приеме " + typeReg + reportsStr + reportSrok + "?align=center" });


            if (typeReports == "report")
                tab.Add((tab.Count + 1).ToString(), new List<string> { "Номер документа?align=center", "Дата поступления?align=center", fio+"?align=center", "Адрес?align=center", "Дата передачи в отдел?align=center", "Дата возврата из отдела?align=center", "Дата выдачи?align=center", "Срок исполнения?align=center", "Регистратор?align=center" });
            if (typeReports == "reportSrok")//нарушение сроков
                tab.Add((tab.Count + 1).ToString(), new List<string> { "Номер документа?align=center", "Дата поступления?align=center", "Заявитель?align=center", "Адрес?align=center", "Дата передачи в отдел?align=center", "Дата возврата из отдела?align=center", "Дата выдачи?align=center", "Дата решения?align=center", "Срок исполнения?align=center", "Исполнитель?align=center", "Коментарий?align=center" });
            if (typeReports == "reportToDate")//приближение к дате
                tab.Add((tab.Count + 1).ToString(), new List<string> { "Номер документа?align=center", "Дата поступления?align=center", "Заявитель?align=center", "Адрес?align=center", "Дата передачи в отдел?align=center", "Срок исполнения?align=center", "Исполнитель?align=center", "Коментарий?align=center" });



            foreach (var g in reg.OrderBy(x=>x.Number).Select(x=>x.RegID).Distinct())
            {

                tab.Add((tab.Count + 1).ToString(), new List<string> { reg.Where(x => x.RegID == g).Select(x => x.Number + " " + x.RegName).OrderBy(x => x).First() });

                foreach (var r in reg.Where(s => s.RegID == g))
                {

                    string fioName = r.LName + " " + (r.FName != null ? r.FName.Substring(0, 1) + "." : "") + (!string.IsNullOrEmpty(r.MName) ? r.MName.Substring(0, 1) + "." : "");
                    if ((new int[] { 2, 6, 8, 5 }).Contains(typePerson))
                        fioName = r.OrgName;

                    if (typeReports == "report")
                        tab.Add((tab.Count + 1).ToString(), new List<string> { r.DocNo.ToString()+"?align=center", r.GettingDate.Value.ToString()+"?align=center",
                        fioName+"?align=center",
                        r.City +", "+r.Address+ ", д."+ r.Home+ ", кв."+ r.Flat+"?align=center",
                        r.OutDeptDate==null ?"": r.OutDeptDate.Value.ToString("d")+"?align=center",
                        r.ReturnInDeptDate==null?"":r.ReturnInDeptDate.Value.ToString("d")+"?align=center",
                       r.IssueDate==null?"": r.IssueDate.Value.ToString("d")+"?align=center",
                        r.MustBeReady==null?"":r.MustBeReady.Value.ToString("d")+"?align=center",
                        r.Registrator+"?align=center"});
                    if (typeReports == "reportSrok")
                        tab.Add((tab.Count + 1).ToString(), new List<string> { r.DocNo.ToString()+"?align=center", r.GettingDate.Value.ToString()+"?align=center",
                        fioName+"?align=center",
                        r.City +", "+r.Address+ ", д."+ r.Home+ ", кв."+ r.Flat+"?align=center",
                        r.OutDeptDate==null ?"": r.OutDeptDate.Value.ToString("d")+"?align=center",
                        r.ReturnInDeptDate==null?"":r.ReturnInDeptDate.Value.ToString("d")+"?align=center",
                       r.IssueDate==null?"": r.IssueDate.Value.ToString("d")+"?align=center",
                       r.sol.dateOfSolution==null?"": r.sol.dateOfSolution.Value.ToString("d")+"?align=center",
                        r.MustBeReady==null?"":r.MustBeReady.Value.ToString("d")+"?align=center",
                        r.PerformerName+"?align=center",
                        r.Notes+"?align=center"});
                    if (typeReports == "reportToDate")
                        tab.Add((tab.Count + 1).ToString(), new List<string> { r.DocNo.ToString()+"?align=center", r.GettingDate.Value.ToString()+"?align=center",
                        fioName+"?align=center",
                        r.City +", "+r.Address+ ", д."+ r.Home+ ", кв."+ r.Flat+"?align=center",
                        r.OutDeptDate==null ?"": r.OutDeptDate.Value.ToString("d")+"?align=center",                        
                        r.MustBeReady==null?"":r.MustBeReady.Value.ToString("d")+"?align=center",
                        r.PerformerName+"?align=center",
                        r.Notes+"?align=center"});
                }

                tab.Add((tab.Count + 1).ToString(), new List<string> { "Количество заявлений по процедуре: "+reg.Count(x => x.RegID == g).ToString() + (typeReports == "report" ? "" : " в " +reg.Where(s => s.RegID == g).OrderBy(x => x.GettingDate).First().performer.Department.Name) });
     
                            
            }
            tab.Add((tab.Count + 1).ToString(), new List<string> { "Итого принятых заявлений: " + reg.Count().ToString() });

            tab.Add("border", null);
            tab.Add("Landscape", null);

            urlNew = createDoc.createWord(urlNew, tab);

            return Ok(urlNew);
        }

    }
}
