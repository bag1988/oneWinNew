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
    /// раздел
    /// </summary>
    public class sectionsModel
    {
        [Key]
        public Guid SectionID { get; set; }

        [StringLength(450)]
        [Column(TypeName = "varchar(450)")]
        [DisplayName("Наименование раздела")]
        public string Name { get; set; }

        [StringLength(3)]
        [Column(TypeName = "varchar(3)")]
        [DisplayName("Номер")]
        public string Number { get; set; }

        [ForeignKey("Heads")]
        public Guid? HeadID { get; set; }

        public  headModel Heads { get; set; }

        public List<docRegModel> DocRegs { get; set; }
    }
}
