using oneWin.Models.baseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models
{
    public class adminProcDetalis
    {
        public docRegModel docReg { get; set; }

        //список отдела
        public List<departmentModel> dept { get; set; }

        //ответственный
        public List<performerModel> performer { get; set; }

        //исполнителе
        public List<performerModel> performerList { get; set; }

        //документы
        public List<docsModel> docs { get; set; }

        //Документы для запросов
        public List<zaprDocModel> zarp { get; set; }

        //Нормативные документы
        public List<normDocModel> normDoc { get; set; }

        //Согласования
        public List<soglasovaniyaModel> soglasovaniyas { get; set; }
    }
}
