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
    /// запросы
    /// </summary>
    public class msgModel
    {
        [Key]
        public Guid MsgId { get; set; }

        public Guid RegistrationId { get; set; }

        [DisplayName("Имя")]
        [Required]
        [Column(TypeName ="varchar(100)")]
        [StringLength(100)]
        public string FName { get; set; }

        [DisplayName("Отчество")]
        [Required]
        [Column(TypeName = "varchar(100)")]
        [StringLength(100)]
        public string MName { get; set; }

        [DisplayName("Фамилия")]
        [Required]
        [Column(TypeName = "varchar(100)")]
        [StringLength(100)]
        public string LName { get; set; }

        [DisplayName("Дата рождения")]
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime? DOB { get; set; }

        [DisplayName("Тип документа")]
        [Required]
        [Column(TypeName = "varchar(50)")]
        [StringLength(50)]
        public string DocType { get; set; }

        [DisplayName("Серия №/ № записи акта")]
        [Column(TypeName = "varchar(20)")]
        [StringLength(20)]
        public string DocNo { get; set; }

        [DisplayName("Личный номер")]
        [Column(TypeName = "varchar(20)")]
        [StringLength(20)]
        public string PersonalNo { get; set; }

        [DisplayName("Дата выдачи")]
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime? DocIssueDate { get; set; }

        [DisplayName("Кем выдан")]
        [Column(TypeName = "varchar(500)")]
        [StringLength(500)]
        public string DocIssuer { get; set; }

        [DisplayName("Город")]
        [Column(TypeName = "varchar(100)")]
        [StringLength(100)]
        public string City { get; set; }

        [DisplayName("Адрес")]
        [Column(TypeName = "varchar(100)")]
        [StringLength(100)]
        public string Address { get; set; }

        [DisplayName("Дом")]
        [Column(TypeName = "varchar(10)")]
        [StringLength(10)]
        public string Home { get; set; }

        [DisplayName("Квартира")]
        [Column(TypeName = "varchar(10)")]
        [StringLength(10)]
        public string Flat { get; set; }

        [DisplayName("Дата прописки")]
        [Column(TypeName = "datetime")]
        [DataType(DataType.Date)]
        public DateTime? AddressDate { get; set; }

        [DisplayName("Номер квитанции")]
        [Column(TypeName = "varchar(50)")]
        [StringLength(50)]
        public string PayNo { get; set; }

        [DisplayName("Сумма")]
        [Column(TypeName = "real")]
        public float Summ { get; set; }

        [DisplayName("МФО банка")]
        [Column(TypeName = "varchar(500)")]
        [StringLength(500)]
        public string Bank { get; set; }

        //ZaprDocID
        [DisplayName("Запрашиваемый документ")]
        [Required]
        public Guid? Sent { get; set; }

        public byte InterDoc { get; set; }

        [DisplayName("Номер договора")]
        [Column(TypeName = "varchar(20)")]
        [StringLength(20)]
        public string DogNo { get; set; }

        [DisplayName("Дата договора")]
        [Column(TypeName = "datetime")]
        [DataType(DataType.Date)]
        public DateTime? DogDate { get; set; }

        [DisplayName("Дата регистрации")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        public DateTime? IssueDate { get; set; }

        [DisplayName("Организация для осуществления запроса")]
        public Guid? OrganisationID { get; set; }

        [DisplayName("Дата выполнения запроса")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        public DateTime? Srok { get; set; }

        [DisplayName("Комментарий")]
        [Column(TypeName = "text")]
        public string Notes { get; set; }

        [DisplayName("Файл")]
        [Column(TypeName = "text")]
        public string File { get; set; }

        public bool? TypeMSG { get; set; }

        public bool? IsSend { get; set; }

        [Column(TypeName = "datetime")]
        [DataType(DataType.Date)]
        public DateTime? DateofCreatingRequest { get; set; }

        [ForeignKey("RegistrationId")]
        public registrationModel Registration { get; set; }

        [ForeignKey("Sent")]
        public zaprDocModel zaprDoc { get; set; }

        [ForeignKey("OrganisationID")]
        public orgsZaprModel orgsZapr { get; set; }

    }
}
