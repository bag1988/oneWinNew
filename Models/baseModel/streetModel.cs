using Microsoft.EntityFrameworkCore;
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
    /// Названия улиц
    /// </summary>
    [Keyless]
    public class streetModel
    {
        [Column(TypeName="nvarchar(40)")]
        [DisplayName("Наименование")]
        [Required]
        public string Name { get; set; }
    }
}
