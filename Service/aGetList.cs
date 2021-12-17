using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using oneWin.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Service
{
    
    public class aGetList
    {
        private AppDbContext _acontext;

        public aGetList(AppDbContext acontext)
        {
            _acontext = acontext;
        }

        public async Task<SelectList> getUserLogList(DateTime? date=null)
        {
            var list = await logger.getUserLogList(date);
            return new SelectList(list.Select(r => new SelectListItem
            {
                Text = r,
                Value = r
            }), "Value", "Text");
        }

        public async Task<SelectList> getControllerLogList(DateTime? date = null)
        {
            var contName = await _acontext.controller.AsNoTracking().ToListAsync();
            var list = await logger.getControllerLogList(date);
            return new SelectList(contName.Where(x=>list.Contains(x.addressController.ToLower())).Select(r => new SelectListItem
            {
                Text = r.nameController,
                Value = r.addressController
            }), "Value", "Text");
        }

        public async Task<SelectList> getActionLogList(DateTime? date = null)
        {
            var list = await logger.getActionLogList(date);
            return new SelectList(list.Select(r => new SelectListItem
            {
                Text = r,
                Value = r
            }), "Value", "Text");
        }

        public async Task<SelectList> getOtdelLogList(DateTime? date = null)
        {            
            var list = await logger.getOtdelLogList(date);
            return new SelectList(list.Select(r => new SelectListItem
            {
                Text = r.Value,
                Value = r.Key
            }), "Value", "Text");
        }


    }
}
