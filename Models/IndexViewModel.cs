using Microsoft.AspNetCore.Mvc;
using oneWin.Models.baseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models
{
    public class IndexViewModel
    {
        public FilterViewModel FilterViewModel { get; set; }
               
        public IEnumerable<regModel> reg { get; set; }
        public PageViewModel PageViewModel { get; set; }

        public searchFastModel Search { get; set; }
    }

    public class regModel
    {
        [DisplayName("ID")]
        public Guid RegistrationID { get; set; }
                
        [DisplayName("Заявитель")]
        public string LName { get; set; }

        [DisplayName("Название организации")]        
        public string OrgName { get; set; }
               
        [DisplayName("Адрес")]
        public string Address { get; set; }        
                
        [DisplayName("Телефон")]
        public string PhoneNo { get; set; }
                       
        [DisplayName("Регистратор")]
        public string Registrator { get; set; }
        
        [DisplayName("Дата обращения")]
        public DateTime GettingDate { get; set; }
       
        [DisplayName("Дата передачи в отдел")]
        public string OutDeptDate { get; set; }
               
        [DisplayName("Дата возврата из отдела")]
        public string ReturnInDeptDate { get; set; }
       
        [DisplayName("Дата выдачи заявителю")]
        public string IssueDate { get; set; }
       
        [DisplayName("Дата исполнения")]
        public DateTime? MustBeReady { get; set; }
               
        //отправлено, получено, передано в отдел...
        [DisplayName("Состояние")]
        public byte? State { get; set; }
               
        [DisplayName("Комментарии")]
        public string Notes { get; set; }                

        [DisplayName("Номер заявления")]
        public int? DocNo { get; set; }        
                
        [DisplayName("№ и дата решения")]
        public string DateSsolutions { get; set; }
                
        [DisplayName("Исполнитель")]
        public string PerformerName { get; set; }
               
        [DisplayName("Номер процедуры")]
        public string Number { get; set; }

        [DisplayName("Наименование процедуры")]        
        public string RegName { get; set; }

        [DisplayName("Решение")]
        public string Solution { get; set; }

        [DisplayName("Дата уведомления")]       
        public string NotificationDate { get; set; }

        [DisplayName("Удалено")]
        public bool? Deleted { get; set; }

        public string stop { get; set; }
        
    }
}
