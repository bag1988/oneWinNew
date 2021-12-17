using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models
{
    public class userAndRoleModel
    { 
        public userModel user { get; set; }
        public IList<string> userRole { get; set; }

    }
}
