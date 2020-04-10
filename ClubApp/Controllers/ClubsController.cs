using ClubApp.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ClubApp.Controllers
{
    [Authorize]
    public class ClubsController : Controller
    {

        AppDbContext db = new AppDbContext();

        private AppUserManager _userManager;

        public ClubsController()
        {
        }

        public ClubsController(AppUserManager userManager)
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
        // GET: Clubs
        public ActionResult Index()
        {
            return View(db.ClubNumbers.Where(c => c.State == (int)EnumState.正常).ToList());
        }
        public ActionResult Club(string cid)
        {
            if (cid == null)
            {
                return RedirectToAction("MyClubs");
            }
            ClubNumber club = db.ClubNumbers.Find(cid);
            if (club == null)
            {
                return HttpNotFound("未发现社团" + cid);
            }
            List<UserClubs> ucs = db.UserClubs.Where(u => u.Club.ClubId == club.ClubId&&u.State>0&&u.State<5).ToList();
            List<Activities> activities = db.Activities.Where(a => a.Club.ClubId == club.ClubId).OrderBy(a=>a.Id).Take(5).ToList();
            List<AnnounceMent> announceMents = db.AnnounceMents.Where(a => a.Club.ClubId == club.ClubId && a.state == (int)EnumState.正常).OrderBy(a=>a.Id).Take(5).ToList();
            ClubViewModel model = new ClubViewModel()
            {
                ClubId = club.ClubId,
                ClubType = club.Type.Name,
                Labels = club.Label?.Split(',').ToList(),
                Name = club.Name,
                HeadImg = club.HeadImg?? "Content/images/head5.jpg",
                ShortDesc = club.ShortDesc,
                Desc = club.Desc,
                State = club.State == null ? "" : Enum.GetName(typeof(EnumState), club.State),
                CreateDate = club.CreateDate2 == null ? "未知" : club.CreateDate2.ToString(),
                User = club.User,
                UserCount = ucs.Count,
                Activities = activities,
                announceMents = announceMents,
                status="0"
            };
            if (User != null)
            {
                UserClubs uc = ucs.Where(u => u.User.UserId == User.Identity.Name).FirstOrDefault();
                if (uc!=null)
                {
                    model.status =Enum.GetName(typeof(UCStatus),uc.Status);
                }
            }

            return View(model);
        }
        public ActionResult ApplyClub(string Msg = "")
        {
            if (!string.IsNullOrEmpty(Msg))
            {
                ViewBag.Msg = Msg;
            }
            ApplyClubModel model = new ApplyClubModel();
            model.clubTypes = db.ClubTypes.ToList();
            return View(model);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyClub(ApplyClubModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    UserNumber thisuser = db.UserNumbers.Find(User.Identity.Name);
                    ApplyType applyType = db.ApplyTypes.Find((int)SQType.注册社团);
                    if (applyType == null || applyType.Enable != 1)
                    {
                        throw new Exception("注册社团的申请通道暂未开通，请持续关注后续开通公告或联系管理员");
                    }
                    ClubNumber club = db.ClubNumbers.Where(c => c.User.UserId == User.Identity.Name).OrderByDescending(c => c.CreateDate).FirstOrDefault();
                    if (club != null && club.CreateDate != null)
                    {
                        if (DateTime.Now.Subtract((DateTime)club.CreateDate).Days < 1)
                        {
                            throw new Exception("每个用户每天只能申请一次社团创建！用户已于" + club.CreateDate.ToString() + "创建过申请！");
                        }
                    }
                    ClubNumber newclub = GetRandomClubNumber();
                    if (newclub == null || newclub.State != (int)EnumState.未使用)
                    {
                        throw new Exception("社团账号已达上限，暂无法新建社团。请联系管理员");
                    }

                    //ApplyAudit apply = new ApplyAudit()
                    //{
                    //    Type = applyType,
                    //    ApplicationDesc = model.ApplyDesc,
                    //    ApplicationFiled = model.ApplyFile,
                    //    ApplyDate=DateTime.Now,
                    //    ApplyUser=db.UserNumbers.Find(User.Identity.Name),
                    //    CheckState=(int)EnumAuditState.已创建,
                    //    AuditTimes=0                      
                    //};
                    newclub.Name = model.Name;
                    if (model.Label?.Length > 1)
                    {
                        newclub.Label = model.Label.Substring(1);
                    }
                    
                    newclub.Desc = model.Desc;
                    newclub.ShortDesc = model.ShortDesc;
                    newclub.CreateDate = DateTime.Now;
                    newclub.Type = db.ClubTypes.Find(model.Type);
                    newclub.User = thisuser;
                    newclub.State = (int)EnumState.待提交;

                    //添加关系
                    UserClubs uc = new UserClubs()
                    {
                        User = thisuser,
                        Club = newclub,
                        Status = (int)UCStatus.社长,
                        CreateDate = DateTime.Now,
                        State=(int)EnumState.正常
                    };
                    db.Entry(newclub).State = System.Data.Entity.EntityState.Modified;
                    db.UserClubs.Add(uc);
                    db.SaveChanges();
                    model = new ApplyClubModel();
                    model.ClubId = newclub.ClubId;
                    ViewBag.Res = "ApplyOK";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }

            }
            model.clubTypes = db.ClubTypes.ToList();
            return View(model);
        }
        [HttpGet]
        public ActionResult JoinClub(string cid)
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
            List<UserClubs> ucs = db.UserClubs.Where(u => u.Club.ClubId == club.ClubId && u.State > 0 && u.State < 5).ToList();
            JoinClubSubModel model = new JoinClubSubModel()
            {
                ClubId = club.ClubId,
                Type = club.Type.Name,
                Name = club.Name,
                HeadImg = club.HeadImg,
                ShortDesc = club.ShortDesc,
                Desc = club.Desc,
                State = Enum.GetName(typeof(EnumState), club.State),
                CreateDate = club.CreateDate == null ? "未知" : club.CreateDate.ToString(),
                User = club.User.UserName,
                UserCount=ucs.Count(),
                CanJoin= !ucs.Where(u => u.User.UserId == User.Identity.Name).Any()
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult JoinClub([Bind(Include = "ClubId,ApplyDesc,ApplyFile")]JoinClubSubModel model)
        {
            try
            {
                ClubNumber club = db.ClubNumbers.Find(model.ClubId);
                UserNumber me = db.UserNumbers.Find(User.Identity.Name);
                if (string.IsNullOrWhiteSpace(model.ApplyFile))
                {
                    ModelState.AddModelError("", "申请任务未上传审批文件！");
                    return View(model);
                }
                if (db.UserClubs.Where(uc => uc.Club.ClubId == model.ClubId && uc.User.UserId == User.Identity.Name && uc.State > 0 && uc.State < 5).Any())
                {
                    ModelState.AddModelError("", "你已经是该社团成员或已申请加入该社团，不允许重复申请加入");
                    return View(model);
                }
                if (club.State != (int)EnumState.正常&& club.State != (int)EnumState.待提交)
                {
                    string state = club.State == null ? "未知" : Enum.GetName(typeof(EnumState), club.State);
                    ModelState.AddModelError("","社团"+club.ClubId+"状态为："+state+ " 不允许申请加入");
                    return View(model);
                }
                ApplyAudit apply = new ApplyAudit()
                {
                    Type = db.ApplyTypes.Find((int)SQType.加入社团),
                    ApplicationDesc = model.ApplyDesc,
                    ApplicationFiled = model.ApplyFile,
                    ApplyUser = me,
                    Club = club,
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
                    AuditUser = me,
                    AuditDate = DateTime.Now
                };
                db.AuditDetails.Add(audit);

                UserClubs newuserClubs = new UserClubs()
                {
                    User = me,
                    Club = club,
                    State = (int)EnumState.待审批,
                    CreateDate = DateTime.Now,
                    Status=(int)UCStatus.申请中,
                    AuditID = apply.Id
                };
                db.UserClubs.Add(newuserClubs);
                db.SaveChanges();
                return RedirectToAction("MyClubs", new { Msg = "加入社团[" + club.ClubId + "]的一个申请已提交，牢记并使用申请任务凭证[" + apply.Id + "]查看申请进度" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
        public ActionResult MyClubs(string Msg = "")
        {
            if (!string.IsNullOrEmpty(Msg))
            {
                ViewBag.Msg = Msg;
            }
            return View();
        }
        public ActionResult MyClubsData(int page, int limit)
        {
            List<UserClubModel> datas = new List<UserClubModel>();
            foreach (UserClubs uc in db.UserClubs.Where(u => u.User.UserId == User.Identity.Name&&u.State<(int)EnumState.已失效).OrderBy(c => c.Id).Skip((page - 1) * limit).Take(limit).ToList())
            {
                UserClubModel data = new UserClubModel()
                {
                    Id = uc.Id,
                    UserId = uc.User.UserId,
                    User = uc.User.UserName,
                    ClubId = uc.Club.ClubId,
                    Club = uc.Club.Name,
                    Status = Enum.GetName(typeof(UCStatus),uc.Status),
                    CreateDate = uc.CreateDate == null ? "未知" : uc.CreateDate.ToString(),
                    Desc = uc.Desc,
                    Enable = uc.State ,
                    state = uc.Club.State ?? 0
                };
                data.CState = Enum.GetName(typeof(EnumState), uc.Club.State);
                datas.Add(data);
            }
            PageDataModel dataModel = new PageDataModel()
            {
                code = 0,
                msg = "",
                count = db.UserClubs.Where(u => u.User.UserId == User.Identity.Name).Count(),
                data = datas.AsQueryable()
            };

            return Json(dataModel, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public ActionResult ApplyClubSubmit(string cid)
        {
            if (cid == null)
            {
                return RedirectToAction("MyClubs");
            }
            ClubNumber club = db.ClubNumbers.Find(cid);
            if (club == null)
            {
                return HttpNotFound("未发现社团" + cid);
            }
            ApplyClubSubModel model = new ApplyClubSubModel()
            {
                ClubId = club.ClubId,
                Type = club.Type.Name,
                Name = club.Name,
                HeadImg = club.HeadImg,
                ShortDesc = club.ShortDesc,
                Desc = club.Desc,
                State = Enum.GetName(typeof(EnumState), club.State),
                CreateDate = club.CreateDate == null ? "未知" : club.CreateDate.ToString(),
                User = club.User.UserName
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyClubSubmit([Bind(Include = "ClubId,ApplyDesc,ApplyFile")]ApplyClubSubModel model)
        {
            try
            {
                ClubNumber club = db.ClubNumbers.Find(model.ClubId);
                if (string.IsNullOrWhiteSpace(model.ApplyFile))
                {
                    ModelState.AddModelError("", "申请任务未上传审批文件！");
                    return View(model);
                }
                if (club.User.UserId != User.Identity.Name)
                {
                    ModelState.AddModelError("", "非用户" + User.Identity.Name + "创建的申请不能由用户" + User.Identity.Name + "提交！");
                    return View(model);
                }
                if (club.State != (int)EnumState.待提交)
                {
                    ModelState.AddModelError("", "请求状态错误不允许提交审批");
                    return View(model);
                }
                ApplyAudit apply = new ApplyAudit()
                {
                    Type = db.ApplyTypes.Find((int)SQType.注册社团),
                    ApplicationDesc = model.ApplyDesc,
                    ApplicationFiled = model.ApplyFile,
                    ApplyUser = club.User,
                    Club = club,
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
                    AuditUser = club.User,
                    AuditDate = DateTime.Now
                };
                db.AuditDetails.Add(audit);

                club.State = (int)EnumState.待审批;
                club.AuditID = apply.Id;
                if (string.IsNullOrEmpty(club.HeadImg))
                {
                    club.HeadImg = "Content/images/head5.jpg";//设置社团默认头像图片
                }
                db.Entry(club).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MyClubs", new { Msg = "社团编号[" + club.ClubId + "]一个申请已提交，牢记并使用申请任务凭证[" + apply.Id + "]查看申请进度" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
        [HttpGet]
        public ActionResult ApplyClubInfo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "未找到指定的社团申请");
            }
            ApplyAudit AppA = db.ApplyAudits.Find(id);
            if (AppA == null)
            {
                return HttpNotFound("未找到指定的社团申请");
            }
            if (AppA.CheckState == (int)EnumAuditState.通过 && AppA.ApplyUser.UserId == User.Identity.Name)
            {
                ClubNumber model = new ClubNumber();
                model = AppA.Club;
                return View(model);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
        [HttpPost]
        public ActionResult ApplyClubInfo(ClubNumber model)
        {
            return View(model);
        }

        public ActionResult Manage(string cid)
        {
            if (cid == null)
            {
                return RedirectToAction("MyClubs");
            }
            ClubNumber club = db.ClubNumbers.Find(cid);
            if (club == null)
            {
                return HttpNotFound("未发现社团" + cid);
            }
            ViewBag.ClubId = cid;
            return View();
        }






        /// <summary>
        /// 从账号池中分配随机账号
        /// </summary>
        /// <returns></returns>
        public ClubNumber GetRandomClubNumber()
        {
            Random r1 = new Random();
            int count = db.ClubNumbers.Count(u => u.State == (int)EnumState.未使用);
            if (count > 0)
            {
                int index = r1.Next(0, count);
                var club = db.ClubNumbers.Where(u => u.State == (int)EnumState.未使用).OrderBy(u => u.ClubId).Skip(index - 1).FirstOrDefault();
                return club;
            }
            else
            {
                return null;
            }
        }
    }
}