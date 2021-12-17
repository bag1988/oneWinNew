using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.baseModel
{
    /// <summary>
    /// Нужна ли эта таблица?????????????
    /// </summary>
    public class organizationModel
    {
        [Key]
        public Guid DeptID { get; set; }

        [Required]
        [Column(TypeName="varchar(500)")]
        [StringLength(500)]
        public string DeptName { get; set; }

        [Column(TypeName = "varchar(500)")]
        [StringLength(500)]
        public string Addres { get; set; }

        [Column(TypeName = "varchar(20)")]
        [StringLength(20)]
        public string Cabinet { get; set; }

        [Column(TypeName = "varchar(20)")]
        [StringLength(20)]
        public string PhoneNo { get; set; }

        [Column(TypeName = "varchar(500)")]
        [StringLength(500)]
        public string Notes { get; set; }
    }
}
