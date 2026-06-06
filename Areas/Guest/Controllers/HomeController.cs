using Microsoft.AspNetCore.Mvc;


namespace graduation_proj.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }
    }
}