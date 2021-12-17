using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.baseModel
{
       public class bufOrgsZAprModel
    {     
       
        public Guid RegID { get; set; }
              
        public Guid ZaprDocID { get; set; }

        [Column(TypeName ="nvarchar(max)")]
        public string CheckViewSite { get; set; }

        [ForeignKey("RegID")]
        public docRegModel DocRegistry { get; set; }

        [ForeignKey("ZaprDocID")]
        public zaprDocModel ZaprDocs { get; set; }
    }
}
