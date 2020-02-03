using ClubApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
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
        public ActionResult Index()
        {
            var userid = UserManager.FindById(User.Identity.GetUserId());
            var userinfo = db.UserNumbers.Where(u => u.UserId == userid.UserName).FirstOrDefault();
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


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}