using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models
{
    public class loadGPModel
    {

        [DataType(DataType.Date)]
        [DisplayName("Вернулись из отдела")]
        public DateTime ReturnInDeptDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Выданы заявителю")]
        public DateTime IssueDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("С")]
        public DateTime StartDateLoad { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("По")]
        public DateTime EndDateLoad { get; set; }        
        [DisplayName("Отправленны")]
        public string EvaluationNotification { get; set; }
    }
}
