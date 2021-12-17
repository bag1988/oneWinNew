using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.generalModel
{
    public class controlerModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [DisplayName("Адрес контроллера")]
        [Required]
        public string addressController { get; set; }

        [DisplayName("Наименование контроллера")]
        [Required]
        public string nameController { get; set; }

        public virtual List<roleForControllerModel> roleController { get; set; }
    }
}
