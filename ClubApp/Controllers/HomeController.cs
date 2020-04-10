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
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Notices()
        {
            return View();
        }
        public ActionResult Error()
        {
            return View();
        }
        public ActionResult Error404()
        {
            if (Session["Error"] == null || Session["Error"].ToString() == "0")
            {
                return RedirectToAction("Error");
            }
            ViewBag.Msg = Session["Error"].ToString();
            Session["Error"] = "0";
            return View();
        }
    }
}