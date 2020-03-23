using ClubApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ClubApp.Controllers
{
    [Authorize]
    public class BAdminController : Controller
    {
        AppDbContext db = new AppDbContext();
        // GET: BAdmin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Notice()
        {
            return View();
        }

        public ActionResult AuditClub(string Msg = "", int s = 0)
        {
            if (!string.IsNullOrEmpty(Msg))
            {
                ViewBag.Msg = Msg;
            }
            List<ApplyAudit> models = new List<ApplyAudit>();
            int st = (int)s;
            if (s > 0 && s < 4)
            {
                models = db.ApplyAudits.Where(a => a.Type.Id == (int)SQType.注册社团 && a.CheckState == st).OrderBy(a => a.Id).ToList();
            }
            else
            {
                models = db.ApplyAudits.Where(a => a.Type.Id == (int)SQType.注册社团).OrderBy(a => a.Id).ToList();
            }

            return View(models);
        }
        public ActionResult AuditClubA(int? id)
        {
            if (id == null)
            {
                Session["Error"] = "未找到指定的社团申请";
                return RedirectToAction("Error404", "Home");
            }
            ApplyAudit AppA = db.ApplyAudits.Find(id);
            if (AppA == null)
            {
                Session["Error"] = "未找到指定的社团申请" + id;
                return RedirectToAction("Error404", "Home");
            }
            ClubNumber club = AppA.Club;
            if (club == null)
            {
                Session["Error"] = "未从申请任务[" + id + "]中发现社团信息";
                return RedirectToAction("Error404", "Home");
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
                User = club.User.UserName,
                ApplyDesc = AppA.ApplicationDesc,
                ApplyFile = AppA.ApplicationFiled,
                AuditDate = AppA.AuditDate == null ? "未知" : AppA.AuditDate.ToString(),
                AuditTime = AppA.AuditTimes ?? 0,
                AuditId = AppA.Id
            };
            return View(model);
        }
        public ActionResult AuditClubDetail(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("AuditClub");
            }
            List<AuditDetail> audits = db.AuditDetails.Where(a => a.ApplyId == id).OrderBy(a => a.Id).ToList();

            return View(audits);
        }
        public ActionResult AuditClubB()
        {
            return View();
        }
        public ActionResult AuditClubAY(int? id, string AuditDesc = "")
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClubNumber club = db.ClubNumbers.Where(c => c.AuditID == id).FirstOrDefault();
            if (club == null)
            {
                Session["Error"] = "未发现社团";
                return RedirectToAction("Error404", "Home");
            }
            int res = AuditFun(id ?? 0, EnumAuditState.通过, AuditDesc);
            if (res == 1)
            {
                return RedirectToAction("AuditClub", new { Msg = "社团申请任务[" + id + "]审批成功" });
            }
            else if (res == 2) 
            {

                club.State = (int)EnumState.正常;
                club.CreateDate2 = DateTime.Now;
                db.Entry(club).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AuditClub", new { Msg = "社团申请任务[" + id + "]审批成功" });
            }
            else
            {
                return RedirectToAction("AuditClub", new { Msg = "失败！社团申请任务[" + id + "]审批失败" });
            }
        }
        public ActionResult AuditClubAN(int? id, string AuditDesc)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClubNumber club = db.ClubNumbers.Where(c => c.AuditID == id).FirstOrDefault();
            if (club == null)
            {
                Session["Error"] = "未发现社团";
                return RedirectToAction("Error404", "Home");
            }
            int res = AuditFun(id ?? 0, EnumAuditState.拒绝, AuditDesc);
            if (res==1)
            {
                club.State = (int)EnumState.已失效;
                db.Entry(club).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AuditClub", new { Msg = "社团申请任务[" + id + "]拒绝成功" }); ;
            }
            else
            {
                return RedirectToAction("AuditClub", new { Msg = "失败！社团申请任务[" + id + "]拒绝失败" });
            }
        }
        public int AuditFun(int id, EnumAuditState state, string AuditDesc = "")
        {
            ApplyAudit AppA = db.ApplyAudits.Find(id);
            if (AppA == null)
            {
                return 0;
            }
            else if (AppA.CheckState != (int)EnumAuditState.创建)
            {
                return 0;
            }
            //ClubNumber club = AppA.Club;
            if (AppA.Type == null)
            {
                return 0;
            }
            AuditDetail audit = new AuditDetail()
            {
                ApplyId = AppA.Id,
                CheckState = (int)state,
                AuditUser = db.UserNumbers.Find(User.Identity.Name),
                AuditDesc = AuditDesc,
                AuditDate = DateTime.Now
            };
            //AuditDetail audit1 = db.AuditDetails.Where(a => a.ApplyId == AppA.Id).OrderByDescending(a => a.AuditDate).FirstOrDefault();
            //if (audit1 != null)
            //{
            //    audit.FromUser = audit1.AuditUser;
            //}
            if (state == EnumAuditState.通过)
            {
                AppA.AuditTimes += 1;
                AppA.AuditDate = DateTime.Now;
                if (AppA.AuditTimes > 2)
                {
                    AppA.CheckState = (int)state;
                }
                db.Entry(AppA).State = System.Data.Entity.EntityState.Modified;
                db.AuditDetails.Add(audit);
                db.SaveChanges();
                if (AppA.AuditTimes > 2)
                {
                    return 2;
                }
                return 1;
            }
            else if (state == EnumAuditState.拒绝)
            {
                AppA.AuditTimes += 1;

                AppA.CheckState = (int)state;
                AppA.AuditDate = DateTime.Now;
                db.Entry(AppA).State = System.Data.Entity.EntityState.Modified;
                db.AuditDetails.Add(audit);
                db.SaveChanges();
                return 1;
            }
            else
            {
                return 0;
            }

        }
        public ActionResult AuditAct(string Msg = "", int s = 0) 
        {
            if (!string.IsNullOrEmpty(Msg))
            {
                ViewBag.Msg = Msg;
            }
            return View();
        }
        public ActionResult AuditActA(string id)
        {
            if (id == null || (int.TryParse(id, out int intaid) == false))
            {
                return RedirectToAction("Manage");
            }
            Activities act = db.Activities.Where(a => a.AuditID == intaid).FirstOrDefault();
            if (act == null)
            {
                Session["Error"] = "未发现活动" + id;
                return RedirectToAction("Error404", "Home");
            }
            if (act.User.UserId != User.Identity.Name)
            {
                Session["Error"] = "访问被拒绝！编号为" + id + "的活动非当前登陆用户创建";
                return RedirectToAction("Error404", "Home");
            }
            ApplyAudit AppA = db.ApplyAudits.Find(intaid);
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
                Labels = act.Label?.Split(',').ToList(),
                ApplyDesc = AppA.ApplicationDesc,
                ApplyFile = AppA.ApplicationFiled,
                AuditDate = AppA.AuditDate == null ? "未知" : AppA.AuditDate.ToString(),
                AuditTime = AppA.AuditTimes ?? 0,
                AuditId = AppA.Id
            };

            return View(model);
        }
        public ActionResult AuditActAY(int? id, string AuditDesc = "")
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activities act = db.Activities.Where(a => a.AuditID == id).FirstOrDefault();
            if (act == null)
            {
                Session["Error"] = "未发现活动";
                return RedirectToAction("Error404", "Home");
            }
            int res = AuditFun(id ?? 0, EnumAuditState.通过, AuditDesc);
            if (res == 1)
            {
                return RedirectToAction("AuditAct", new { Msg = "活动申请任务[" + id + "]审批成功" });
            }
            else if (res == 2)
            {

                act.State = (int)ActiveState.未开始;
                db.Entry(act).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AuditAct", new { Msg = "活动申请任务[" + id + "]审批成功" });
            }
            else
            {
                return RedirectToAction("AuditAct", new { Msg = "失败！活动申请任务[" + id + "]审批失败" });
            }
        }
        public ActionResult AuditActAN(int? id, string AuditDesc)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activities act = db.Activities.Where(a => a.AuditID == id).FirstOrDefault();
            if (act == null)
            {
                Session["Error"] = "未发现社团";
                return RedirectToAction("Error404", "Home");
            }
            int res = AuditFun(id ?? 0, EnumAuditState.拒绝, AuditDesc);
            if (res == 1)
            {
                act.State = (int)ActiveState.已取消;
                db.Entry(act).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AuditAct", new { Msg = "社团申请任务[" + id + "]拒绝成功" }); ;
            }
            else
            {
                return RedirectToAction("AuditAct", new { Msg = "失败！社团申请任务[" + id + "]拒绝失败" });
            }
        }
    }
}