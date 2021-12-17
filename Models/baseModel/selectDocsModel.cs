using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.baseModel
{
    /// <summary>
    /// документы для заявления
    /// </summary>
    public class selectDocsModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid? DocId { get; set; }

        public Guid? RegId { get; set; }

        [ForeignKey("RegId")]
        public registrationModel Registration { get; set; }

        [ForeignKey("DocId")]
        public docsModel docs { get; set; }
    }
}
