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
    /// Нормативные документы
    /// </summary>
    public class normDocModel
    {
        [Key]
        public Guid NormID { get; set; }

        [Required]
        [Column(TypeName="varchar(1500)")]
        [StringLength(1500)]
        [DisplayName("Наименование")]
        public string Name { get; set; }

        [Column(TypeName = "varchar(1500)")]
        [StringLength(1500)]
        [DisplayName("Ссылка")]
        public string URL { get; set; }
    }
}
