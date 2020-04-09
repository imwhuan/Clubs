using ClubApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ClubApp.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private AppUserManager _userManager;
        private AppSignInManager _signInManager;
        public UserController() { }
        public UserController(AppUserManager appUserManager, AppSignInManager appsignInManager)
        {
            _userManager = appUserManager;
            _signInManager = appsignInManager;
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
        public AppSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<AppSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        AppDbContext db = new AppDbContext();
        // GET: User
        public ActionResult Index(string uid)
        {
            UserNumber userinfo = new UserNumber();
            if (string.IsNullOrEmpty(uid))
            {
                userinfo = db.UserNumbers.Where(u => u.UserId == User.Identity.Name).FirstOrDefault();
            }
            else
            {
                userinfo = db.UserNumbers.Where(u => u.UserId == uid).FirstOrDefault();
            }
            if (userinfo == null)
            {
                return HttpNotFound("加载用户主页信息失败");
            }
            UserIndexModel model = new UserIndexModel()
            {
                UserId = userinfo.UserId,
                UserName = userinfo.UserName,
                HeadImg = userinfo.HeadImg ?? "/Content/layui/images/face/0.gif",
                State = userinfo.State == null ? "未知" : Enum.GetName(typeof(EnumState), userinfo.State),
                Online = userinfo.OnlineState == null ? "未知" : Enum.GetName(typeof(OnlineState), userinfo.OnlineState),
                Coloege = userinfo.Coloege == null ? "未知" : userinfo.Coloege.Name,
                Class = userinfo.Class ?? "未知",
                RelName = userinfo.RelName ?? "未知",
                Gender = userinfo.Gender == null ? "未知" : Enum.GetName(typeof(Gender), userinfo.Gender),
                Birthday = userinfo.Birthday,
                CreateDate = userinfo.CreateDate,
                LoginDate = userinfo.LoginDate,
                SysAge = "未知",
                Label = "无标签",
                Role = "无角色"
            };
            if (userinfo.CreateDate != null)
            {
                model.SysAge = DateTime.Now.Subtract((DateTime)userinfo.CreateDate).Days.ToString() + "天";
            }

            return View(model);
        }
        public ActionResult UMsg()
        {
            UserNumber model = db.UserNumbers.Where(u => u.UserId == User.Identity.Name).FirstOrDefault();
            if (model == null)
            {
                return HttpNotFound();
            }
            model.Gender = model.Gender ?? 0;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UMsg(UserNumber model)
        {
            if (ModelState.IsValid)
            {
                UserNumber info = db.UserNumbers.Find(User.Identity.Name);
                info.UserName = model.UserName;
                info.Gender = model.Gender;
                info.Birthday = model.Birthday;
                info.Desc = model.Desc;
                info.ShortDesc = model.ShortDesc;
                db.Entry(info).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                ViewBag.Msg = "信息保存成功！页面部分显示信息可能在重新登陆后才能更新生效";
                return View(info);
            }
            else
            {
                ModelState.AddModelError("", "验证未通过");
                return View(model);
            }
        }
        [HttpGet]
        public ActionResult CPwd()
        {
            return View();
        }
        public async Task<ActionResult> CPwd(CPassWorldModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser Tuser = UserManager.FindById(User.Identity.GetUserId());
                string uid = User.Identity.GetUserId();
                var result = UserManager.ChangePassword(uid, model.OldPwd, model.NewPwd);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(uid);
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, false, false);
                        ViewBag.Msg = "密码修改成功";
                    }
                    return View();
                }
                AddErrors(result);
                return View();
            }
            else
            {
                ModelState.AddModelError("", "验证失败");
                return View();
            }
        }
        [HttpGet]
        public ActionResult UserSet()
        {
            AppUser appUser = UserManager.FindById(User.Identity.GetUserId());
            UserNumber u = db.UserNumbers.Find(User.Identity.Name);
            UserSetModel model = new UserSetModel();
            if (u != null)
            {
                model.Email = appUser.Email;
                model.Phone = appUser.PhoneNumber;
                model.RealName = u.RelName;
                model.Cologe = u.Coloege==null?0: u.Coloege.Id;
                model.Grade = (int)u.Gender;
                model.Grade2 = u.Class;
            }
            model.Labels = new List<string>();
            if (!string.IsNullOrEmpty(u.Labels))
            {
                model.Labels = u.Labels.Split(',').ToList();
            }
            model.Cologes = db.Coloeges.ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult UserSet(UserSetModel model)
        {
            UserNumber me = db.UserNumbers.Find(User.Identity.Name);
            me.RelName = model.RealName;
            if (model.Cologe > 0)
            {
                Coloege c = db.Coloeges.Find(model.Cologe);
                if (c != null)
                {
                    me.Coloege = c;
                }
            }
            me.Gender = model.Grade;
            me.Class = model.Grade2;
            if (model.Label?.Length > 1)
            {
                me.Labels = model.Label.Substring(1);
            }
            db.Entry(me).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("UserSet");
        }
        
        public ActionResult Recommend()
        {
            //根据用户标签获取拥有相似标签的对象，目前每组标签推荐2组，不重复
            UserNumber me = db.UserNumbers.Find(User.Identity.Name);
            List<string> labs = new List<string>(); 
            if (!string.IsNullOrEmpty(me.Labels))
            {
                labs = me.Labels.Split(',').ToList();
            }
            //List<string> labs = me.Labels.Length>0?me.Labels.Split(',').ToList():new List<string>();
            List<UserNumber> users = new List<UserNumber>();
            List<Activities> Acts = new List<Activities>();
            List<ClubNumber> Clubs = new List<ClubNumber>();
            //Acts = db.Activities.Where(a => a.Label.Split(',').Any(lab => labs.Contains(lab))).Take(6).ToList();
            //Clubs = db.ClubNumbers.Where(a => a.Label.Split(',').Any(lab => labs.Contains(lab))).Take(6).ToList();
            //users = db.UserNumbers.Where(a => a.Labels.Split(',').Any(lab => labs.Contains(lab))).Take(6).ToList();
            List<string> uid = new List<string>();
            uid.Add(me.UserId);//把自己从推荐用户中排除
            List<int> aid = new List<int>();
            List<string> cid = new List<string>();
            Random r1 = new Random();
            foreach (string lab in labs)
            {
                int ucount = db.UserNumbers.Where(u => u.Labels.Contains(lab) && !uid.Contains(u.UserId)).Count();
                int i = r1.Next(ucount);
                List<UserNumber> us = db.UserNumbers.Where(u =>u.State == (int)EnumState.正常 && u.Labels.Contains(lab) && !uid.Contains(u.UserId)).OrderBy(u=>u.UserId).Skip(i).Take(2).ToList();
                foreach(UserNumber u0 in us)
                {
                    uid.Add(u0.UserId);
                    users.Add(u0);
                }
                List <Activities> ats = db.Activities.Where(a =>a.State == (int)EnumState.正常 && a.Label.Contains(lab) && !aid.Contains(a.Id)).OrderByDescending(a => a.Id).Take(2).ToList();
                foreach(Activities a0 in ats)
                {
                    aid.Add(a0.Id);
                    Acts.Add(a0);
                }
                List <ClubNumber> cs = db.ClubNumbers.Where(c =>c.State==(int)EnumState.正常&& c.Label.Contains(lab) && !cid.Contains(c.ClubId)).OrderByDescending(c => c.ClubId).Take(2).ToList();
                foreach(ClubNumber c0 in cs)
                {
                    cid.Add(c0.ClubId);
                    Clubs.Add(c0);
                }
                //UserNumber u1 = db.UserNumbers.Where(u => u.Labels.Contains(lab)&&!uid.Contains(u.UserId)).FirstOrDefault();
                //Activities a1 = db.Activities.Where(a => a.Label.Contains(lab)&&!aid.Contains(a.Id)).OrderByDescending(a => a.Id).FirstOrDefault();
                //ClubNumber c1 = db.ClubNumbers.Where(c => c.Label.Contains(lab) && !cid.Contains(c.ClubId)).OrderByDescending(c => c.ClubId).FirstOrDefault();
                //if (u1 != null)
                //{
                //    uid.Add(u1.UserId);
                //    users.Add(u1);
                //}
                //if (a1 != null)
                //{
                //    aid.Add(a1.Id);
                //    Acts.Add(a1);
                //}
                //if (c1 != null)
                //{
                //    cid.Add(c1.ClubId);
                //    Clubs.Add(c1);
                //}
            }
            RecommendView model = new RecommendView
            {
                Uid = me.UserId,
                Labels = me.Labels,
                Acts=Acts,
                Clubs=Clubs,
                Users=users
            };
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

    }
}