using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models.generalModel
{
    public class logerModel
    {
        public string userName { get; set; }

        public string otdelName { get; set; }

        public string ipAdres { get; set; }
        public DateTime dateRequest { get; set; }

        public int? actionId { get; set; }
        public string urlRequest { get; set; }
        public string queryRequest { get; set; } 
    }

    public class logerView :logerModel
    {        
        [ForeignKey("actionId")]
        public actionModel action { get; set; }
    }
}
