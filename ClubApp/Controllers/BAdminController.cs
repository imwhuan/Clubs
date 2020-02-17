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
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "未找到指定的社团申请");
            }
            ApplyAudit AppA = db.ApplyAudits.Find(id);
            if (AppA == null)
            {
                return HttpNotFound();
            }
            ClubNumber club = AppA.Club;
            if (club == null)
            {
                return HttpNotFound("未从申请任务[" + id + "]中发现社团信息");
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
            if (AuditClubFun(id ?? 0, EnumAuditState.通过, AuditDesc))
            {
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
            if (AuditClubFun(id ?? 0, EnumAuditState.拒绝, AuditDesc))
            {
                return RedirectToAction("AuditClub", new { Msg = "社团申请任务[" + id + "]拒绝成功" });
            }
            else
            {
                return RedirectToAction("AuditClub", new { Msg = "失败！社团申请任务[" + id + "]拒绝失败" });
            }
        }
        public bool AuditClubFun(int id, EnumAuditState state, string AuditDesc = "")
        {
            ApplyAudit AppA = db.ApplyAudits.Find(id);
            if (AppA == null)
            {
                return false;
            }
            else if (AppA.CheckState != (int)EnumAuditState.创建)
            {
                return false;
            }
            ClubNumber club = AppA.Club;
            if (AppA.Type == null)
            {
                return false;
            }
            if (club == null || club.Type == null)
            {
                return false;
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
                if (AppA.AuditTimes > 2)
                {
                    AppA.CheckState = (int)state;
                    club.State = (int)EnumState.正常;
                    club.CreateDate2 = DateTime.Now;
                    db.Entry(club).State = System.Data.Entity.EntityState.Modified;
                }
                AppA.AuditDate = DateTime.Now;
                db.Entry(AppA).State = System.Data.Entity.EntityState.Modified;
                db.AuditDetails.Add(audit);
                db.SaveChanges();
                return true;
            }
            else if (state == EnumAuditState.拒绝)
            {
                AppA.AuditTimes += 1;

                AppA.CheckState = (int)state;
                AppA.AuditDate = DateTime.Now;
                club.State = (int)EnumState.已失效;
                db.Entry(club).State = System.Data.Entity.EntityState.Modified;
                db.Entry(AppA).State = System.Data.Entity.EntityState.Modified;
                db.AuditDetails.Add(audit);
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}