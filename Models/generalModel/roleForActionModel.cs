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
    public class roleForActionModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [DisplayName("Роль")]
        public string RoleId { get; set; }
        [DisplayName("Действие")]
        public int idAction { get; set; }

        [ForeignKey("idAction")]
        public actionModel action { get; set; }

        [ForeignKey("RoleId")]
        public IdentityRole Role { get; set; }
    }
}
