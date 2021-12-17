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
    /// Поданные заявления
    /// </summary>
    public class registrationModel
    {
        [Key]   
        public Guid RegistrationID { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        [DisplayName("Имя")]
        public string FName { get; set; } //имя

        [DisplayName("Отчество")]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string MName { get; set; } //отчество


        [DisplayName("Фамилия")]
        [Required]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string LName { get; set; } //фамилия


        [DisplayName("Название организации")]
        [MaxLength(300)]
        [Column(TypeName = "varchar(300)")]       
        public string OrgName { get; set; } //организация

        [Required]
        [DisplayName("Тип заявления")]//физ, юр лица и т.д.
        public int? TypeReg { get; set; }

        [MaxLength(100)]
        [Required]
        [Column(TypeName = "varchar(100)")]
        [DisplayName("Нас. пункт")]
        public string City { get; set; }

        [MaxLength(100)]
        [Required]
        [Column(TypeName = "varchar(100)")]
        [DisplayName("Адрес")]
        public string Address { get; set; }

        [MaxLength(50)]
        [Required]
        [Column(TypeName = "varchar(50)")]
        [DisplayName("Дом")]
        public string Home { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        [DisplayName("Квартира")]
        public string Flat { get; set; }

        [MaxLength(30)]
        [Column(TypeName = "varchar(30)")]
        [DisplayName("Телефон")]
        public string PhoneNo { get; set; }


        [DisplayName("Личный №")]
        [MaxLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string PersonalNo { get; set; }

        [DisplayName("Дата выдачи")]
        [Column(TypeName = "datetime")]
        public DateTime? PassIssuerDate { get; set; }

        [DisplayName("Кем выдан")]
        [Column(TypeName = "varchar(100)")]
        public string PassIssuer { get; set; }

        [DisplayName("№ паспорта")]        
        [MaxLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string PassportNo { get; set; }

        [DisplayName("Наименование процедуры")]
        [Required]
        public Guid? RegID { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        [DisplayName("Регистратор")]
        public string Registrator { get; set; }

        [Column(TypeName = "datetime")]
        [DisplayName("Дата обращения")]
        public DateTime? GettingDate { get; set; }

        [Column(TypeName = "datetime")]
        [DisplayName("Дата передачи в отдел")]
        public DateTime? OutDeptDate { get; set; }

        [Column(TypeName = "datetime")]
        [DisplayName("Дата возврата из отдела")]
        public DateTime? ReturnInDeptDate { get; set; }

        [Column(TypeName = "datetime")]
        [DisplayName("Дата выдачи заявителю")]
        public DateTime? IssueDate { get; set; }

        [Column(TypeName = "datetime")]
        [DisplayName("Дата исполнения")]
        public DateTime? MustBeReady { get; set; }

        public bool? Deleted { get; set; }

        //отправлено, получено, передано в отдел...
        [DisplayName("Состояние")]
        public byte? State { get; set; }

        [MaxLength(500)]
        [DataType(DataType.MultilineText)]
        [Column(TypeName = "nvarchar(500)")]
        [DisplayName("Комментарии")]
        public string Notes { get; set; }

        /// <summary>
        /// Этот столбец не нужен вообще
        /// </summary>
        [Column(TypeName = "image")]
        public byte[] RVCContent { get; set; }

        [Column(TypeName = "Номер orderNo")]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int OrderNo { get; set; }

        [DisplayName("Номер заявления")]
        public int? DocNo { get; set; }

        /// <summary>
        /// Этот столбец не нужен вообще
        /// </summary>
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string NPrav { get; set; }

        /// <summary>
        /// Этот столбец не нужен вообще
        /// </summary>
        [Column(TypeName = "numeric(5,2)")]
        public decimal? PIO { get; set; }

        /// <summary>
        /// Этот столбец не нужен вообще
        /// </summary>
        [Column(TypeName = "numeric(5,2)")]
        public decimal? PIG { get; set; }


        /// <summary>
        /// Этот столбец не нужен вообще
        /// </summary>
        public byte? KolB { get; set; }

        /// <summary>
        /// Для увеличения срока
        /// </summary>
        [DisplayName("Решение")]
        public byte? Vid { get; set; }

        // [MaxLength(100)]
        [DisplayName("Исполнитель")]
        public Guid? Organiz { get; set; }

        /// <summary>
        /// Этот столбец не нужен вообще
        /// </summary>
        public byte? Room { get; set; }

        [DisplayName("Кол-во листов")]
        public int? KolList { get; set; }

        [DisplayName("Кол-во приложения")]
        public int? KolListPril { get; set; }

        [DisplayName("Форма подачи")]
        [MaxLength(15)]
        [Column(TypeName = "varchar(15)")]
        public string StatementForm { get; set; }

        [DisplayName("Документ подшит в дело")]
        [MaxLength(15)]
        [Column(TypeName = "varchar(15)")]
        public string CaseNamber { get; set; }

        [Column(TypeName = "datetime")]
        [DisplayName("№ и дата решения")]
        public DateTime? DateSsolutions { get; set; }


        /// <summary>
        /// Этот столбец не нужен вообще
        /// </summary>
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string EvaluationControl { get; set; }


        [DisplayName("Выдано на")]//руки, по почте, иным способом
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string EvaluationNotification { get; set; }

        [DisplayName("Кол-во листов")]
        public byte? LoListCase { get; set; }

        [DisplayName("Административное решение")]
        [MaxLength(15)]
        [Column(TypeName = "varchar(15)")]
        public string NamberSolutions { get; set; }

        [DisplayName("Ход рассмотрения")]
        //[Column(TypeName = "text")]
        public string Proceedings { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        [DisplayName("Мобильный телефон")]
        [DataType(DataType.PhoneNumber)]
        public string MobPhone { get; set; }

        [DisplayName("Email")]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string e_mail { get; set; }

        [DisplayName("Файл регистрационной карты")]
        [Column(TypeName = "text")]
        public string URLKartReg { get; set; }

        [DisplayName("Файл заявления")]
        [Column(TypeName = "text")]
        public string UrlZayav { get; set; }

        [MaxLength(300)]
        [Column(TypeName = "varchar(300)")]
        [DisplayName("Исполнитель")]
        public string PerformerName { get; set; }

        [MaxLength(15)]
        [Column(TypeName = "varchar(15)")]
        [DisplayName("Номер процедуры")]
        public string Number { get; set; }

        [DisplayName("Наименование процедуры")]
        [MaxLength(3000)]
        [Column(TypeName = "varchar(3000)")]
        public string RegName { get; set; }

        [DisplayName("Дата уведомления")]
        [Column(TypeName = "datetime")]
        public DateTime? NotificationDate { get; set; }

        [DisplayName("Номер реестра уведомлений")]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string NotificationRegistryNumber { get; set; }


        [DisplayName("Дополнительные документы")]
        [Column(TypeName = "varchar(MAX)")]
        public string NameDoc_Dop { get; set; }

        [DisplayName("Дополнительные запросы")]
        [Column(TypeName = "varchar(MAX)")]
        public string NameZp_Dop { get; set; }

        [ForeignKey("RegID")]
        public docRegModel DocRegistry { get; set; }

        public virtual ICollection<familyModel> Family { get; set; }

        public virtual solutionModel sol { get; set; }

        public virtual List<SuspendedDocRegistrieModel> suspend { get; set; }

        [ForeignKey("Organiz")]
        public performerModel performer { get; set; }

    }
}
