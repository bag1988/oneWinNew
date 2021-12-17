using oneWin.Models.generalModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace oneWin.Service
{
    public class logger
    {
        private static string logValue = "%LogValue%";
        private static string logSplit = "%LogSplit%";
        private static string wwwroot = "wwwroot/log/";
        private static string pathLogger = wwwroot + DateTime.Now.ToString("d") + "/";
        private static string nameFile = "log.txt";
        public void saveLoger(logerModel logStr)
        {
            if (!Directory.Exists(pathLogger))
                Directory.CreateDirectory(pathLogger);
            List<string> newLine = new();
            foreach (var p in logStr.GetType().GetProperties())
            {
                newLine.Add(p.Name + logValue + p.GetValue(logStr));
            }
            block.Post(string.Join(logSplit, newLine));

        }
        private ActionBlock<string> block;
        public logger()
        {           
            block = new ActionBlock<string>(async message => {
                using (var f = File.Open(pathLogger + nameFile, FileMode.OpenOrCreate, FileAccess.Write))
                {                    
                    f.Position = f.Length;
                    using (var sw = new StreamWriter(f))
                    {
                        await sw.WriteLineAsync(message);
                    }
                }
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 });
        }
        public static async Task<IEnumerable<logerModel>> getLoger(DateTime date, TimeSpan? dateTimeStart = null, TimeSpan? dateTimeStop= null, string userName = null, string controllerName = null, string otdelName = null, string actionName=null)
        {
            var logList = await getLogerList(date);

            return logList.Where(x => (userName == null || x.userName == userName)
            &&(dateTimeStart==null|| x.dateRequest.TimeOfDay>=dateTimeStart)
            && (dateTimeStop == null || x.dateRequest.TimeOfDay <= dateTimeStop)
                 && (otdelName == null || x.otdelName == otdelName)
                 && (!string.IsNullOrEmpty(x.urlRequest))
                 && (controllerName == null||(x.urlRequest.ToLower().Contains("/" + controllerName) || (x.urlRequest == "/" && controllerName == "home")))
                 && (actionName == null ||(x.urlRequest.ToLower().Contains("/" + actionName) || ((x.urlRequest == "/" || Regex.Match(x.urlRequest, @"^(?:\/admin|\/identity)?\/(\w*)(?:\/)?(\w*)?").Groups[2].Value == "") && actionName == "index"))))
                .Take(100);                
        }

        public static async Task<IEnumerable<logerModel>> getLogerList(DateTime? date = null)
        {
            if (date == null)
                date = DateTime.Now;
            string pathLog = wwwroot + date.Value.Date.ToString("d") + "/";
            if (!System.IO.File.Exists(pathLog + nameFile))
                return new List<logerModel>();
            List<logerModel> logList = new();
            logerModel l = new();
            using (StreamReader readLog = new StreamReader(pathLog + nameFile, System.Text.Encoding.Default))
            {
                string str;
                while ((str = await readLog.ReadLineAsync()) != null)
                {                   
                    l = new();
                    foreach (var p in str.Split(logSplit))
                    {
                        var ar = p.Split(logValue);
                        if (l.GetType().GetProperties().Any(x => x.Name == ar[0]))
                        {
                            switch (ar[0])
                            {
                                case "dateRequest": l.dateRequest = Convert.ToDateTime(ar[1]); break;
                                case "actionId": l.actionId = !string.IsNullOrEmpty(ar[1]) ? int.Parse(ar[1]) : null; break;
                                case "userName": l.userName = ar[1]; break;
                                case "otdelName": l.otdelName = ar[1]; break;
                                case "queryRequest": l.queryRequest = ar[1]; break;
                                case "urlRequest": l.urlRequest = ar[1]; break;
                                case "ipAdres": l.ipAdres = ar[1]; break;
                            }
                        }
                    }
                    logList.Add(l);
                }
            }

            return logList.OrderByDescending(x => x.dateRequest).ToList();
        }

        public static async Task<IEnumerable<string>> getUserLogList(DateTime? date = null)
        {
            var list = await logger.getLogerList(date);
            if (!list.Any())
                return new List<string>();
            return list.Where(x => x.userName != null).Select(x => x.userName).Distinct();
        }

        public static async Task<IEnumerable<string>> getControllerLogList(DateTime? date = null)
        {
            var list = await logger.getLogerList(date);
            if (!list.Any())
                return new List<string>();

            return list.Where(x => !string.IsNullOrEmpty(x.urlRequest)).Select(x => (x.urlRequest == "/" ? "home" : Regex.Match(x.urlRequest, @"^(?:\/admin|\/identity)?\/(\w*)(?:\/)?(\w*)?").Groups[1].Value.ToLower())).Distinct();

        }

        public static async Task<IEnumerable<KeyValuePair<string,string>>> getOtdelLogList(DateTime? date = null)
        {
            var list = await logger.getLogerList(date);
            if (!list.Any())
                return new List<KeyValuePair<string, string>>();
            return dictionaryList.otdelList.Where(x => list.Select(x=>x.otdelName).Contains(x.Key)).Distinct();
        }


        public static async Task<IEnumerable<string>> getActionLogList(DateTime? date = null)
        {
            var list = await logger.getLogerList(date);
            if (!list.Any())
                return new List<string>();

            return list.Where(x => !string.IsNullOrEmpty(x.urlRequest)).Select(x => (x.urlRequest == "/" ? "index" : Regex.Match(x.urlRequest, @"^(?:\/admin|\/identity)?\/(\w*)(?:\/)?(\w*)?").Groups[2].Value == "" ? "index" : Regex.Match(x.urlRequest, @"^(?:\/admin|\/identity)?\/(\w*)(?:\/)?(\w*)?").Groups[2].Value.ToLower())).Distinct();

        }

    }
}
