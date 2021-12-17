using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.baseModel
{
    public class departmentModel
    {
        [Key]
        public Guid Id { get; set; }
       
        [Required]
        [DisplayName("Название отдела")]
        [Column(TypeName ="nchar(500")]
        public string Name { get; set; }

        [DisplayName("Куратор")]
        public Guid? Curator_Id { get; set; }

        [DisplayName("Номер по порядку")]
        public int? Number { get; set; }
     
        [ForeignKey("Curator_Id")]
        public curatorModel Curators { get; set; }
    }
}
