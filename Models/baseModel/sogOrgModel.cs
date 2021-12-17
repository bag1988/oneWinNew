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
    /// организации для согласования
    /// </summary>
    public class sogOrgModel
    {
        [Key]
        public Guid SoglOrgID { get; set; }

        [Required]
        [Column(TypeName="varchar(500)")]
        [DisplayName("Название")]
        public string DeptName { get; set; }

        [DisplayName("Адрес")]
        [Column(TypeName = "varchar(500)")]
        public string Addres { get; set; }
        [Column(TypeName = "varchar(20)")]

        [DisplayName("Кабинет")]
        public string Cabinet { get; set; }

        [DisplayName("Телефон")]
        [Column(TypeName = "varchar(20)")]
        public string PhoneNo { get; set; }

        [DisplayName("Комментарий")]
        [Column(TypeName = "varchar(500)")]
        public string Notes { get; set; }

    }
}
