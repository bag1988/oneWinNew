using Microsoft.AspNetCore.Mvc.ModelBinding;
using oneWin.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.baseModel
{
    public class infoFlatModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid? Registration_Id { get; set; }

        [DisplayName("Площадь общая")]
        [Column(TypeName="float")]              
        public double? totalArea { get; set; }

        [DisplayName("Площадь жилая")]
        [Column(TypeName = "float")]
        public double? livingSpace { get; set; }

        [DisplayName("Количество жильцов")]
        public int? personNumber { get; set; }

        [DisplayName("Количество комнат")]
        public int? roomNumber { get; set; }

        [DisplayName("Право заселения")]
        [Column(TypeName = "varchar(50)")]
        [StringLength(50)]
        public string Pravo { get; set; }

        [ForeignKey("Registration_Id")]
        public registrationModel registration { get; set; }
    }

}
