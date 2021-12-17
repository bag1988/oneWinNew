using Microsoft.EntityFrameworkCore;
using oneWin.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Service
{
    public class WorkingDay
    {
        private readonly AppDbContext _context;

        public WorkingDay(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Проверяем рабочие дни
        /// </summary>
        /// <param name="countday">кол-во дней</param>
        /// <param name="typeIssue">тип(дни, недели..)</param>
        /// <param name="GettingDate"></param>
        /// <returns></returns>
        public async Task<DateTime> GetDay(int countday, int typeIssue, DateTime GettingDate)
        {            
            countday = countday > 0 ? countday : 0;
            DateTime IssueDate;
            DateTime endDate;
            DateTime startDate;
            switch (typeIssue)
            {
                //календарные дни
                case 1:
                    IssueDate = GettingDate.AddDays(countday);
                    return await CheckDay(IssueDate);
                //рабочие дни
                case 2:
                    endDate = GettingDate.AddDays(countday);
                    startDate = GettingDate;
                    return await CheckDay(startDate, endDate);
                //месяц
                case 3:
                    IssueDate = GettingDate.AddMonths(countday);
                    return await CheckDay(IssueDate);
                //неделя
                case 4:
                    IssueDate = GettingDate.AddDays(countday * 7);
                    return await CheckDay(IssueDate);
                //календарные дни со дня подачи
                case 5:
                    GettingDate = GettingDate.AddDays(-1);
                    IssueDate = GettingDate.AddDays(countday);
                    return await CheckDay(IssueDate);
                //рабочие дни со дня подачи
                case 6:
                    startDate = GettingDate.AddDays(-1);
                    endDate = startDate.AddDays(countday);
                    return await CheckDay(startDate, endDate);
                //месяц со дня подачи
                case 7:
                    GettingDate = GettingDate.AddDays(-1);
                    IssueDate = GettingDate.AddMonths(countday);
                    return await CheckDay(IssueDate);
                //неделя со дня подачи
                case 8:
                    GettingDate = GettingDate.AddDays(-1);
                    IssueDate = GettingDate.AddDays(countday * 7);
                    return await CheckDay(IssueDate);
                default:
                    return GettingDate;
            }
        }

        private async Task<DateTime> CheckDay(DateTime startDate, DateTime? endDate = null)
        {
            if (endDate == null)
                endDate = startDate;

            var calendarList = await _context.Calendar.AsNoTracking().ToListAsync();
            int outInt = 0;
            while (startDate.Date <= endDate.Value.Date)
            {
                if (calendarList.Any(x => x.Month == startDate.Month && x.Year == startDate.Year))
                {
                    if (calendarList.First(x => x.Month == startDate.Month && x.Year == startDate.Year).days != null && calendarList.First().days != "")
                    {
                        if (calendarList.First(x => x.Month == startDate.Month && x.Year == startDate.Year).days.Split(',').Where(x => int.TryParse(x, out outInt)).Select(x => int.Parse(x)).Contains(startDate.Date.Day))
                        {
                            endDate = endDate.Value.AddDays(1);
                        }
                    }
                }
                startDate = startDate.AddDays(1);
            }
            return endDate.Value;
        }
    }
}
