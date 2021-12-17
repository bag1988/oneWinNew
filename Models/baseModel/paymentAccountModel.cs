using Microsoft.EntityFrameworkCore;
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
    /// справочник р/с
    /// </summary>
    public class paymentAccountModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column(TypeName="varchar(2000)")]
        [DisplayName("Наименование")]
        [StringLength(2000)]
        public string Name { get; set; }
    }
}
