using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using oneWin.Models.baseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Models
{
    [Keyless]
    public class FilterViewModel
    {
        [DisplayName("Начальный номер")]
        public int? startNumber { get; set; }
        [DisplayName("Конечный номер")]
        public int? endNumber { get; set; }
        [DisplayName("Фамилия")]
        public string LName { get; set; }
        [DisplayName("Имя")]
        public string FName { get; set; }
        [DisplayName("Отчество")]
        public string MName { get; set; }
        [DisplayName("Название организации")]
        public string OrgName { get; set; }
        [DisplayName("Телефон")]
        public string PhoneNo { get; set; }
        [DisplayName("Город")]
        public string City { get; set; }
        [DisplayName("Адрес")]
        public string Address { get; set; }
        [DisplayName("Дом")]
        public string Home { get; set; }
        [DisplayName("Квартира")]
        public string Flat { get; set; }

        //Обращение
        [DisplayName("С")]
        [DataType(DataType.Date)]
        public DateTime? StartGetDateTime { get; set; }
        [DisplayName("ПО")]
        [DataType(DataType.Date)]
        public DateTime? EndGetDateTime{ get; set; }

        //Передали в отдел
        [DisplayName("С")]
        [DataType(DataType.Date)]
        public DateTime? StartOutDeptDate { get; set; }
        [DisplayName("По")]
        [DataType(DataType.Date)]
        public DateTime? EndOutDeptDate { get; set; }

        //Вернули из отдела
        [DisplayName("С")]
        [DataType(DataType.Date)]
        public DateTime? StartReturnInDeptDate { get; set; }
        [DisplayName("По")]
        [DataType(DataType.Date)]
        public DateTime? EndReturnInDeptDate { get; set; }

        //Выдали
        [DisplayName("С")]
        [DataType(DataType.Date)]
        public DateTime? StartIssueDate { get; set; }
        [DisplayName("По")]
        [DataType(DataType.Date)]
        public DateTime? EndIssueDate { get; set; }

        //Срок исполнения
        [DisplayName("С")]
        [DataType(DataType.Date)]
        public DateTime? StartMustBeReady { get; set; }
        [DisplayName("По")]
        [DataType(DataType.Date)]
        public DateTime? EndMustBeReady { get; set; }

        //Нарушение сроков исполнения
        [DisplayName("С")]
        [DataType(DataType.Date)]
        public DateTime? StartOutdated { get; set; }
        [DisplayName("По")]
        [DataType(DataType.Date)]
        public DateTime? EndOutdated { get; set; }

        //сортировка
        [DisplayName("Вид")]
        public int? sort { get; set; }


        //Приближение сроков исполнения
        [DisplayName("Контрольная дата")]
        [DataType(DataType.Date)]
        public DateTime? PotentiallyOutdated { get; set; }

        //Административная процедура
        [DisplayName("Глава")]
        public Guid? HeadsID { get; set; } //глава
        [DisplayName("Раздел")]
        public Guid? SectionID { get; set; } //секция 
        [DisplayName("Процедура")]
        public List<Guid> DocRegList { get; set; } //процедура

        //решение
        [DisplayName("Номер решения")]
        public string NumberSolution { get; set; } //номер решения
        [DisplayName("Дата решения")]
        [DataType(DataType.Date)]
        public DateTime? DateSolution { get; set; } //дата решения
        [DisplayName("Вопрос решен")]
        public string SolutionFilter { get; set; } //select: положительно, отрицательно...
        [DisplayName("Выдано")]
        public string EvaluationNotificationFilter { get; set; } //выдано на

        // /// /////// другой раздел
        [DisplayName("Куратор")]
        public Guid? CuratorFiltr { get; set; } //куратор
        [DisplayName("Отдел")]
        public Guid? DepartamentFiltr { get; set; } //отдел
        [DisplayName("Исполнитель")]
        public Guid? PerformerFiltr { get; set; }//исполнитель
        [DisplayName("Регистратор")]
        public string Registrator { get; set; }//регистратор

       
        [DisplayName("Коментарий")]
        public string Notes { get; set; }//коментарий
        [DisplayName("Ход расмотрения")]
        public string Proceeding { get; set; }//ход расмотрения
        [DisplayName("Номер реестра")]
        public string SentNotificationRegistryNumberFilter { get; set; }//номер реестра

        [DisplayName("С")]
        [DataType(DataType.Date)]
        public DateTime? SentNotificationBeginDateFilter { get; set; } //дата направления уведомления с

        [DisplayName("По")]
        [DataType(DataType.Date)]
        public DateTime? SentNotificationEndDateFilter { get; set; }//дата направления уведомления по
    }

    
}
