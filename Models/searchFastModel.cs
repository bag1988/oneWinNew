using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models
{
    public class searchFastModel
    {
        public int? DocNoFilter { get; set; }
        public string LNameFilter { get; set; }
        public string OrgNameFilter { get; set; }
        public string PhoneNoFilter { get; set; }
        public string AddressFilter { get; set; }
        public DateTime? GettingDateFilter { get; set; }
        public DateTime? OutDeptDateFilter { get; set; }
        public DateTime? ReturnInDeptDateFilter { get; set; }
        public DateTime? IssueDateFilter { get; set; }
        public DateTime? MustBeReadyFilter { get; set; }

        public DateTime? NotificationDateFilter { get; set; }

        public DateTime? DateSsolutionsFilter { get; set; }
        public string PerformerNameFilter { get; set; }

        public string SolutionFilter { get; set; }
        public string NumberFilter { get; set; }
        public string RegistratorFilter { get; set; }               
        public string NotesFilter { get; set; }
        public string PersonalNoFilter { get; set; }
        public string emailFilter { get; set; }

        public int? typePerson { get; set; }

        public int? stateStatment { get; set; }

        public string sortTable { get; set; }

        public bool? sortDesc { get; set; }

    }
}
