using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.baseModel
{
    /// <summary>
    /// таблица улиц
    /// </summary>
    public class rvcSulicModel
    {
        public int? KODUL { get; set; }

        [Column(TypeName ="varchar(20)")]
        [StringLength(20)]
        public string ULTIP { get; set; }

        [Column(TypeName = "varchar(40)")]
        [StringLength(40)]
        public string NAME { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
    }
}
