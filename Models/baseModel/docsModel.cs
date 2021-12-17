using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.baseModel
{
    public class docsModel
    {
        [Key]
        public Guid DocID { get; set; }
               
        [Column(TypeName = "varchar(1500)")]
        [DisplayName("Наименование")]
        [StringLength(1500)]
        public string Name { get; set; }

        [DisplayName("Кол-во листов")]
        public byte? CountList { get; set; }
    }
}
