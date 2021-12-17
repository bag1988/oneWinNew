using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.baseModel
{
    public class headModel
    {       
        [Key]
        public Guid HedID { get; set; }

        [Required]
        [DisplayName("Название главы")]
        [Column(TypeName = "varchar(450)")]
        public string Name { get; set; }

        [DisplayName("Номер главы")]
        public int Number { get; set; }

        public List<sectionsModel> Sections { get; set; }
    }
}
