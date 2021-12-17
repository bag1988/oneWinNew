using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.baseModel
{
    /// <summary>
    /// Ответственный по процедурам
    /// </summary>
    public class responsibleDocRegistryModel
    {       
        public Guid Performers_Id { get; set; }
      
        public Guid DocRegistry_Id { get; set; }

        [ForeignKey("Performers_Id")]
        public performerModel performer { get; set; }

        [ForeignKey("DocRegistry_Id")]
        public docRegModel DocRegistry { get; set; }

    }
}
