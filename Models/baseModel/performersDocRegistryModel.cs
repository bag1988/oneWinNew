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
    /// Исполнители для процедур(связка)
    /// </summary>
       public class performersDocRegistryModel
    {
       
        public Guid Performers_Id { get; set; }
       
        public Guid DocRegistry_Id { get; set; }

        [ForeignKey("DocRegistry_Id")]
        public docRegModel DocRegistry { get; set; }
        
        [ForeignKey("Performers_Id")]
        public performerModel Performers { get; set; }
    }
}
