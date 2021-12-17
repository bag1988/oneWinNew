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
    /// Справочник сроков действия документов
    /// </summary>
    public class siteValidatyModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Наименование")]
        [Column(TypeName = "varchar(1000)")]
        public string Name { get; set; }
    }
}
