using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.baseModel
{
    /// <summary>
    /// справочник размер оплаты
    /// </summary>
    
    public class siteCostModel
    {
        [Key]
        public int idCost { get; set; }

        [DisplayName("Наименование")]
        [Column(TypeName = "varchar(1000)")]
        public string name{ get;set;}
    }
}
