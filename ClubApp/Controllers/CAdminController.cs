using ClubApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ClubApp.Controllers
{
    public class CAdminController : Controller
    {
        AppDbContext db = new AppDbContext();
        // GET: CAdmin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AuditJoinClub(string Msg,int s=0)
        {
            if (!string.IsNullOrEmpty(Msg))
            {
                ViewBag.Msg = Msg;
            }
            List<ApplyAudit> models = new List<ApplyAudit>();
            int st = (int)s;
            if (s > 0 && s < 4)
            {
                models = db.ApplyAudits.Where(a => a.Type.Id == (int)SQType.加入社团 && a.CheckState == st).OrderBy(a => a.Id).ToList();
            }
            else
            {
                models = db.ApplyAudits.Where(a => a.Type.Id == (int)SQType.加入社团).OrderBy(a => a.Id).ToList();
            }

            return View(models);
        }
        public ActionResult AuditJoinClubA(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("AuditJoinClub");
            }
            ApplyAudit apply = db.ApplyAudits.Find(id);
            if (apply == null)
            {
                return HttpNotFound();
            }
            if (apply.Type.Id !=(int) SQType.加入社团)
            {
                return RedirectToAction("AuditJoinClub");
            }
            AuditJoinClubModel model = new AuditJoinClubModel()
            {
                ClubName = apply.Club.Name,
                UserInfo = apply.ApplyUser,
                ApplyDesc = apply.ApplicationDesc,
                ApplyFile = apply.ApplicationFiled,
                ApplyDate = apply.ApplyDate?.ToString(),
                AuditDate = apply.AuditDate?.ToString(),
                AuditId = (int)id,
                AuditTime = (int)apply.AuditTimes,
                State = apply.CheckState == null ? "未知" : Enum.GetName(typeof(EnumAuditState), apply.CheckState)
            };
            return View(model);
        }
        public ActionResult AuditJoinClubAY(int? id, string AuditDesc = "")
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (AuditJoinClubFun(id ?? 0, EnumAuditState.通过, AuditDesc))
            {
                return RedirectToAction("AuditJoinClub", new { Msg = "加入社团申请任务[" + id + "]审批成功" });
            }
            else
            {
                return RedirectToAction("AuditJoinClub", new { Msg = "失败！加入社团申请任务[" + id + "]审批失败" });
            }
        }
        public ActionResult AuditJoinClubAN(int? id, string AuditDesc)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (AuditJoinClubFun(id ?? 0, EnumAuditState.拒绝, AuditDesc))
            {
                return RedirectToAction("AuditJoinClub", new { Msg = "加入社团申请任务[" + id + "]拒绝成功" });
            }
            else
            {
                return RedirectToAction("AuditJoinClub", new { Msg = "失败！加入社团申请任务[" + id + "]拒绝失败" });
            }
        }
        public bool AuditJoinClubFun(int id, EnumAuditState state, string AuditDesc = "")
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
            UserClubs uc = db.UserClubs.Where(u => u.AuditID == id).FirstOrDefault();
            if (AppA.Type == null)
            {
                return false;
            }
            if (uc == null || uc.State != (int)EnumState.待审批)
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
                    uc.State = (int)EnumState.正常;
                    uc.CreateDate = DateTime.Now;
                    uc.Status = (int)UCStatus.会员;
                    db.Entry(uc).State = System.Data.Entity.EntityState.Modified;
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
                uc.State = (int)EnumState.已失效;
                db.Entry(uc).State = System.Data.Entity.EntityState.Modified;
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