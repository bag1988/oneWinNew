using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.generalModel
{
    public class orgIssueModel
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [DisplayName("Наименование")]
        [StringLength(300)]
        public string Name { get; set; }
        [Required]
        [DisplayName("Код")]
        public int kod { get; set; }
    }
}
