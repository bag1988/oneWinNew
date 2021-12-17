using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.baseModel
{
    public class docRegModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("Идентификатор")]
        public Guid RegID { get; set; }

        [Required]
        [ForeignKey("Sections")]
        [DisplayName("Раздел")]
        public Guid? SectionID { get; set; }

        [DisplayName("Раздел")]
        public sectionsModel Sections { get; set; }

        [Required]
        [DisplayName("Номер")]
        [Column(TypeName = "varchar(15")]
        [StringLength(15)]
        public string Number { get; set; }

        [Required]
        [DisplayName("Название процедуры")]
        [Column(TypeName = "nvarchar(max")]
        public string RegName { get; set; }

        [DisplayName("Не известно что это такое")]
        public Guid? ParrentID { get; set; }
       

        [Required]
        [DisplayName("Срок исполнения")]
        public int? IssueTerms { get; set; }

        [DisplayName("Описание")]
        [Column(TypeName = "varchar(500")]
        [StringLength(500)]
        public string Notes { get; set; }


        [DisplayName("Непонятная дата?????????")]
        public bool? GetDataFromRVC { get; set; }

        [DisplayName("Для юр. лиц")]
        public bool IP { get; set; }

        [DisplayName("Непонятно?????????")]
        [Column(TypeName = "varchar(15")]
        [StringLength(15)]
        public string StatementForm { get; set; }

        [DisplayName("Наверное удаление")]
        public bool? Deleted { get; set; }

        [DisplayName("Нет данных в базе данных")]
        public byte? PeriodType { get; set; }

        [DisplayName("Неведома хератень")]
        public byte? KolList { get; set; }


        [Required]
        [DisplayName("Глава")]
        [ForeignKey("Heads")]
        public Guid? HeadsID { get; set; }

        [DisplayName("Глава")]
        public headModel Heads { get; set; }

        [DisplayName("Доступна для выбора")]
        public bool Selected { get; set; }


        [DisplayName("Шаблон")]
        [Column(TypeName = "varchar(200")]
        [StringLength(200)]
        [RegularExpression(@"^[\w\d_А-Яа-я]*[\.]?[\w]{0,5}", ErrorMessage = "Допускается использовать только буквы и цифры, а также знак подчеркивания (_). Пример шаблон_1.docx")]
        public string URL { get; set; }

        [Required]
        [DisplayName("Номер по порядку")]
        public int? Num { get; set; }

        [Required]
        [DisplayName("Срок исполнения, выбор периода")]
        [Column(TypeName = "varchar(30")]
        [StringLength(30)]
        public string TypeIssue { get; set; }


        [DisplayName("Размер оплаты")]
        [Column(TypeName = "varchar(500")]
        [StringLength(500)]
        public string Cost { get; set; }

        //[Required]
        [DisplayName("Срок действия документа(справки) в днях")]
        [Column(TypeName = "varchar(500")]
        [StringLength(500)]
        public string Validaty { get; set; }

        [DisplayName("Кол-во времени, ожидания документов")]
        public int? IssueZapr { get; set; }

        [DisplayName("Тип времени, ожидания документов")]
        public int? TypeIssueZapr { get; set; }
        
        [DisplayName("Срок исполнения, условие исполнения")]
        public int? Issue { get; set; }

        [DisplayName("Отдел ЖП")]
        public bool GP { get; set; }

        [DisplayName("Название процедуры для ЖП")]
        [Column(TypeName = "varchar(400")]
        [StringLength(400)]
        public string NameGP { get; set; }

        [DisplayName("Номер процедуры")]
        [Column(TypeName = "varchar(max")]
        public string UniversalRegNumber { get; set; }

        [DisplayName("ПМС 740")]
        public bool Regulation740 { get; set; }

        [DisplayName("На сайт")]
        public bool ViewSite { get; set; }

        [DisplayName("Размер оплаты на сайт")]
        public int? ViewSiteCost { get; set; }

        [DisplayName("Срок действия документа(справки) на сайт")]
        public int? ViewSiteValidaty { get; set; }

        [DisplayName("Сроки исполнения на сайт")]
        public int? ViewSiteInssue { get; set; }

        [DisplayName("Перечень для сайта")]
        public int? ViewSiteSections { get; set; }


        [DisplayName("??????????????????????????")]
        public bool? ViewImplementing { get; set; }

        [DisplayName("Шаблон на сайт")]
        [Column(TypeName = "varchar(200")]
        [StringLength(200)]
        [RegularExpression(@"^[\w\d_А-Яа-я]*[\.]?[\w]{0,5}", ErrorMessage = "Допускается использовать только буквы и цифры, а также знак подчеркивания (_). Пример шаблон_1.docx")]
        public string URLSite { get; set; }

       
    }
}
