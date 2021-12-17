using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.baseModel
{
    public class attachedFileModel
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Поле, которое содержит ID карточки заявителя, к которой прикрепляется файл
        /// </summary>
        public Guid? RegistrationId { get; set; }

        /// <summary>
        /// Поле, которое содержит URL прикрепляемого файла
        /// </summary>
        [Column(TypeName = "text")]
        public string Url { get; set; }

        /// <summary>
        /// Поле, которое содержит название прикрепляемого файла
        /// </summary>
        [Column(TypeName = "varchar(300)")]
        [StringLength(300)]
        public string Name { get; set; }

        /// <summary>
        /// Поле, которое содержит дату и время прикрепления файла
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? AttachingDateTime { get; set; }

        /// <summary>
        /// Виртуальное свойство, возвращающее сведения о карточке заявителя, к которой прикрепляется файл
        /// </summary>
        [ForeignKey("RegistrationId")]
        public registrationModel Registration { get; set; }
    }
}
