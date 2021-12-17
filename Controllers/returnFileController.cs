using Microsoft.AspNetCore.Mvc;
using oneWin.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace oneWin.Controllers
{
    public class returnFileController : Controller
    {
        public IActionResult Index(string urlFile)
        {
            getUrlFile getUrl = new getUrlFile();
            string fullName = getUrl.urlFile() + urlFile;
            if (System.IO.File.Exists(urlFile))
            {
                fullName = urlFile;
            }
            if(System.IO.File.Exists(fullName))
            {
                var f = new FileInfo(fullName);
                var stream = f.OpenRead();
                string fileName = f.Name;               
                return File(stream, "application/pdf", fileName);
            }
            return NoContent();
        }
    }
}
