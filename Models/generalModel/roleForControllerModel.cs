using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.generalModel
{
    public class roleForControllerModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [DisplayName("Роль")]
        [Required]
        public string RoleId { get; set; }
        [DisplayName("Контроллер")]
        [Required]
        public int idController { get; set; }

        [ForeignKey("idController")]
        public controlerModel controller { get; set; }
        [ForeignKey("RoleId")]
        public IdentityRole Role { get; set; }

    }
}
