using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using oneWin.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Service
{
    public class enumList
    {
        public enum SortState
        {
            [Display(Name = "По дате передачи в отдел")]
            OutDeptDate,
            [Display(Name = "По дате решения")]
            DateSolution,
            [Display(Name = "По дате вовзрата из отдела")]
            ReturnInDeptDate,
            [Display(Name = "По дате выдачи")]
            IssueDate,
            [Display(Name = "По дате исполнения")]
            MustBeReady,
            [Display(Name = "Старый вариант(до 15.07.2017)")]
            Old
        }
    }

    public class dictionaryList
    {
        public static Dictionary<string, string> otdelList
        {
            get
            {
                return new Dictionary<string, string>() {
                { "len", "Ленинский" },
               { "frun", "Фрунзенский" },
                { "cen", "Центральный" },
                { "mingor", "Мингорисполком" },
                { "mos", "Московский" },
                { "okt", "Октябрьский" },
                { "par", "Партизанский" },
                { "per", "Первомайский" },
                { "sov", "Советский" },
                { "zav", "Заводской" },
                { "test", "Тестовый" }
                };
            }
        }

        public static Dictionary<string, string> typeReports
        {
            get
            {
                return new Dictionary<string, string>() {
                    { "9", "Отчет по общему количеству принятых АП (c разделениям физ. лиц и юр. лиц)" },
                { "1", "Отчет по количеству принятых АП физ. лиц" },
               { "2", "Отчет по количеству принятых АП юр. лиц" },
                { "6", "Отчет по количеству запросов юр. лиц" },
                { "7", "Отчет по количеству других вопросов физ. лиц" },
                { "8", "Отчет по количеству других вопросов юр. лиц" },
                { "3", "Отчет по количеству принятых АП внутренних потребностей" },
                { "4", "Отчет по количеству принятых АП административных жалоб физ. лиц" },
                { "5", "Отчет по количеству принятых АП административных жалоб юр. лиц" }
                };
            }
        }


        public static Dictionary<string, string> typeReg
        {
            get
            {
                return new Dictionary<string, string>() {
                 { "1", "Физические лица" },
                { "2", "Юридические лица" },              
                { "6", "Запросы юридических лиц" },
                { "7", "Другие вопросы физических лиц" },
                { "8", "Другие вопросы юридических лиц" },
                 { "3", "Внутренние потребности" },
                { "4", "Административные жалобы физических лиц" },
                { "5", "Административные жалобы юридических лиц" }
                };
            }
        }


        public static Dictionary<string, string> typeDoc
        {
            get
            {
                return new Dictionary<string, string>() {
                    { "1", "Паспорт РБ" },
                { "2", "Паспорт иностр. гражданина" },
               { "3", "Свидетельство о рождении" },
                { "4", "Свидетельство о заключении брака" },
                { "5", "Свидетельство о расторж. брака" },
                { "6", "Справка органа ЗАГС" },
                { "7", "Решение суда" },
                { "8", "Вид на жительство" },
                { "9", "Доверенность" },
                { "10", "Свидетельство о перемене имени" },
                { "11", "Справка ОгИМ" },
                { "12", "Военный билет" },
                { "13", "Справка из СИЗО, тюрем, колоний" },
                { "14", "Свидетельство о смерти" },
                { "15", "Свидетельство об отцовстве" },
                { "16", "Свидетельство об усын-нии(удоч-нии)" }
                };
            }
        }
                

    }

}
