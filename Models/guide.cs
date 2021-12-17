using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models
{
    public class guide
    {
        public string nameTitle { get; set; }
        public List<guideList> list { get; set; }
    }

    public class guideList
    {
        public int id { get; set; }
        [Required]
        public string Name { get; set; }
    }

    public class guideCreate : guideList
    {
        public string nameTitle { get; set; }
    }
}
