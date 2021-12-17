using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.baseModel
{
    /// <summary>
    /// таблица типов улиц
    /// </summary>
    public class rvcSuvictipModel
    {       
        public int? ULTIP { get; set; }

        [StringLength(20)]
        [Column(TypeName ="varchar(20)")]
        public string FNAME { get; set; }

        [StringLength(10)]
        [Column(TypeName = "varchar(10)")]
        public string NAME { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

    }
}
