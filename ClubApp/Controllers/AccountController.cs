using ClubApp.Models;
using ClubApp.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ClubApp.Controllers
{
    public class AccountController : Controller
    {
        #region 初始化用户/角色/登陆管理器
        private AppSignInManager _signInManager;
        private AppUserManager _userManager;
        private AppRoleManager _roleManager;

        public AccountController()
        {
        }

        public AccountController(AppUserManager userManager, AppSignInManager signInManager, AppRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;

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
        public AppRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<AppRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        AppDbContext db = new AppDbContext();
        #endregion

        // GET: Account
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        #region 登陆
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //可以使用邮箱/手机号/用户名登陆
            var user = (from u in db.Users where u.Email.ToUpper() == model.UserName.ToUpper() || u.PhoneNumber == model.UserName || u.UserName == model.UserName select u).FirstOrDefault();
            if (user == null)
            {
                ModelState.AddModelError("", "账户不存在！");
                return View(model);
            }
            //var userinfo=(from u in db)
            //var identity = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            //SignInManager.AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, identity);
            // 这不会计入到为执行帐户锁定而统计的登录失败次数中
            // 若要在多次输入错误密码的情况下触发帐户锁定，请更改为 shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "无效的登录尝试。");
                    return View(model);
            }
            //return RedirectToLocal(returnUrl);
        }
        [HttpPost]
        public void LogOff()
        {
            SignInManager.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //return RedirectToAction("Index", "Home");
        }
        #endregion

        #region 注册
        public ActionResult EmailRegisterValidate(string Email)
        {
            var user = UserManager.FindByEmail(Email);
            if (user == null)
            {
                return Content("true");
            }
            else
            {
                return Content("false");
            }
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost, ActionName("Register")]
        public async Task<ActionResult> RegisterAsync(EmailRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string userid = GetRandomUserNumber();
                if (string.IsNullOrEmpty(userid))
                {
                    ModelState.AddModelError("", "账号池可注册账号为空，系统暂不允许注册，请联系管理员解决");
                    return View(model);
                }
                var userinfo = db.UserNumbers.Where(u => u.UserId == userid).FirstOrDefault();
                if (userinfo == null || userinfo.State != (int)EnumState.未使用)
                {
                    ModelState.AddModelError("", "账号池可注册账号为空，系统暂不允许注册，请联系管理员解决");
                    return View(model);
                }
                AppUser user = new AppUser()
                {
                    Email = model.Email,
                    UserName = userinfo.UserId
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    userinfo.UserName = userinfo.UserId;
                    userinfo.CreateDate = DateTime.Now;
                    userinfo.State = (int)EnumState.正常;
                    userinfo.Gender = model.Gender;
                    if (model.Gender == (int)Gender.女)
                    {
                        userinfo.HeadImg = "Content/images/head1.png";
                    }
                    else
                    {
                        userinfo.HeadImg = "Content/images/head2.png";
                    }
                    db.SaveChanges();
                    if (userinfo.UserId == "Admin")
                    {
                        await UserManager.AddToRoleAsync(user.Id, "Admin");
                    }
                    //return RedirectToAction("Login");
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "确认你的帐户", "请通过单击 <a href=\"" + callbackUrl + "\">这里</a>来确认你的帐户");
                    return RedirectToAction("RegisterConfirm", new { Email = model.Email });
                }
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult RegisterConfirm(string Email = "")
        {
            if (Email == "")
            {
                return RedirectToAction("Register");
            }
            ViewBag.Msg = Email + "欢迎您注册成功，系统已经自动给您发送注册确认信息，请在邮箱及时查收确认";
            AppUser user = UserManager.FindByEmail(Email);
            string code = UserManager.GenerateEmailConfirmationToken(user.Id);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, protocol: Request.Url.Scheme);
            ViewBag.ConfirmUrl = callbackUrl;
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var userident = UserManager.FindById(userId);
            if (userident == null)
            {
                return View("Error");
            }
            if (userident.EmailConfirmed)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                var user = (from u in db.UserNumbers where u.UserId == userident.UserName select u).FirstOrDefault();
                if (user == null)
                {
                    return View("Error");
                }
                user.CreateDate = DateTime.Now;
                db.SaveChanges();
                RegisterConfirmModel model = new RegisterConfirmModel()
                {
                    Date = DateTime.Now.ToString("yyyy年MM月dd日hh时mm分ss秒"),
                    Num = user.UserId,
                    Index = db.Users.Count()
                };
                return View(model);
            }
            else
            {
                return View("Error");
            }
        }
        [HttpGet]
        public ActionResult PhoneRegister()
        {
            return View();
        }
        public ActionResult PhoneRegisterValidate(string Phone)
        {
            var user = (from u in db.Users where u.PhoneNumber == Phone select u).FirstOrDefault();
            if (user == null)
            {
                return Content("true");
            }
            else
            {
                return Content("false");
            }
        }
        [HttpPost]
        public ActionResult PhoneRegister(PhoneRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

            }
            return View();
        }

        #endregion


        #region 帮助程序
        public ActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// 从账号池中分配随机账号
        /// </summary>
        /// <returns></returns>
        public string GetRandomUserNumber()
        {
            Random r1 = new Random();
            int count = db.UserNumbers.Count(u => u.State == (int)EnumState.未使用);
            if (count > 0)
            {
                int index = r1.Next(0, count);
                var user = db.UserNumbers.Where(u => u.State == (int)EnumState.未使用).OrderBy(u => u.UserId).Skip(count - 1).FirstOrDefault();
                return user.UserId;
            }
            else if (db.UserNumbers.Count()==0)
            {
                UserNumber newuser = new UserNumber() { UserId = "Admin", State = (int)EnumState.未使用 };
                db.UserNumbers.Add(newuser);
                if (!RoleManager.RoleExists("Admin"))
                {
                    Microsoft.AspNet.Identity.EntityFramework.IdentityRole role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("Admin");
                    RoleManager.Create(role);
                }
                db.SaveChanges();
                return "Admin";
            }
            else
            {               
                return null;
            }
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult GetImgCode()
        {
            int width = 100;
            int height = 40;
            int fontsize = 20;
            byte[] bytes = ImgValidateCode.CreateValidateGraphic(out string code, 4, width, height, fontsize);
            Session["ImgCode"] = code;
            return File(bytes, @"image/jpeg");
        }

        [Obsolete]
        public async Task<string> SendMsgCodeAsync(string To, string regtype)
        {
            try
            {
                switch (regtype)
                {
                    case "email":
                        Random r = new Random();
                        int emailcode = r.Next(10000, 100000);
                        EmailSender2 sender = new EmailSender2();
                        await sender.SendEmailAsync(To, "注册凭证", emailcode.ToString(), SendEmailType.注册验证码);
                        Session["EmailCode"] = emailcode;
                        return "信息发送成功";
                    case "phone":
                        string userid = User.Identity.GetUserId();
                        string phonecode = UserManager.GenerateChangePhoneNumberToken(userid, To);
                        if (UserManager.SmsService == null)
                        {
                            return "信息发送失败:未发现SMS短信服务";
                        }
                        else
                        {
                            var message = new IdentityMessage
                            {
                                Destination = To,
                                Body = "你的安全代码是:" + phonecode
                            };
                            await UserManager.SmsService.SendAsync(message);
                            Session["PhoneCode"] = phonecode;
                            return "信息发送成功";
                        }
                    default:
                        return "验证信息发送失败:未能识别的注册类型：" + regtype;
                }
            }
            catch (Exception ex)
            {
                return "信息发送失败，" + ex.Message;
            }
        }
        public string GetEmailCode()
        {
            string res=Session["EmailCode"]==null?"未找到邮箱验证码":"你的邮箱验证码为："+ Session["EmailCode"].ToString();
            return res;
        }
        public ActionResult CheckImgCode(string ImgCode)
        {
            string imgcode = Session["ImgCode"] == null ? "" : Session["ImgCode"].ToString();
            if (string.IsNullOrEmpty(ImgCode) || string.IsNullOrEmpty(imgcode) || imgcode.ToLower() != ImgCode.ToLower())
            {
                return Content("false");
            }
            else
            {
                return Content("true");
            }
        }
        public ActionResult CheckEmailCode(string EmailCode)
        {
            string code = Session["EmailCode"] == null ? "" : Session["EmailCode"].ToString();
            if (string.IsNullOrEmpty(EmailCode) || string.IsNullOrEmpty(code) || code.ToLower() != EmailCode.ToLower())
            {
                return Content("false");
            }
            else
            {
                return Content("true");
            }
        }
        public ActionResult CheckPhoneCode(string PhoneCode)
        {
            string code = Session["PhoneCode"] == null ? "" : Session["PhoneCode"].ToString();
            if (string.IsNullOrEmpty(PhoneCode) || string.IsNullOrEmpty(code) || code.ToLower() != PhoneCode.ToLower())
            {
                return Content("false");
            }
            else
            {
                return Content("true");
            }
        }
        #endregion
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}

