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
    /// Принятые решения по заявлениям
    /// </summary>
    public class solutionModel
    {
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName ="varchar(50)")]
        [DisplayName("Решение")]
        public string solution { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string solutionNumber { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? dateOfSolution { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? stopDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? beginDate { get; set; }
        public Guid? RegistrationId { get; set; }

        [ForeignKey("RegistrationId")]
        public registrationModel Registration { get; set; }
    }
}
