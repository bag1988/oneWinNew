using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.baseModel
{
    public class areaModel
    {
        [Key]
        public Guid Id { get; set; }

        [DisplayName("Наименование")]
        [Required]
        [Column(TypeName = "nchar(300)")]
        [StringLength(300)]
        public string Name { get; set; }
        [Required]
        [DisplayName("Номер")]
        public int? Number { get; set; }
    }
}
