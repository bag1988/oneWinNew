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
    /// Справочник соглосований
    /// </summary>
    public class soglasovaniyaModel
    {
        [Key]
        public Guid SoglID { get; set; }

        [Required]
        [Column(TypeName="varchar(50)")]
        [DisplayName("Наименование")]
        public string Name { get; set; }
        [DisplayName("Кол-во листов")]
        public byte? CounList { get; set; }

        [DisplayName("Организация")]
        public Guid? SoglOrgID { get; set; }

        [ForeignKey("SoglOrgID")]
        public  sogOrgModel SoglOrg { get; set; }
    }
}
