using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.generalModel
{
    public class actionModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [DisplayName("Контроллер")]
        [Required]
        public int idController { get; set; }

        [DisplayName("Адрес действия")]
        [Required]
        public string addressAction { get; set; }

        [DisplayName("Наименование действия")]
        [Required]
        public string nameAction { get; set; }

        [ForeignKey("idController")]
        public controlerModel controller { get; set; }

        public virtual List<roleForActionModel> roleAction { get; set; }
    }
}
