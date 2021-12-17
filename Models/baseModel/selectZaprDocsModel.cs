using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.baseModel
{
    /// <summary>
    /// запрашиваемые документы для заявления
    /// </summary>
    public class selectZaprDocsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid? ZaprDocId { get; set; }

        public Guid? RegId { get; set; }
        
        [ForeignKey("RegId")]
        public registrationModel Registration { get; set; }

        [ForeignKey("ZaprDocId")]
        public zaprDocModel zaprDoc { get; set; }
    }
}
