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
    /// Организации для запросов
    /// </summary>
    public class orgsZaprModel
    {
        [Key]
        public Guid OrgZaprID { get; set; }
               
        [Column(TypeName="varchar(500)")]
        [StringLength(500)]
        [DisplayName("Название")]
        public string Name { get; set; }

        [Column(TypeName = "varchar(40)")]
        [StringLength(40)]
        [DisplayName("Электронный адрес")]
        public string e_mail { get; set; }

        [Column(TypeName = "text")]
        [DisplayName("Почтовый адрес")]
        public string PostAddress { get; set; }

        [Column(TypeName = "varchar(15)")]
        [StringLength(15)]
        [DisplayName("Телефон")]
        public string Telephone { get; set; }

        [Column(TypeName = "nchar(1)")]
        [StringLength(1)]
        public string TypeRequest { get; set; }

        [Column(TypeName = "text")]
        public string HTTP { get; set; }

        [Column(TypeName = "text")]
        public string Params { get; set; }

      
    }
}
