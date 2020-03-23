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
                Session["Error"] = "未发现社团" + cid;
                return RedirectToAction("Error404", "Home");
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
                Session["Error"] = "未发现社团" + model.ClubID;
                return RedirectToAction("Error404", "Home");
            }
            //保存活动信息
            Activities Act = new Activities()
            {
                Title1 = model.Title1,
                Title2 = model.Title2,
                Content = model.Content,
                Club = club,
                CreateDate = DateTime.Now,
                User = u,
                State = (int)ActiveState.待提交,
                Votes0="0"
            };
            int type;
            int.TryParse(model.Type, out type);
            Act.Type = type;
            if (model.LabelStr.Length > 1)
            {
                Act.Label = model.LabelStr.Substring(1, model.LabelStr.Length - 1);
            }
            DateTime t1 ;
            DateTime.TryParse(model.Time1, out t1);
            if (t1 < DateTime.Now)
            {
                ModelState.AddModelError("", "活动开始时间不能小于当前时间");
                return View(model);
            }
            Act.Time1 = t1;
            DateTime.TryParse(model.Time2, out t1);
            Act.Time2 = t1;
            db.Activities.Add(Act);
            db.SaveChanges();
            return View("Index");
        }
        [HttpGet]
        public ActionResult Submit(string aid)
        {
            if (aid == null||(int.TryParse(aid,out int intaid) ==false))
            {
                return RedirectToAction("Manage");
            }
            Activities act = db.Activities.Find(intaid);
            if (act == null)
            {
                Session["Error"] = "未发现活动" + aid;
                return RedirectToAction("Error404", "Home");
            }
            if (act.User.UserId != User.Identity.Name)
            {
                Session["Error"] = "访问被拒绝！编号为" + aid + "的活动非当前登陆用户创建";
                return RedirectToAction("Error404", "Home");
            }
            ActiveSubModel model = new ActiveSubModel()
            {
                Id = act.Id,
                Type = act.Type.ToString(),
                State = Enum.GetName(typeof(ActiveState), act.State),
                Title1 = act.Title1,
                Title2 = act.Title2,
                Content = act.Content,
                Area = act.Area == null ? act.Area0 : act.Area.Name,
                Time1 = act.Time1.ToString(),
                Time2 = act.Time2.ToString(),
                MaxUser = act.MaxUser == null ? "无限制" : act.MaxUser.ToString(),
                Labels = act.Label?.Split(',').ToList()
            };

            return View(model);
        }
        [HttpPost]
        public ActionResult Submit([Bind(Include = "Id,ApplyDesc,ApplyFile")]ActiveSubModel model)
        {
            Activities act = db.Activities.Find(model.Id);
            if (act == null)
            {
                Session["Error"] = "未发现活动" + model.Id;
                return RedirectToAction("Error404", "Home");
            }
            ActiveSubModel model1 = new ActiveSubModel()
            {
                Id = act.Id,
                Type = act.Type.ToString(),
                State = Enum.GetName(typeof(ActiveState), act.State),
                Title1 = act.Title1,
                Title2 = act.Title2,
                Content = act.Content,
                Area = act.Area == null ? act.Area0 : act.Area.Name,
                Time1 = act.Time1.ToString(),
                Time2 = act.Time2.ToString(),
                MaxUser = act.MaxUser == null ? "无限制" : act.MaxUser.ToString(),
                Labels = act.Label?.Split(',').ToList()
            };
            try
            {
                if (string.IsNullOrWhiteSpace(model.ApplyFile))
                {
                    ModelState.AddModelError("", "申请任务未上传审批文件！");
                    return View(model1);
                }
                if (act.User.UserId != User.Identity.Name)
                {
                    ModelState.AddModelError("", "非用户" + User.Identity.Name + "创建的活动不能由用户" + User.Identity.Name + "提交！");
                    return View(model1);
                }
                if (act.State != (int)ActiveState.待提交)
                {
                    ModelState.AddModelError("", "请求状态错误不允许提交审批");
                    return View(model1);
                }
                ApplyType type = db.ApplyTypes.Find((int)SQType.创建活动);
                if (type == null || type.Enable != 1)
                {
                    ModelState.AddModelError("", "活动创建申请通道未启用，请联系管理员");
                    return View(model1);
                }

                ApplyAudit apply = new ApplyAudit()
                {
                    Type = db.ApplyTypes.Find((int)SQType.创建活动),
                    ApplicationDesc = model.ApplyDesc,
                    ApplicationFiled = model.ApplyFile,
                    ApplyUser = act.User,
                    Club = act.Club,
                    ApplyDate = DateTime.Now,
                    CheckState = (int)EnumAuditState.创建,
                    AuditTimes = 0
                };
                db.ApplyAudits.Add(apply);
                db.SaveChanges();

                AuditDetail audit = new AuditDetail()
                {
                    ApplyId = apply.Id,
                    CheckState = (int)EnumAuditState.创建,
                    AuditUser = act.User,
                    AuditDate = DateTime.Now
                };
                db.AuditDetails.Add(audit);

                act.State = (int)ActiveState.待审批;
                act.AuditID = apply.Id;

                db.Entry(act).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Manage", "Clubs", new { Msg = "活动编号[" + act.Id + "]一个申请已提交，牢记并使用申请任务凭证[" + apply.Id + "]查看申请进度" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model1);
            }
        }
        [HttpPost]
        public ActionResult Add(ActiveListModel model)
        {
            return View(model);
        }
    }
}