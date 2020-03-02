using ClubApp.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClubApp.Controllers
{
    public class ActiveController : Controller
    {
        AppDbContext db = new AppDbContext();

        private AppUserManager _userManager;

        public ActiveController()
        {
        }

        public ActiveController(AppUserManager userManager)
        {
            UserManager = userManager;
        }

        public AppUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Active
        public ActionResult Index(string cid)
        {
            if (cid != null)
            {
                ClubNumber club = db.ClubNumbers.Find(cid);
                if (club != null)
                {
                    ViewBag.ClubId = cid;
                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult Add(string cid)
        {
            if (cid == null)
            {
                return RedirectToAction("Index");
            }
            ClubNumber club = db.ClubNumbers.Find(cid);
            if (club == null)
            {
                return HttpNotFound("未发现社团" + cid);
            }
            ViewBag.Club = club.Name;
            return View();
        }
        public ActionResult Detail(string aid)
        {
            if (aid == null)
            {
                return RedirectToAction("Index");
            }
            if (!int.TryParse(aid, out int id))
            {
                return HttpNotFound("未发现活动" + aid);
            }
            Activities act = db.Activities.Find(id);
            if (act == null)
            {
                return HttpNotFound("未发现活动" + aid);
            }
            return View(act);
        }
        [HttpGet]
        public ActionResult AddAct(string cid)
        {
            if (cid == null)
            {
                return RedirectToAction("Index");
            }
            ClubNumber club = db.ClubNumbers.Find(cid);
            if (club == null)
            {
                return HttpNotFound("未发现社团" + cid);
            }
            ActiveListModel model = new ActiveListModel();
            model.ClubID = club.ClubId;
            model.ClubName = club.Name;
            return View(model);
        }
        [HttpPost,ValidateInput(false),Authorize]
        public ActionResult AddAct(ActiveListModel model)
        {
            ClubNumber club = db.ClubNumbers.Find(model.ClubID);
            UserNumber u = db.UserNumbers.Find(User.Identity.Name);
            if (club == null)
            {
                return HttpNotFound("未发现社团" + model.ClubID);
            }
            Activities Act = new Activities()
            {
                Title1 = model.Title1,
                Title2 = model.Title2,
                Content = model.Content,
                Club = club,
                CreateDate = DateTime.Now,
                User = u,
                State = (int)ActiveState.已创建,
                Votes0="0"
            };
            if (model.LabelStr.Length > 1)
            {
                Act.Label = model.LabelStr.Substring(1, model.LabelStr.Length - 1);
            }
            DateTime t1 ;
            DateTime.TryParse(model.Time1, out t1);
            Act.Time1 = t1;
            DateTime.TryParse(model.Time2, out t1);
            Act.Time2 = t1;
            db.Activities.Add(Act);
            db.SaveChanges();
            return View("Index");
        }
        [HttpPost]
        public ActionResult Add(ActiveListModel model)
        {
            return View(model);
        }
    }
}