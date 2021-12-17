using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.baseModel
{
    public class familyModel
    {
        [Key]
        public Guid FamilyID { get; set; }

        public Guid RegistrationID { get; set; }

        [DisplayName("Имя")]
        [Required]
        [Column(TypeName ="varchar(100)")]
        [StringLength(100)]
        public string FName { get; set; }


        [DisplayName("Отчество")]
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
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime? DOB { get; set; }

        [DisplayName("Родственные отношения")]
        [Required]
        [Column(TypeName = "varchar(40)")]
        [StringLength(40)]
        public string NRotN { get; set; }

        [DisplayName("Город")]
        
        [Column(TypeName = "varchar(100)")]
        [StringLength(100)]
        public string City { get; set; }

        [DisplayName("Улица")]
        
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
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime? AddressDate { get; set; }

        [DisplayName("Номер паспорта")]
        [Column(TypeName = "varchar(20)")]
        [StringLength(20)]
        public string PassportNo { get; set; }

        [DisplayName("Дата выдачи")]
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime? PassIssuerDate { get; set; }

        [DisplayName("Орган выдавший")]
        [Column(TypeName = "varchar(100)")]
        [StringLength(100)]
        public string PassIssuer { get; set; }

        [DisplayName("Личный номер")]
        [Column(TypeName = "varchar(20)")]
        [StringLength(20)]
        public string PersonalNo { get; set; }        

        [ForeignKey("RegistrationID")]
        public registrationModel registration { get; set; }
    }


}
