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
    /// Приостановка заявления
    /// </summary>
    public class SuspendedDocRegistrieModel
    {
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "datetime")]
        [DisplayName("Дата приостоновления")]
        public DateTime? stopDate { get; set; }

        [Column(TypeName = "datetime")]
        [DisplayName("Дата возобновления")]
        public DateTime? beginDate { get; set; }

        [Column(TypeName = "varchar(250")]
        public string reason { get; set; }

        public Guid? RegistrationId { get; set; }

        public registrationModel Registration { get; set; }
    }
}
