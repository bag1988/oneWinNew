using oneWin.Models.baseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models
{
    public class createStatement: registrationModel
    {       
        [DisplayName("Увеличить срок")]
        public bool extendSrok { get { return this.Vid == 1 ? true : false; } set { this.Vid = value==true?1:null; } }
        public List<Guid> NameDocDop { get; set; }
        public List<Guid> NameZpDop { get; set; }
    }


    public class docZapr 
    {
        public Guid regId { get; set; }
        public List<Guid> selectList { get; set; }
    }

    public class attachFile
    {
        public Guid id { get; set; }
        public string nameFile { get; set; }

        public string urlFile { get; set; }
    }
}
