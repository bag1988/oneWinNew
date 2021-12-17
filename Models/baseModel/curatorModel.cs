using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.baseModel
{
    public class curatorModel
    {
        [Key]
        public Guid Id { get; set; }

        [DisplayName("Район")]
        public Guid? Area_Id { get; set; }
        [DisplayName("Должность")]
        [Column(TypeName = "nchar(300)")]
        public string Title { get; set; }
        [Required]
        [DisplayName("ФИО")]
        [Column(TypeName = "varchar(300)")]
        public string FIO { get; set; }
        [DisplayName("Адрес")]
        [Column(TypeName = "varchar(300)")]
        public string Address { get; set; }
        [DisplayName("Кабинет")]
        [Column(TypeName = "nchar(10)")]
        public string Cabinet { get; set; }
        [DisplayName("Телефон")]
        [Column(TypeName = "nchar(50)")]
        public string Phone { get; set; }


        [Column("e-mail", TypeName = "nchar(50)")]
        [DisplayName("E-mail")]
        public string e_mail { get; set; }

        [DisplayName("Дополнительная информация")]
        [Column(TypeName = "text")]
        public string Notes { get; set; }
        [DisplayName("Номер по порядку")]
        public int? Number { get; set; }

        [ForeignKey("Area_Id")]
        public areaModel Areas { get; set; }
    }
}
