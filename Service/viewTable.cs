using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using oneWin.Data;
using oneWin.Models;
using oneWin.Models.baseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace oneWin.Service
{
    public class viewTable
    {
        private readonly oneWinDbContext _context;

        public viewTable(oneWinDbContext context)
        {
            _context = context;
        }

        //формируем таблицу
        public async Task<IndexViewModel> createTable(string quertyString)
        {
            Dictionary<string, string> queryList = stringToDictionary(quertyString);
            return await createTable(queryList);
        }

        //формируем таблицу
        public async Task<IndexViewModel> createTable(Dictionary<string, string> queryList, bool? allView = false)
        {
            if (queryList.ContainsKey("otdel"))
            {
                queryList.Remove("otdel");
            }
            searchFastModel searchModel = new();
            FilterViewModel filterModel = new();
            int page = 1;

            if (queryList.ContainsKey("page"))
                page = int.Parse(queryList["page"]);
            HttpContext Current = new HttpContextAccessor().HttpContext;
            if (Current == null)
                return null;
            var queryListCookie = stringToDictionary(Current.Request.Cookies["queryList"]);

            if (queryList.ContainsKey("sortTable"))
            {
                if (!queryListCookie.ContainsKey("sortTable"))
                {
                    queryListCookie.Add("sortTable", queryList["sortTable"]);
                    if(queryList["sortTable"]== "GettingDate")
                        queryListCookie.Add("sortDesc", (true).ToString());
                }
                else
                {
                    if (!queryListCookie.ContainsKey("sortDesc"))
                        queryListCookie.Add("sortDesc", (queryListCookie["sortTable"] == queryList["sortTable"]).ToString());
                    else
                        queryListCookie["sortDesc"] = ((queryListCookie["sortTable"] == queryList["sortTable"]).ToString() == queryListCookie["sortDesc"] ? false : true).ToString();

                    queryListCookie["sortTable"] = queryList["sortTable"];
                }
            }
            else
            {
                if (queryListCookie.ContainsKey("sortTable"))
                {
                    queryList.Add("sortTable", queryListCookie["sortTable"]);
                }
            }

            if (queryList.ContainsKey("typePerson"))
            {
                if (!queryListCookie.ContainsKey("typePerson"))
                    queryListCookie.Add("typePerson", queryList["typePerson"]);
                else
                    queryListCookie["typePerson"] = queryList["typePerson"];
            }
            else
            {
                if (queryListCookie.ContainsKey("typePerson"))
                {
                    queryList.Add("typePerson", queryListCookie["typePerson"]);
                }
            }

            if (queryList.ContainsKey("stateStatment"))
            {
                if (!queryListCookie.ContainsKey("stateStatment"))
                    queryListCookie.Add("stateStatment", queryList["stateStatment"]);
                else
                    queryListCookie["stateStatment"] = queryList["stateStatment"];
            }
            else
            {
                if (queryListCookie.ContainsKey("stateStatment"))
                {
                    queryList.Add("stateStatment", queryListCookie["stateStatment"]);
                }
            }


            if (!queryList.Any(x => x.Key != "page" && x.Key != "stateStatment" && x.Key != "typePerson" && x.Key != "sortTable"))
            {
                if (queryListCookie.Count > 0)
                {
                    foreach (var q in queryListCookie.Where(x => x.Key != "page" && x.Key != "stateStatment" && x.Key != "typePerson" && x.Key != "sortTable"))
                    {
                        queryList.Add(q.Key, q.Value);
                    }
                }
            }

            if (queryList.Any(x => x.Key != "page"))
            {
                searchModel = (searchFastModel)dictionaryToModel(queryList, searchModel);
                filterModel = (FilterViewModel)dictionaryToModel(queryList, filterModel);
            }

            bool replaseCookie = false;
            if (page == -1 || page == 0)
            {

                if (page == -1)
                {
                    queryList = queryList.Where(x => !filterModel.GetType().GetProperties().Any(m => m.Name == x.Key)).ToDictionary(x => x.Key, x => x.Value);
                    filterModel = new();

                }
                if (page == 0)
                {
                    int? typePerson = null;
                    int? stateStatment = null;

                    typePerson = searchModel.typePerson;
                    stateStatment = searchModel.stateStatment;
                    queryList = queryList.Where(x => !searchModel.GetType().GetProperties().Where(x=>x.Name!= "typePerson"&&x.Name!= "stateStatment").Select(s=>s.Name).Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
                    searchModel = new();

                    searchModel.stateStatment = stateStatment;
                    searchModel.typePerson = typePerson;

                }
                replaseCookie = true;
                page = 1;
            }


            if (queryList.Any(x => x.Key != "page")|| replaseCookie)
                Current.Response.Cookies.Append("queryList", string.Join("&", queryList.Select(x => x.Key + "=" + x.Value)), new CookieOptions() { Expires = DateTime.Now.AddDays(10) });

            var table = await createTable(page, (bool)allView, searchModel, filterModel);
            return table;
        }

        //формируем таблицу
        private async Task<IndexViewModel> createTable(int page = 1, bool allView = false, searchFastModel searchFast = null, FilterViewModel filter = null)
        {
            int pageSize = 200;   // количество элементов на странице
            bool search = isNullModel(searchFast); //проверяем на null
            bool filtr = isNullModel(filter); //проверяем на null
            bool sortDesc = false;
            IEnumerable<regModel> items = new List<regModel>();

            string sort = "GettingDate";

            Expression<Func<registrationModel, bool>> exSearch = null;
            Expression<Func<registrationModel, bool>> exFiltr = null;
            Expression<Func<registrationModel, bool>> exSrok = null;

            var count = 0;
            if (search)
            {
                if (searchFast.stateStatment == 0)
                    searchFast.stateStatment = null;
                if (searchFast.typePerson == 0)
                    searchFast.typePerson = null;
                if (searchFast.sortTable != null)
                    sort = searchFast.sortTable;
                if (searchFast.sortDesc != null)
                    sortDesc = (bool)searchFast.sortDesc;
                exSearch = x =>
                //быстрый поиск

                (searchFast.LNameFilter == null || (x.LName.Contains(searchFast.LNameFilter) || x.FName.Contains(searchFast.LNameFilter) || x.MName.Contains(searchFast.LNameFilter)))
                && (searchFast.DocNoFilter == null || x.DocNo == searchFast.DocNoFilter)
                && (searchFast.OrgNameFilter == null || x.OrgName.Contains(searchFast.OrgNameFilter))
                && (searchFast.PhoneNoFilter == null || (x.PhoneNo.Contains(searchFast.PhoneNoFilter) || x.MobPhone.Contains(searchFast.PhoneNoFilter)))
                && (searchFast.AddressFilter == null || x.Address.Contains(searchFast.AddressFilter))
                && (searchFast.NumberFilter == null || x.Number.Contains(searchFast.NumberFilter))
                && (searchFast.GettingDateFilter == null || x.GettingDate.Value.Date == searchFast.GettingDateFilter)
                && (searchFast.DateSsolutionsFilter == null || x.DateSsolutions.Value.Date == searchFast.DateSsolutionsFilter)
                && (searchFast.OutDeptDateFilter == null || x.OutDeptDate.Value.Date == searchFast.OutDeptDateFilter)
                && (searchFast.ReturnInDeptDateFilter == null || x.ReturnInDeptDate.Value.Date == searchFast.ReturnInDeptDateFilter)
                && (searchFast.NotificationDateFilter == null || x.NotificationDate.Value.Date == searchFast.NotificationDateFilter)
                && (searchFast.IssueDateFilter == null || x.IssueDate.Value.Date == searchFast.IssueDateFilter)
                && (searchFast.MustBeReadyFilter == null || x.MustBeReady.Value.Date == searchFast.MustBeReadyFilter)
                && (searchFast.PersonalNoFilter == null || x.PersonalNo.Contains(searchFast.PersonalNoFilter))
                && (searchFast.emailFilter == null || x.e_mail.Contains(searchFast.emailFilter))
                && (searchFast.RegistratorFilter == null || x.Registrator.Contains(searchFast.RegistratorFilter))
                && (searchFast.PerformerNameFilter == null || x.PerformerName.Contains(searchFast.PerformerNameFilter))
                && (searchFast.SolutionFilter == null || x.sol.solution.Contains(searchFast.SolutionFilter))
                && (searchFast.typePerson == null || x.TypeReg == searchFast.typePerson)
                && (searchFast.stateStatment == null || (searchFast.stateStatment == 5 ? x.Deleted == true : (x.State == searchFast.stateStatment && x.Deleted != true)))
                && ((searchFast.NotesFilter == null || searchFast.NotesFilter == "") || (x.Notes != "" ? Convert.ToString(x.Notes).Contains(searchFast.NotesFilter) : false));
            }
            if (filtr)
            {
                exFiltr = x =>
                 (filter.startNumber == null || x.DocNo >= filter.startNumber)
                 && (filter.endNumber == null || x.DocNo <= filter.endNumber)
                 && (filter.LName == null || x.LName.Contains(filter.LName))
                 && (filter.FName == null || x.FName.Contains(filter.FName))
                 && (filter.MName == null || x.MName.Contains(filter.MName))
                 && (filter.OrgName == null || x.OrgName.Contains(filter.OrgName))
                 && (filter.PhoneNo == null || (x.PhoneNo.Contains(filter.PhoneNo) || x.MobPhone.Contains(filter.PhoneNo)))
                 && (filter.Address == null || x.Address.Contains(filter.Address))
                 && (filter.City == null || x.City.Contains(filter.City))
                 && (filter.Home == null || x.Home.Contains(filter.Home))
                 && (filter.Flat == null || x.Flat.Contains(filter.Flat))
                 && (filter.StartGetDateTime == null || x.GettingDate.Value.Date >= filter.StartGetDateTime)
                 && (filter.EndGetDateTime == null || x.GettingDate.Value.Date <= filter.EndGetDateTime)
                 && (filter.StartOutDeptDate == null || x.OutDeptDate.Value.Date >= filter.StartOutDeptDate)
                 && (filter.EndOutDeptDate == null || x.OutDeptDate.Value.Date <= filter.EndOutDeptDate)
                 && (filter.StartIssueDate == null || x.IssueDate.Value.Date >= filter.StartIssueDate)
                 && (filter.EndIssueDate == null || x.IssueDate.Value.Date <= filter.EndIssueDate)
                 && (filter.StartReturnInDeptDate == null || x.ReturnInDeptDate.Value.Date >= filter.StartReturnInDeptDate)
                 && (filter.EndReturnInDeptDate == null || x.ReturnInDeptDate.Value.Date <= filter.EndReturnInDeptDate)
                 && (filter.StartMustBeReady == null || x.MustBeReady.Value.Date >= filter.StartMustBeReady)
                 && (filter.EndMustBeReady == null || x.MustBeReady.Value.Date <= filter.EndMustBeReady)
                 && (filter.SentNotificationBeginDateFilter == null || x.NotificationDate.Value.Date >= filter.SentNotificationBeginDateFilter)
                 && (filter.SentNotificationEndDateFilter == null || x.NotificationDate.Value.Date <= filter.SentNotificationEndDateFilter)
                 && (filter.SentNotificationRegistryNumberFilter == null || x.NotificationRegistryNumber.Contains(filter.SentNotificationRegistryNumberFilter))
                 && (filter.Notes == null || x.Notes.Contains(filter.Notes))
                 && (filter.Proceeding == null || x.Proceedings.Contains(filter.Proceeding))
                 && (filter.Registrator == null || x.Registrator.Contains(filter.Registrator))
                 && (filter.CuratorFiltr == null || x.performer.Department.Curator_Id == filter.CuratorFiltr)
                 && (filter.DepartamentFiltr == null || x.performer.Department_ID == filter.DepartamentFiltr)
                 && (filter.PerformerFiltr == null || x.Organiz == filter.PerformerFiltr)
                 && (filter.HeadsID == null || x.DocRegistry.HeadsID == filter.HeadsID)
                 && (filter.SectionID == null || x.DocRegistry.SectionID == filter.SectionID)
                 && (filter.DocRegList == null || filter.DocRegList.Contains((Guid)x.RegID))
                 && (filter.NumberSolution == null || x.sol.solutionNumber.Contains(filter.NumberSolution))
                 && (filter.DateSolution == null || x.sol.dateOfSolution.Value.Date == filter.DateSolution.Value.Date)
                 && (filter.SolutionFilter == null || x.sol.solution.Contains(filter.SolutionFilter))
                 && (filter.EvaluationNotificationFilter == null || x.EvaluationNotification.Contains(filter.EvaluationNotificationFilter))
                 && (filter.PotentiallyOutdated == null || (x.MustBeReady.Value.Date <= filter.PotentiallyOutdated.Value.Date && x.State == 2));



                if (filter.StartOutdated != null || filter.EndOutdated != null)
                {
                    //WorkingDay w = new(_context);
                    exSrok = x =>
                             ((filter.sort != null && filter.sort != 6
                                && (
                                    (filter.sort == 0 && (x.OutDeptDate.Value > x.MustBeReady.Value || (x.OutDeptDate == null && x.State == 1 && x.ReturnInDeptDate == null && x.IssueDate == null)))
                                    || (filter.sort == 1 && ((x.State == 2
                                        && (x.sol.dateOfSolution == null || x.sol.dateOfSolution.Value.Year < 1990)
                                        && (x.ReturnInDeptDate == null || x.ReturnInDeptDate.Value.Year < 1990)
                                        && (x.IssueDate == null || x.IssueDate.Value.Year < 1990)
                                        && x.MustBeReady.Value < DateTime.Now)
                                        ||
                                        (
                                        x.sol.dateOfSolution > x.MustBeReady
                                        )
                                        ||
                                        (
                                        (x.sol.dateOfSolution == null || x.sol.dateOfSolution.Value.Year < 1990)
                                        && x.ReturnInDeptDate > x.MustBeReady
                                        )
                                        ||
                                        (
                                        (x.sol.dateOfSolution == null || x.sol.dateOfSolution.Value.Year < 1990)
                                        && (x.ReturnInDeptDate == null || x.ReturnInDeptDate.Value.Year < 1990)
                                        && x.OutDeptDate > x.MustBeReady
                                        )
                                    ))
                                    || (filter.sort == 2 && (
                                        ((x.OutDeptDate < x.MustBeReady)
                                        && (x.sol.dateOfSolution == null || x.sol.dateOfSolution.Value.Year < 1990)
                                        && (x.ReturnInDeptDate > x.MustBeReady)
                                        )
                                        ||
                                        (
                                        (x.OutDeptDate < x.MustBeReady)
                                        && (x.sol.dateOfSolution.Value.Year > 1990)
                                        && (x.ReturnInDeptDate.Value.Year > 1990)
                                        && (x.sol.dateOfSolution.Value.AddDays(8) < x.ReturnInDeptDate)
                                        )
                                        ||
                                        (
                                        x.ReturnInDeptDate == null && x.IssueDate == null && x.MustBeReady < DateTime.Now && x.State == 2
                                        )
                                    ))
                                    || (filter.sort == 3 &&
                                        ((x.GettingDate.Value.Date != x.MustBeReady.Value.Date)
                                        &&
                                            (((x.sol.dateOfSolution == null || x.sol.dateOfSolution.Value.Year < 1990)
                                            && (x.ReturnInDeptDate == null || x.IssueDate > x.ReturnInDeptDate.Value.AddDays(8)))
                                            ||
                                            (x.sol.dateOfSolution.Value.Year > 1990
                                            && x.IssueDate.Value > x.sol.dateOfSolution.Value.AddDays(8))
                                            ||
                                            (
                                            x.IssueDate == null && x.State == 3 && x.MustBeReady.Value < DateTime.Now
                                            )
                                            )
                                        )
                                    )
                                    || (filter.sort == 4 &&
                                        (x.sol.dateOfSolution.Value.Year > 1990
                                        && x.sol.dateOfSolution.Value < x.MustBeReady.Value)
                                        &&
                                            (
                                            (x.ReturnInDeptDate.Value.Year > 1990
                                            && x.ReturnInDeptDate.Value > x.MustBeReady.Value)
                                            ||
                                            (
                                            (x.IssueDate == null || x.IssueDate.Value.Year < 1990)
                                            && (x.OutDeptDate == null || x.OutDeptDate.Value.Year < 1990)
                                            )
                                            )
                                    )
                                )
                            )
                            ||
                            (filter.sort == null
                                && ((x.OutDeptDate.Value > x.MustBeReady.Value
                                || (x.State == 1 && x.OutDeptDate == null && x.ReturnInDeptDate == null && x.IssueDate == null)
                                )
                                || (
                                    x.OutDeptDate.Value < x.MustBeReady.Value && x.sol.dateOfSolution.Value > x.MustBeReady.Value
                                )
                                || (
                                (x.OutDeptDate.Value < x.MustBeReady.Value && (x.sol.dateOfSolution == null || x.sol.dateOfSolution.Value.Year < 1990)
                                    && x.ReturnInDeptDate.Value > x.MustBeReady.Value
                                )
                                || (x.OutDeptDate.Value < x.MustBeReady.Value && x.sol.dateOfSolution.Value.Year > 1990
                                    && x.sol.dateOfSolution.Value.AddDays(8) < x.ReturnInDeptDate.Value
                                )
                                || (
                                    x.State == 2 && DateTime.Now > x.MustBeReady.Value && x.ReturnInDeptDate == null && x.IssueDate == null
                                )
                                )
                                || (
                                    (((x.sol.dateOfSolution == null || x.sol.dateOfSolution.Value.Year < 1990)
                                        && x.IssueDate.Value > x.ReturnInDeptDate.Value.AddDays(8)
                                    )
                                    || (x.sol.dateOfSolution.Value.Year > 1990 && x.IssueDate.Value > x.sol.dateOfSolution.Value.AddDays(8))
                                    || (x.IssueDate == null && x.State == 3 && DateTime.Now > x.MustBeReady.Value)
                                    )
                                     && (DateTime.Now.Date != x.MustBeReady.Value.Date)
                                )
                                )
                            )
                            || (filter.sort == 5
                            && (x.MustBeReady.Value < x.ReturnInDeptDate.Value || x.MustBeReady.Value < x.sol.dateOfSolution.Value
                                || (x.State == 1 && x.OutDeptDate == null && x.ReturnInDeptDate == null && x.IssueDate == null)
                                || (x.State == 2 && x.ReturnInDeptDate == null && x.IssueDate == null))
                            )
                            )
                            && ((filter.StartOutdated == null || x.MustBeReady.Value.Date >= filter.StartOutdated.Value.Date)
                            && (filter.EndOutdated == null || x.MustBeReady.Value.Date <= filter.EndOutdated.Value.Date))
                            && x.Deleted != true
                    ;
                    var invoked = Expression.Invoke(exSrok, exFiltr.Parameters.Cast<Expression>());
                    exFiltr = Expression.Lambda<Func<registrationModel, bool>>(Expression.AndAlso(exFiltr.Body, invoked), exFiltr.Parameters);
                }
            }

            Expression<Func<registrationModel, bool>> ex = null;


            if (search && filtr)
            {
                var invoked = Expression.Invoke(exSearch, exFiltr.Parameters.Cast<Expression>());
                ex = Expression.Lambda<Func<registrationModel, bool>>(Expression.AndAlso(exFiltr.Body, invoked), exFiltr.Parameters);
            }
            else
                ex = exSearch ?? exFiltr;

            count = await _context.Registration.Include(x => x.sol).Include(x => x.performer).ThenInclude(x => x.Department).ThenInclude(x => x.Curators).AsNoTracking().Where(ex ?? (x => true)).CountAsync();

            if ((int)Math.Ceiling(count / (double)pageSize) < page)
                page = 1;

            if (allView)
            {
                pageSize = count;
            }

            if (pageSize > 3000)
                pageSize = 3000;
            if (count > 0)
            {
                items = await _context.Registration.Include(x => x.suspend).Include(x => x.sol).Include(x => x.performer).ThenInclude(x => x.Department).ThenInclude(x => x.Curators)
                   .AsNoTracking()
                   .Where(ex ?? (x => true))
                   .OrderBy(x => (sortDesc == true ? sort == "Solution" ? x.sol.solution : EF.Property<object>(x, sort) : 0))
                   .ThenByDescending(x => (sortDesc == false ? sort == "Solution" ? x.sol.solution : EF.Property<object>(x, sort) : 0))
                   .ThenBy(x=>x.GettingDate)
                   .Select(x => new regModel
                   {
                       RegistrationID = x.RegistrationID,
                       DocNo = x.DocNo,
                       LName = x.LName + " " + (x.FName != null ? x.FName.Substring(0, 1) + "." : "") + (!string.IsNullOrEmpty(x.MName) ? x.MName.Substring(0, 1) + "." : ""),
                       OrgName = x.OrgName,
                       PhoneNo = x.PhoneNo + " " + x.MobPhone,
                       Address = x.City + " " + x.Address + " д." + x.Home + " кв." + x.Flat,
                       GettingDate = (DateTime)x.GettingDate,
                       OutDeptDate = x.OutDeptDate != null ? x.OutDeptDate.Value.ToString("d") : "",
                       ReturnInDeptDate = x.ReturnInDeptDate != null ? x.ReturnInDeptDate.Value.ToString("d") : "",
                       NotificationDate = x.NotificationDate != null ? x.NotificationDate.Value.ToString("d") : "",
                       IssueDate = x.IssueDate != null ? x.IssueDate.Value.ToString("d") : "",
                       MustBeReady = x.MustBeReady,
                       PerformerName = x.PerformerName,
                       Number = x.Number,
                       RegName = x.RegName,
                       Registrator = x.Registrator,
                       Solution = x.sol.solution,
                       State = x.State,
                       Notes = x.Notes,
                       DateSsolutions = (x.sol.solutionNumber != null ? "№ " + x.sol.solutionNumber + " " : "") + ((x.sol.dateOfSolution != null && x.sol.dateOfSolution.Value.Year > 1990) ? "от " + x.sol.dateOfSolution.Value.ToString("d") : ""),
                       Deleted = x.Deleted,
                       stop = x.suspend.Any(s=>s.stopDate!=null && s.beginDate==null)? x.suspend.OrderByDescending(s=>s.stopDate).First(s=>s.stopDate!=null && s.beginDate==null).stopDate.Value.ToString("d"):null
                   })
                   .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            }

            if (!search)
                searchFast = null;
            if (!filtr)
                filter = null;



            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            IndexViewModel viewModel = new IndexViewModel
            {
                FilterViewModel = filter,
                Search = searchFast,
                reg = items,
                PageViewModel = pageViewModel
            };            
            return viewModel;
        }

        //проверяем модель на null
        private bool isNullModel(object obj)
        {
            if (obj != null)
            {
                if (obj.GetType().GetProperties().Any(x => x.GetValue(obj) != null))
                    return true;
            }
            return false;
        }


        //конвертируем параметры в модель
        public Object dictionaryToModel(Dictionary<string, string> queryList, Object model)
        {
            if (queryList != null && queryList.Any())
            {
                foreach (var query in queryList.Where(x => x.Value != null && x.Value != ""))
                {
                    if (model.GetType().GetProperties().Any(x => x.Name == query.Key))
                    {
                        var prop = model.GetType().GetProperties().Single(x => x.Name == query.Key);
                        var t = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                        var valQuery = query.Value;
                        if(query.Key== "DocRegList")
                        {
                            List<Guid> l = new List<Guid>();
                            l.AddRange(valQuery.Split(',').Select(x=> Guid.Parse(x)).ToList());
                            prop.SetValue(model, l);
                        }
                        else if (t.Name == "Guid")
                        {
                            prop.SetValue(model, Guid.Parse(valQuery));
                        }
                        else
                            prop.SetValue(model, Convert.ChangeType(valQuery, t));
                    }
                }
            }
            return model;
        }

        public Dictionary<string, string> stringToDictionary(string queryString)
        {
            Dictionary<string, string> queryList = new();
            if (queryString != null && queryString != "")
            {
                queryList = System.Net.WebUtility.UrlDecode(queryString).TrimStart('?').Split("&").Where(x => x.Split("=")[1] != "").ToDictionary(x => x.Split("=")[0], x => x.Split("=")[1]);
            }
            return queryList;
        }
    }
}
