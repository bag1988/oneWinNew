using Microsoft.AspNetCore.Mvc;
using oneWin.Data;
using oneWin.Models.generalModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Controllers
{
    public class CalendarController : Controller
    {
        private AppDbContext _context;
        public CalendarController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult getCurrentDate(int yearCalendar, int monthCalendar)
        {
            var d = _context.Calendar.FirstOrDefault(x => x.Month == monthCalendar && x.Year == yearCalendar);

            return PartialView(d);

        }

        [HttpPost]
        public IActionResult saveCurrentDate(int yearCalendar, int monthCalendar, string daysCalendar)
        {
            calendarModel d = _context.Calendar.FirstOrDefault(x => x.Month == monthCalendar && x.Year == yearCalendar);

            if (d == null)
            {
                d = new();
                d.Year = yearCalendar;
                d.Month = monthCalendar;
                d.days = daysCalendar;
                _context.Calendar.Add(d);
            }
            else
            {
                d.days = daysCalendar;
                _context.Calendar.Update(d);
            }
            _context.SaveChanges();

            return Ok();

        }
    }
}
