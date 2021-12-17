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
    /// таблица настроек
    /// </summary>
    public class settingModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [DisplayName("Логин БРТИ")]
        [Column(TypeName = "text")]
        public string LoginBRTI { get; set; }

        [DisplayName("Пароль БРТИ")]
        [Column(TypeName = "text")]
        public string PassBrti { get; set; }

        [DisplayName("Путь для клиент-банка")]
        [Column(TypeName = "text")]
        public string PathBank { get; set; }

        [DisplayName("Телефон")]
        [Column(TypeName = "text")]
        public string Phone { get; set; }

        [DisplayName("Выберите район")]
        public Guid? Areas_Id { get; set; }

        [ForeignKey("Areas_Id")]
        public areaModel area { get; set; }

        [DisplayName("Название организации")]
        [Column(TypeName = "text")]
        public string Name { get; set; }

        [DisplayName("Логин e-mail")]
        [Column(TypeName = "text")]
        public string MailLogin { get; set; }

        [DisplayName("Пароль e-mail")]
        [Column(TypeName = "text")]
        public string MailPass { get; set; }

        [DisplayName("Номер для физ лиц")]
        public int? StartDocNoFis { get; set; }

        [DisplayName("Номер для юр лиц")]
        public int? StartDocNoYur { get; set; }

        [DisplayName("OrderNo")]
        public int? StartOrderNo { get; set; }

        [DisplayName("Путь для отдела жилищной политики")]
        [Column(TypeName = "text")]
        public string PathGP { get; set; }

        [DisplayName("Путь для отдела сайта")]
        [Column(TypeName = "text")]
        public string PathSite { get; set; }

        [DisplayName("Код района")]
        public int? ValueArea { get; set; }

        [DisplayName("E-mail для уведомлений")]
        [Column(TypeName = "text")]
        public string MailLoginForNotification { get; set; }

        [DisplayName("Пароль e-mail для уведомлений")]
        [Column(TypeName = "text")]
        public string MailPassForNotification { get; set; }
    }
}
