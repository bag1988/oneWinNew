using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.baseModel
{
    /// <summary>
    /// таблица улиц и домов
    /// </summary>
    public class tempModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ULTIP { get; set; }
        [Column(TypeName ="varchar(20")]
        public string FNAME { get; set; }
        [Column(TypeName = "varchar(10")]
        public string NAMETip { get; set; }
        [Column(TypeName = "nvarchar(6")]
        public string NDOM { get; set; }
        [Column(TypeName = "nvarchar(6")]
        public string FLAT { get; set; }
        [Column(TypeName = "varchar(10")]
        public string accountNumber { get; set; }
        public int KODUL { get; set; }


        [Column(TypeName = "nvarchar(40")]
        public string NAME { get; set; }
    }
}
