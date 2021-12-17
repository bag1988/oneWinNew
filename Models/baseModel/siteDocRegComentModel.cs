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
    /// комменатарий для процедупы(отдел)
    /// </summary>
    public class siteDocRegComentModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Орган")]
        [Required]
        public Guid IdDep { get; set; }
        [Required]
        [DisplayName("Процедура")]
        public Guid IdDoc { get; set; }

        [Required]
        [DisplayName("Коментарий")]
        [Column(TypeName = "varchar(max)")]
        public string Text { get; set; }

        [DisplayName("Ссылка")]
        [Column(TypeName = "varchar(max)")]
        public string Email { get; set; }

        [ForeignKey("IdDoc")]
        public docRegModel DocsReg { get; set; }

        [ForeignKey("IdDep")]
        public departmentModel DepartmentModel  { get; set; }
    }
}
