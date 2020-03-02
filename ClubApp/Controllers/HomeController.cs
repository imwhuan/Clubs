using System.Web.Mvc;

namespace ClubApp.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Error404()
        {
            return View();
        }
    }
}