using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebShell.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using WebShell.ShellScripts;

namespace WebShell.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private Shell webShell;
        private ApparatShell apparatShell;

        public HomeController(ILogger<HomeController> logger, Shell _shell, ApparatShell _apparatShell)
        {
            _logger = logger;
            webShell = _shell;
            apparatShell = _apparatShell;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public string Request(string request,string shell)
        {
            if(webShell.IsWebShellCommand(request,out string result))
            {
                return result;
            }
            else
            {
                return apparatShell.Request(request, shell);
            }
        }
    }
}
