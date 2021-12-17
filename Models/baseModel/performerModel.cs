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
    /// Исполнители процедур
    /// </summary>
    public class performerModel
    {
        [Key]
        public Guid Id { get; set; }

        [DisplayName("Должность")]
        [Column(TypeName ="varchar(300")]
        public string Title { get; set; }
        [Required]
        [DisplayName("ФИО исполнителя")]
        [Column(TypeName = "varchar(300")]
        public string FIO { get; set; }
        [DisplayName("Адрес")]
        [Column(TypeName = "varchar(300")]
        public string Address { get; set; }
        [DisplayName("Кабинет")]
        [Column(TypeName = "nchar(10")]
        public string Cabinet { get; set; }
        [DisplayName("Телефон")]
        [Column(TypeName = "nchar(50")]
        public string Phone { get; set; }
        [DisplayName("E-mail")]
        [Column(name: "e-mail", TypeName = "nchar(30")]
        public string e_mail { get; set; }
        [DisplayName("Дополнительная информация")]
        [Column(TypeName = "text")]
        public string Notes { get; set; }
        [DisplayName("Номер по порядку")]
        public int? Number { get; set; }
        [Required]
        [DisplayName("Отдел")]
        public Guid? Department_ID { get; set; }

        [ForeignKey("Department_ID")]
        public departmentModel Department { get; set; }
        public bool? CheckPerName { get; set; }
        public bool? CheckPerTitle { get; set; }
        public bool? CheckPerAddress { get; set; }
        public bool? CheckPerCabinet { get; set; }
        public bool? CheckPerPhone { get; set; }
        public bool? CheckPermail { get; set; }

        public bool? CheckPerNum { get; set; }
        public bool? ChekPerNotes { get; set; }
    }
}
