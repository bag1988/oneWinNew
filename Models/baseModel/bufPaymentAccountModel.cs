using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.baseModel
{    
    public class bufPaymentAccountModel
    {
        public Guid? IdDoc { get; set; }

        public int IdPayment { get; set; }

        [ForeignKey("IdDoc")]
        public docRegModel DocReg { get; set; }

        [ForeignKey("IdPayment")]
        public paymentAccountModel payment { get; set; }
    }
}
