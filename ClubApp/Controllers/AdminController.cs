using ClubApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ClubApp.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        #region 初始化用户/角色/登陆管理器
        private AppSignInManager _signInManager;
        private AppUserManager _userManager;
        private AppRoleManager _roleManager;

        public AdminController()
        {
        }

        public AdminController(AppUserManager userManager, AppSignInManager signInManager, AppRoleManager roleManager)
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
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles ="Admin")]
        /// <summary>
        /// 一键初始化，为项目填充初始数据
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminFirstSet()
        {
            ViewBag.Msg = "执行结果：";
            //设置角色
            if (!RoleManager.RoleExists("BAdmin"))
            {
                RoleManager.Create(new IdentityRole("BAdmin"));
                ViewBag.Msg += "角色初始化成功；";
            }
            else
            {
                ViewBag.Msg += "角色初始化失败；";
            }
            //申请通道
            if (db.ApplyTypes.Count() == 0)
            {
                List<ApplyType> types = new List<ApplyType>()
                {
                    new ApplyType(){Name = "注册社团", Enable = 1},
                    new ApplyType(){Name = "注销社团", Enable = 1},
                    new ApplyType(){Name = "加入社团", Enable = 1},
                    new ApplyType(){Name = "退出社团", Enable = 1},
                };
                db.ApplyTypes.AddRange(types);
                ViewBag.Msg += "申请通道初始化成功；";
            }
            else
            {
                ViewBag.Msg += "申请通道初始化失败；";
            }
            //社团类型
            if (db.ClubTypes.Count() == 0)
            {
                ClubType club = new ClubType() { Name = "无", Enable = 0 };
                db.ClubTypes.Add(club);
                ViewBag.Msg += "社团类型初始化成功；";
            }
            else
            {
                ViewBag.Msg += "社团类型初始化失败；";
            }
            db.SaveChanges();
            return View();
        }

        public ActionResult EdUserRoles()
        {
            return View();
        }
        public string SetBAdmin(string uid)
        {
            try
            {
                var u = UserManager.FindByName(uid);
                if (u != null)
                {
                    UserManager.AddToRole(u.Id, "BAdmin");
                    return "OK";
                }
                else
                {
                    return "用户" + uid + "不存在";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }      
        }
        public string CancelBAdmin(string uid)
        {
            try
            {
                var u = UserManager.FindByName(uid);
                if (u != null)
                {
                    UserManager.RemoveFromRole(u.Id, "BAdmin");
                    return "OK";
                }
                else
                {
                    return "用户" + uid + "不存在";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #region 用户账号管理
        public ActionResult UserNumber(string Msg = "")
        {
            if (!string.IsNullOrEmpty(Msg))
            {
                ViewBag.Msg = Msg;
            }
            return View(db.UserNumbers.ToList());
        }
        public string LockUserNum(string uid)
        {
            try
            {
                var u = db.UserNumbers.Find(uid);
                if (u != null)
                {
                    u.State = (int)EnumState.系统锁定;
                    db.SaveChanges();
                    return "OK";
                }
                else
                {
                    return "用户" + uid + "不存在";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string UnLockUserNum(string uid)
        {
            try
            {
                var u = db.UserNumbers.Find(uid);
                if (u != null)
                {
                    u.State = (int)EnumState.未使用;
                    db.SaveChanges();
                    return "OK";
                }
                else
                {
                    return "用户" + uid + "不存在";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public ActionResult AddUserNumber()
        {
            AddUserNumModel model = new AddUserNumModel();
            var userinfo = db.UserNumbers.Where(u=>u.UserId!="Admin").Select(u => u.UserId).Max();
            if (userinfo == null)
            {
                model.NowNum = 0;
            }
            else
            {
                model.NowNum = int.Parse(userinfo);
            }
            model.MinNum = model.NowNum + 1;
            model.MaxNum = model.MinNum;
            return View(model);
        }
        [HttpPost]
        public ActionResult AddUserNumber(AddUserNumModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.MinNum > model.NowNum)
                {
                    if (model.MaxNum >= model.MinNum)
                    {
                        List<UserNumber> userlist = new List<UserNumber>();
                        for (int min = model.MinNum; min <= model.MaxNum; min++)
                        {
                            UserNumber newuser = new UserNumber() { UserId = min.ToString(), State = (int)EnumState.未使用 };
                            userlist.Add(newuser);
                        }
                        db.UserNumbers.AddRange(userlist);
                        int addcount = db.SaveChanges();
                        return RedirectToAction("UserNumber", new { Msg = "成功添加" + addcount + "个账户" });
                    }
                    else
                    {
                        ModelState.AddModelError("", "最大序号不能小于起始序号");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "起始序号需要大于当前已存在的最大序号:" + model.NowNum.ToString());
                }

            }
            return View(model);
        }
        #endregion

        #region 社团账号管理
        [HttpGet]
        public ActionResult ClubNumber(string Msg = "")
        {
            if (!string.IsNullOrEmpty(Msg))
            {
                ViewBag.Msg = Msg;
            }
            return View();
        }
        public string LockClubNum(string cid)
        {
            try
            {
                var u = db.ClubNumbers.Find(cid);
                if (u != null)
                {
                    u.State = (int)EnumState.系统锁定;
                    db.SaveChanges();
                    return "OK";
                }
                else
                {
                    return "社团" + cid + "不存在";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string UnLockClubNum(string cid)
        {
            try
            {
                var u = db.ClubNumbers.Find(cid);
                if (u != null)
                {
                    u.State = (int)EnumState.未使用;
                    db.SaveChanges();
                    return "OK";
                }
                else
                {
                    return "社团" + cid + "不存在";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpGet]
        public ActionResult AddClubNumber()
        {
            AddUserNumModel model = new AddUserNumModel();
            var clubinfo = db.ClubNumbers.Select(u => u.ClubId).Max();
            if (clubinfo == null)
            {
                model.NowNum = 0;
            }
            else
            {
                model.NowNum = int.Parse(clubinfo);
            }
            model.MinNum = model.NowNum + 1;
            model.MaxNum = model.MinNum;
            return View(model);
        }
        [HttpPost]
        public ActionResult AddClubNumber(AddUserNumModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.MinNum > model.NowNum)
                {
                    if (model.MaxNum >= model.MinNum)
                    {
                        List<ClubNumber> clublist = new List<ClubNumber>();
                        for (int min = model.MinNum; min <= model.MaxNum; min++)
                        {
                            ClubNumber newclub = new ClubNumber() { ClubId = min.ToString(), State = (int)EnumState.未使用 };
                            clublist.Add(newclub);
                        }
                        db.ClubNumbers.AddRange(clublist);
                        int addcount = db.SaveChanges();
                        return RedirectToAction("ClubNumber", new { Msg = "成功添加" + addcount + "个账户" });
                    }
                    else
                    {
                        ModelState.AddModelError("", "最大序号不能小于起始序号");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "起始序号需要大于当前已存在的最大序号:" + model.NowNum.ToString());
                }

            }
            return View(model);
        }


        #endregion

        #region 社团类型管理
        public ActionResult ClubType(string Msg = "")
        {
            if (!string.IsNullOrEmpty(Msg))
            {
                ViewBag.Msg = Msg;
            }
            return View(db.ClubTypes.ToList());
        }
        [HttpGet]
        public ActionResult AddClubType()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddClubType(ClubType model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var type = db.ClubTypes.Where(t => t.Name == model.Name).FirstOrDefault();
                    if (type == null)
                    {
                        db.ClubTypes.Add(model);
                        int res = db.SaveChanges();
                        return RedirectToAction("ClubType", new { Msg = "成功添加" + res + "个社团类型" });
                    }
                    else
                    {
                        ModelState.AddModelError("", "已存在名为" + model.Name + "的类型，不可重复添加！");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult ClubTypeDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClubType ClubT = db.ClubTypes.Find(id);
            if (ClubT == null)
            {
                return HttpNotFound();
            }
            var clubs = db.ClubNumbers.Where(c => c.Type.Id == ClubT.Id);
            List<ClubsNumberModel> clubsmodel = new List<ClubsNumberModel>();
            foreach (ClubNumber club in clubs)
            {
                ClubsNumberModel clubmodel = new ClubsNumberModel()
                {
                    ClubId = club.ClubId,
                    ClubName = club.Name,
                    State = club.State == null ? "未知" : Enum.GetName(typeof(EnumState), club.State),
                    CreateDate = club.CreateDate == null ? "未知" : club.CreateDate.ToString()
                };
                clubsmodel.Add(clubmodel);
            }
            DelClubTypeModel model = new DelClubTypeModel()
            {
                TypeName = ClubT.Name,
                Enable = ClubT.Enable ?? 0,
                clubs = clubsmodel
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClubTypeDelete(int id)
        {
            ClubType ClubT = db.ClubTypes.Find(id);
            db.ClubTypes.Remove(ClubT);
            db.SaveChanges();
            return RedirectToAction("ClubType", new { Msg = "已成功删除社团类型[ " + ClubT.Name + " ]" });
        }
        public ActionResult ClubTypeOpen(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClubType ClubT = db.ClubTypes.Find(id);
            if (ClubT == null)
            {
                return HttpNotFound();
            }
            if (ClubT.Enable == 1)
            {
                return Content("<script>alert('已启用的类型');window.location.href='../ClubType';</script>");
            }
            ClubT.Enable = 1;
            db.SaveChanges();
            return RedirectToAction("ClubType", new { Msg = "[" + ClubT.Name + "]已启用" });
        }
        public ActionResult ClubTypeClose(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClubType ClubT = db.ClubTypes.Find(id);
            if (ClubT == null)
            {
                return HttpNotFound();
            }
            if (ClubT.Enable == 1)
            {
                ClubT.Enable = 0;
                db.SaveChanges();
                return RedirectToAction("ClubType", new { Msg = "[" + ClubT.Name + "]已关闭" });
            }
            return Content("<script>alert('已关闭的类型');window.location.href='../ClubType';</script>");
        }


        #endregion

        public ActionResult ApplyType(string Msg = "")
        {
            if (!string.IsNullOrEmpty(Msg))
            {
                ViewBag.Msg = Msg;
            }
            return View(db.ApplyTypes.ToList());
        }
        public ActionResult AddApplyType()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddApplyType(ApplyType model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var type = db.ApplyTypes.Where(t => t.Name == model.Name).FirstOrDefault();
                    if (type == null)
                    {
                        db.ApplyTypes.Add(model);
                        int res = db.SaveChanges();
                        return RedirectToAction("ApplyType", new { Msg = "成功添加" + res + "个申请类型" });
                    }
                    else
                    {
                        ModelState.AddModelError("", "已存在名为" + model.Name + "的类型，不可重复添加！");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult ApplyTypeDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplyType AppT = db.ApplyTypes.Find(id);
            if (AppT == null)
            {
                return HttpNotFound();
            }
            var applys = db.ApplyAudits.Where(a => a.Type.Id == AppT.Id&&a.CheckState==(int)EnumAuditState.创建);
            List<ApplyAuditModel> applymodels = new List<ApplyAuditModel>();
            foreach (ApplyAudit apply in applys)
            {
                ApplyAuditModel applymodel = new ApplyAuditModel()
                {
                    Id = apply.Id,
                    Type = apply.Type.Name,
                    ApplyDate = apply.ApplyDate == null ? "未知" : apply.ApplyDate.ToString(),
                    CheckState = apply.CheckState == null ? "未知" : Enum.GetName(typeof(EnumAuditState), apply.CheckState),
                };
                applymodels.Add(applymodel);
            }
            AllTypeModel model = new AllTypeModel()
            {
                TypeName = AppT.Name,
                Enable = AppT.Enable,
                Data = applymodels
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyTypeDelete(int id)
        {
            ClubType ClubT = db.ClubTypes.Find(id);
            db.ClubTypes.Remove(ClubT);
            db.SaveChanges();
            return RedirectToAction("ClubType", new { Msg = "已成功删除社团类型[ " + ClubT.Name + " ]" });
        }
        public ActionResult ApplyTypeOpen(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplyType AppT = db.ApplyTypes.Find(id);
            if (AppT == null)
            {
                return HttpNotFound();
            }
            if (AppT.Enable == 1)
            {
                return Content("<script>alert('已启用的类型');window.location.href='../ApplyType';</script>");
            }
            AppT.Enable = 1;
            db.SaveChanges();
            return RedirectToAction("ApplyType", new { Msg = "[" + AppT.Name + "]已启用" });
        }
        public ActionResult ApplyTypeClose(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplyType AppT = db.ApplyTypes.Find(id);
            if (AppT == null)
            {
                return HttpNotFound();
            }
            if (AppT.Enable == 1)
            {
                AppT.Enable = 0;
                db.SaveChanges();
                return RedirectToAction("ApplyType", new { Msg = "[" + AppT.Name + "]已关闭" });
            }
            return Content("<script>alert('已关闭的类型');window.location.href='../ApplyType';</script>");
        }
        public ActionResult PageList()
        {
            return View();
        }
        public ActionResult PageData(int page, int limit)
        {
            var userdata = db.UserNumbers.OrderBy(u => u.UserId).Skip((page - 1) * limit).Take(limit);
            PageDataModel model = new PageDataModel()
            {
                code = 0,
                msg = "",
                count = db.UserNumbers.Count(),
                data = userdata
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}