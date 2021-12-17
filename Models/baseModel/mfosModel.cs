using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.baseModel
{
    /// <summary>
    /// Коды банков
    /// </summary>
    public class mfosModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Column(TypeName="varchar(30")]
        [StringLength(30)]
        public string MFO { get; set; }
    }
}
