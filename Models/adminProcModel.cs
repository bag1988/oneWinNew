using oneWin.Models.baseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models
{
    public class adminProcModel
    {
        public headModel head { get; set; }

        public List<sectionAdminProc> section { get; set; }
    }

    public class sectionAdminProc
    {
        public sectionsModel section { get; set; }
        public List<docRegModel> docReg { get; set; }
    }


    public class adminProcForPerformer
    {
        //исполнитель процедуры
        public List<docRegModel> DocsReg { get; set; }
        //ответственный
        public List<docRegModel> DocsResponsible { get; set; }
        //прием документов
        public List<docRegModel> DocumentsAccept { get; set; }
    }

    public class viewAdminProcForPerformer
    {
        public List<Guid> docReg { get; set; }
        public List<Guid> docRes { get; set; }
        public List<Guid> docAcc { get; set; }
        public List<adminProcModel> adminProcList { get; set; }
    }
}
