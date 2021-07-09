using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace SocialApp.API.Controllers
{
    public class Fallback : Controller
    {
        public IActionResult Index(){
            //when route does not match the route that mvc already knows about
            //we run this Index IActionResult
            //physicalpath Directory.CurrentDirectory

            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),
            "wwwroot","index.html"), "text/HTML");
        }
    }
}