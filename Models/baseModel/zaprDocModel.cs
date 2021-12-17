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
    /// Запрашиваемые документы
    /// </summary>
    public class zaprDocModel
    {
        [Key]
        public Guid ZaprDocID { get; set; }

        [Required]
        [DisplayName("Наименование")]
        [Column(TypeName ="varchar(1500)")]
        public string Name { get; set; }

        [DisplayName("Кол-во дней")]
        public byte? CountDay { get; set; }

        [DisplayName("Документ")]
        [Column(TypeName = "varchar(300)")]
        [RegularExpression(@"^[\w\d_А-Яа-я]*[\.]?[\w]{0,5}", ErrorMessage = "Допускается использовать только буквы и цифры, а также знак подчеркивания (_). Пример шаблон_1.docx")]
        public string File { get; set; }

        [DisplayName("Организация")]
        public Guid? OrgZaprID { get; set; }

        [DisplayName("HTTP")]
        [Column(TypeName = "text")]
        public string HTTP { get; set; }
        [Column(TypeName = "text")]

        [DisplayName("Params")]
        public string Params { get; set; }

        [DisplayName("Имя файла для клиент-банка")]
        [Column(TypeName = "text")]
        public string BankName { get; set; }

        [DisplayName("Клиент-банк")]
        [Column(TypeName = "text")]
        public string BankParams { get; set; }

        [DisplayName("HTTP")]
        public bool HTTPZapr { get; set; }
        [DisplayName("e-mail")]
        public bool e_mailzapr { get; set; }
        [DisplayName("по почте")]
        public bool postzapr { get; set; }
        [DisplayName("Клиент-банк")]
        public bool bankzapr { get; set; }

        [DisplayName("Сумма")]
        [Column(TypeName = "real")]
        public float Summ { get; set; }
       
    }
}
