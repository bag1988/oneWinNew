using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.baseModel
{
       public class bufNormDocModel
    {       
        public Guid? NormID { get; set; }
       
        public Guid? RegID { get; set; }

        [ForeignKey("RegID")]
        public docRegModel DocRegistry { get; set; }

        [ForeignKey("NormID")]
        public normDocModel NormDoc { get; set; }
    }
}
